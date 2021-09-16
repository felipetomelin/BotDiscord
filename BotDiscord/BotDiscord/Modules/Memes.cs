using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace BotDiscord.Modules
{
    public class Memes : ModuleBase
    {
        //Enviar meme com base no reddit
        //Cuidar com nome dos campos json, pode não trazer o meme no discord
        [Command("meme")]
        [Alias("reddit")]
        public async Task Meme(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");

            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("Esse site não existe");
                return;
            }
            
            JArray array = JArray.Parse(result);
            JObject post = JObject.Parse(array[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(119, 13, 133))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($"🦧 {post["num_comments"]} ⬆ {post["ups"]} ");
            
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}