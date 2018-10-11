using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBotTemplate.Models;

namespace DiscordBotTemplate.Modules
{
    public class GeneralModule : ModuleBase
    {
        private CommandService _service;
        public DiscordSocketClient DiscordSocketClient { get; set; }
        public IOptions<AppSettings> AppSettings { get; set; }

        public GeneralModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("help")]
        [Remarks("Lists all commands")]
        public async Task Help()
        {
            var cmdList = new List<string>();

            var embed = new EmbedBuilder
            {
                Color = new Color(114, 137, 218),
                Title = $"{Context.Client.CurrentUser.Username} | Commands | Prefix: @{AppSettings.Value.BotName}"
            };

            foreach (var module in _service.Modules)
            {
                cmdList.AddRange(module.Commands
                    .Where(command => command.Summary != "private")
                    .Select(command => $"`@{AppSettings.Value.BotName} {command.Summary}` - {command.Remarks}").ToList());
            }

            embed.Description = string.Join("\n\n", cmdList);

            embed.AddField($"Owner: AppSettings.Value.BotOwner",
               $"Visit: <{AppSettings.Value.BotSite}>");

            await ReplyAsync("", false, embed.Build());
        }
    }
}
