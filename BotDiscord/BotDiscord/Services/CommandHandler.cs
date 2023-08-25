using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Victoria;

namespace BotDiscord.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commandService;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IServiceProvider _serviceProvider;
        private readonly LavaNode _lavaNode;

        public CommandHandler(DiscordSocketClient discord, CommandService commandService,
            IConfigurationRoot configurationRoot, IServiceProvider serviceProvider, LavaNode lavaNode)
        {
            _discord = discord;
            _commandService = commandService;
            _configurationRoot = configurationRoot;
            _serviceProvider = serviceProvider;
            _lavaNode = lavaNode;

            _discord.Ready += OnReady;
            _discord.MessageReceived += OnMessageReceived;
            _discord.ChannelCreated += OnChannelCreated;
            _discord.JoinedGuild += OnJoinedServer;
            _discord.ReactionAdded += OnReactionAdded;
            _discord.Ready += OnReadyAsync;
        }

        private async Task OnReadyAsync() 
        {
            if (!_lavaNode.IsConnected) {
                await _lavaNode.ConnectAsync();
            }
        }
        
        //Dar cargo ao clicar na reação
        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel channel, SocketReaction message)
        {
            if (message.MessageId != 888205118393176094) return;

            if (message.Emote.Name != "✅") return;

            var role = (channel as SocketGuildChannel).Guild.Roles.FirstOrDefault(x => x.Id == 405917574518538240);
            await (message.User.Value as SocketGuildUser).AddRoleAsync(role);
        }

        //Quando o bot é chamado ao servidor
        private async Task OnJoinedServer(SocketGuild arg)
        {
            await arg.DefaultChannel.SendMessageAsync("Obrigado por me chamar para seu servidor.");
        }

        //Quando criado novo canal dispara esse evento
        private async Task OnChannelCreated(SocketChannel arg)
        {
            if ((arg as ITextChannel) == null) return;
            var canal = arg as ITextChannel;
            await canal.SendMessageAsync("Novo canal criado");
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