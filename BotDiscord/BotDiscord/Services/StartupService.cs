using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace BotDiscord.Services
{
    public class StartupService
    {
        public static IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _command;
        private readonly IConfigurationRoot _configuration;

        public StartupService(IServiceProvider provider, DiscordSocketClient discord, CommandService command,
            IConfigurationRoot configuration)
        {
            _provider = provider;
            _discord = discord;
            _command = command;
            _configuration = configuration;
        }

        public async Task StartAsync()
        {
            var token = _configuration["tokens:discord"];

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Por favor insira seu Discord Token no arquivo _config.yml");
                return;
            }

            await _discord.LoginAsync(TokenType.Bot, token);
            await _discord.StartAsync();

            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}