using Discord;
using Discord.WebSocket;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using MuffinBot.Services.Anime.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MuffinBot.Services.Anime
{
    public class AnilistService : IAnilistService
    {
        private readonly string STATUS_FINISH = "FINISHED";
        private readonly string STATUS_NOT_RELEASED = "NOT_YET_RELEASED";


        private readonly IGraphQLClient _graphQLClient;

        public AnilistService(IGraphQLClient graphQLClient) => _graphQLClient = graphQLClient;
        
        public async Task HandleCommand(SocketSlashCommand command)
        {
            string title, desc = "";
            Color color;
            var animeName = (string)command.Data.Options.First().Value;
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                    query($name: String) {
                        Media (search: $name, type: ANIME) {
                            episodes
                            status(version: 2)
                            season
                            seasonYear
                            nextAiringEpisode{
                                timeUntilAiring
                                episode
                            }
                            title {
                                romaji
                            }
                        }
                    }
                    ",
                    Variables = new { name = animeName }
                };
                
                var response = await _graphQLClient.SendQueryAsync<object>(query) ?? throw new Exception("Response value cannot be null");

                // Convert the json to a dictionary
                var animeData = JsonConvert.DeserializeObject<Dictionary<string, object>>(JObject.Parse(response.Data.ToString()).GetValue("Media").ToString());
                title = JObject.Parse(animeData["title"].ToString()).First.First.ToString();
                var status = animeData["status"].ToString();
                if (status == STATUS_FINISH)
                {
                    desc = $"The anime has **already ended**. ({animeData["episodes"]} episodes)";
                } else if (status == STATUS_NOT_RELEASED)
                {
                    if (animeData["seasonYear"] == null)
                    {
                        desc = $"Unknown release date.";
                    }
                    else
                    {
                        if (animeData["season"] == null)
                        {
                            desc = $"Airing in **{animeData["seasonYear"]}**.";
                        }
                        else
                        {
                            desc = $"Airing in **{animeData["seasonYear"]}, {animeData["season"].ToString().ToLower()}**.";
                        }
                    }
                } else if (animeData["nextAiringEpisode"] != null)
                {
                    var nextEpisodeData = JsonConvert.DeserializeObject<Dictionary<string, object>>(JObject.Parse(animeData["nextAiringEpisode"].ToString()).ToString());
                    
                    var time = TimeSpan.FromSeconds(double.Parse(nextEpisodeData["timeUntilAiring"].ToString()));
                    var episode = nextEpisodeData["episode"];

                    desc = $"Episode **{episode}** airs in **{time.ToString(@"d' days, 'hh\:mm\:ss")}**";
                }
                
                color = Color.DarkBlue;
 
            }
            catch (Exception ex)
            {
                // TODO: See if there is a better method
                if (ex.Message.Contains("NotFound"))
                {
                    title = animeName;
                    desc = "The anime you are looking for is not register in Anilist";
                    color = Color.Gold;
                }
                else
                {
                    title = "Sorry, something went wrong";
                    desc = ex.Message;
                    color = Color.Red;
                }
                
            }
            var embedMsg = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(desc)
                .WithColor(color)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedMsg.Build());
        }
    }
}
