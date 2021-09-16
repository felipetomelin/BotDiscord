using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BotDiscord.Modules
{
    public class Moderacao : ModuleBase
    {
        //Deletar mensagens do chat
        [Command("deletar")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var deletedMessage = await Context.Channel.SendMessageAsync($"{messages.Count() - 1} mensagens deletadas");
            await Task.Delay(5000);
            await deletedMessage.DeleteAsync();
        }
    }
}