using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using DiscordBotTemplate.Models;
using DiscordBotTemplate.Services;

namespace DiscordBotTemplate
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            try
            {
                // Configure Services
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                // Setup Discord Client
                var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
                var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();

                // Setup Logging
                client.Log += LogAsync;
                serviceProvider.GetRequiredService<CommandService>().Log += LogAsync;

                // Start Discord Client
                await client.LoginAsync(TokenType.Bot, settings.Value.BotToken);
                await client.StartAsync();
                await serviceProvider.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Something really bad happened in main..");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Log.Information(log.ToString());
            return Task.CompletedTask;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("NETCORE_ENV");

            // As it says...
            SerilogSetup();

            // Appsettings Config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Add Discord.Net Services, Logging, AppSettings
            services
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<InteractiveService>()
                .AddLogging(configure => configure.AddSerilog())
                .AddOptions()
                .Configure<AppSettings>(configuration.GetSection("AppSettings"));
        }

        private void SerilogSetup()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                "botlog.txt",
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1000000)
            .CreateLogger();
        }
    }
}
