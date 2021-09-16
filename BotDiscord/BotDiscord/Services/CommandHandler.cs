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
            _discord.MessageReceived += OnMessageReceived;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;

            if (msg.Author.IsBot) return;

            var context = new SocketCommandContext(_discord, msg);

            int posicao = 0;

            if (msg.HasStringPrefix(_configurationRoot["prefix"], ref posicao) ||
                msg.HasMentionPrefix(_discord.CurrentUser, ref posicao))
            {
                var result = await _commandService.ExecuteAsync(context, posicao, _serviceProvider);

                if (!result.IsSuccess)
                {
                    var erro = result.Error;
                    await context.Channel.SendMessageAsync($"Ocorreu um erro: \n {erro}");
                    Console.WriteLine(erro);
                }
            }
        }

        //Pegar nome de usuário e tag do perfil ex: Nome#0000
        private Task OnReady()
        {
            Console.WriteLine($"Conectado no Bot: \nUsuário: {_discord.CurrentUser.Username}#{_discord.CurrentUser.Discriminator}");
            return Task.CompletedTask;
        }
    }
}