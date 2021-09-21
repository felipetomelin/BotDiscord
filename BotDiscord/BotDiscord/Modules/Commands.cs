using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BotDiscord.Modules
{
    public class Commands : ModuleBase
    {
        //Enviar mensagem por trigger
        [Command("teste")]
        public async Task Teste()
        {
            await Context.Channel.SendMessageAsync("Mensagem de teste retorno!");
        }
        
        //Card de entrada no servidor e check para dar cargo
        [Command("verificar")]
        public async Task Verificar()
        {
            var imgFooter =
                "https://cdn.discordapp.com/attachments/654672723846496266/887889620254224384/31780396ad86894b9271e46e2a18442174b236d0_full.png";
            
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle("Bem vindo ao Cartel!")
                .WithColor(new Color(119, 13, 133))
                .AddField("Como começar?",
                    "Após ler a constituição, clique no check para tornar-se um SUB-HUMANO e começar a usar o servidor!", true)
                .WithFooter("É os guri e não tem como", imgFooter);
            
            var embed = builder.Build();
            
            var mensagemInicial = await Context.Channel.SendMessageAsync(null, false, embed);
            var checkEmoji = new Emoji("\u2705");
            
            await mensagemInicial.AddReactionAsync(checkEmoji);
        }
        
        //Informações de quem executa o comando User
        [Command("template")]
        public async Task Info(SocketGuildUser user = null)
        {
            var imgFooter =
                "https://cdn.discordapp.com/attachments/654672723846496266/887889620254224384/31780396ad86894b9271e46e2a18442174b236d0_full.png";
            
            if (user == null)
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription($"Informações sobre você {Context.User.Username}")
                    .WithColor(new Color(119, 13, 133))
                    .AddField("User ID", Context.User.Id, true)
                    .AddField("Criado em", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)
                    .AddField("Entrou em", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                    .AddField("Cargos", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                    .WithFooter("É os guri e não tem como", imgFooter)
                    .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription("Informações sobre você")
                    .WithColor(new Color(119, 13, 133))
                    .AddField("User ID", user.Id, true)
                    .AddField("Criado em", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                    .AddField("Entrou em", (user as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                    .AddField("Cargos", string.Join(" ", (user as SocketGuildUser).Roles.Select(x => x.Mention)))
                    .WithFooter("É os guri e não tem como", imgFooter)
                    .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }

        //Informações do server
        [Command("server")]
        public async Task Server()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Informações sobre o server")
                .WithTitle($"{Context.Guild.Name} Info")
                .WithColor(new Color(119, 13, 133))
                .AddField("Membros", (Context.Guild as SocketGuild).MemberCount, true)
                .AddField("Membros online",(Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count(), true);
            
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        
        
        
    }
}