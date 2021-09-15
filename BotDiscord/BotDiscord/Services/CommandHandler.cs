using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace BotDiscord.Services
{
    public class CommandHandler
    {
        public static DiscordSocketClient _discord;
        public static CommandService _commandService;
        public static IConfigurationRoot _configurationRoot;
        public static IServiceProvider _serviceProvider;

        public CommandHandler(DiscordSocketClient discord, CommandService commandService,
            IConfigurationRoot configurationRoot, IServiceProvider serviceProvider)
        {
            _discord = discord;
            _commandService = commandService;
            _configurationRoot = configurationRoot;
            _serviceProvider = serviceProvider;

            _discord.Ready += OnReady;
        }

        //Pegar nome de usuário e tag do perfil ex: Nome#0000
        private Task OnReady()
        {
            Console.WriteLine($"Conectado no Bot: \nUsuário: {_discord.CurrentUser.Username}#{_discord.CurrentUser.Discriminator}");
            return Task.CompletedTask;
        }
    }
}