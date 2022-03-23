using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;

namespace BotDiscord.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        public List<string> ThumbnailPrimeiraMusica = new List<string>();

        public Music(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string query)
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Você precisa estar conectado a um canal!");
                return;
            }

            try
            {
                if (!_lavaNode.HasPlayer(Context.Guild))
                {
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                }
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Não estou conectado a nenhum canal.");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Não foi informado link ou nome da musica para procura.");
                return;
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"Não foi possivel achar o video desejado `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            
            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);
                
                var thumbnail = await track.FetchArtworkAsync();
                ThumbnailPrimeiraMusica.Add(thumbnail);
                
                var campos = player.Queue.Select((x, i) => new EmbedFieldBuilder().WithName((i+1).ToString()).WithValue(x.Title + "\n Duração: "+x.Duration)).Reverse();
                
                var builder = new EmbedBuilder()
                    .WithTitle("Musicas:")
                    .WithFields(campos)
                    .AddField("Tocando agora:", player.Queue.First().Title)
                    .WithColor(new Color(119, 13, 133))
                    .WithImageUrl(ThumbnailPrimeiraMusica[0]);
                
                var embed = builder.Build();
                
                var messages = await Context.Channel.GetMessagesAsync( 2).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
                
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);
                
                var thumbnail = await track.FetchArtworkAsync();

                ThumbnailPrimeiraMusica.Add(thumbnail);
                
                var builder = new EmbedBuilder()
                    .WithTitle($"Musica: {track.Title}")
                    .WithDescription(track.Duration.ToString())
                    .WithColor(new Color(119, 13, 133))
                    .WithImageUrl(thumbnail);
                var embed = builder.Build();
                await player.PlayAsync(track);

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }

        [Command("skip", RunMode = RunMode.Async)]
        public async Task Skip()
        {
            var player = _lavaNode.GetPlayer(Context.Guild);
            var voiceState = Context.User as IVoiceState;
            
            

            if (player.Queue.Count == 0)
            {
                await ReplyAsync("Não há mais musicas na fila!");
                return;
            }

            await player.SkipAsync();
            await ReplyAsync($"Skiped! Agora tocando {player.Track.Title}!");
        }

        [Command("pause", RunMode = RunMode.Async)]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not already connected to a voice channel!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Você precisa estar no mesmo canal que eu!");
                return;
            }

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("A musica já está pausada!");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync($"Musica pausada {player.Track.Title}!");
        }

        [Command("resume", RunMode = RunMode.Async)]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not already connected to a voice channel!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Você precisa estar no mesmo canal que eu!");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("A musica já está tocando!");
                return;
            }

            await player.ResumeAsync();
            await ReplyAsync($"Musica tocando {player.Track.Title}!");
        }
    }
}