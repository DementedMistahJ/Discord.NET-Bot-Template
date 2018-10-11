# Discord Bot Template using [Discord.Net](https://raw.githubusercontent.com/RogueException/Discord.Net)

An easy way to get started with .NET Core 2, Discord.NET, and AppSettings DI

## Installation
You'll want to create an appsettings.development.json file. This should look exactly like appsettings.json, 
except that it will hold your development environment variables. By default your working environment in Visual Studio will
be set to development.

There are 4 settings that need to be added:

* **BotName:** The name of your bot as it is (include spaces, etc)
* **BotOwner:** Your name
* **BotSite:** Your website
* **BotToken:** The most import one. This is what brings your bot to life. Get it from your bot under [Discord Developer Applications](https://discordapp.com/developers/applications)

You'll need to add the bot to a server to test it. Run the bot, and use @botname help
i.e. ```@MyTestBot help```