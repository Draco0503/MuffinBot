using Discord;
using Discord.WebSocket;
using MuffinBot.Services.Anime.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MuffinBot.Services.Anime
{
    public class SaucenaoService : ISaucenaoService
    {

        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly string _apiKey;

        public SaucenaoService(HttpClient httpClient, string url, string apiKey)
        {
            _httpClient = httpClient;
            _url = url;
            _apiKey = apiKey;
        }

        public async Task HandleCommand(SocketSlashCommand command)
        {
            var imgUrl = command.Data.Options.First().Value;

            var urlBuilt = string.Format(_url, _apiKey, imgUrl);
            var response = await _httpClient.GetAsync(urlBuilt);

            string title = "SAUCENAO", desc = "";
            Color color;

            if (response == null)
            {
                desc = "The response return null value.";
                color = Color.Red;
            } 
            else if (!response.IsSuccessStatusCode)
            {
                desc = string.Concat("The response was not successfull code: ", response.StatusCode, ".");
                color = Color.Gold;
            } else
            {
                title = string.Concat(title, " SOURCES:");

                var jsonContent = await response.Content.ReadAsStringAsync();
                var responseContentAsJson = JsonConvert.DeserializeObject<JToken>(jsonContent);

                if (responseContentAsJson["header"]["status"].ToString() == "-3")
                {
                    desc = "That's not an image.";
                }
                else if (responseContentAsJson["header"]["status"].ToString() != "0")
                {
                    desc = "Succesfull call but unknown status code please contact with mods.";
                }
                else 
                {
                    var resultList = responseContentAsJson["results"];
                    List<JToken> relevantResults = new List<JToken>();
                    
                    foreach (var r in resultList)
                    {
                        
                        var similarity = Convert.ToDouble(r["header"]["similarity"]);
                        if (similarity > 65)
                        {
                            relevantResults.Add(r);
                        }
                    }

                    if (relevantResults.Count <= 0)
                    {
                        desc = "No relevant results found.";
                    }
                    else
                    {
                        foreach (var r in relevantResults) 
                        {
                            foreach (JProperty dataValue in r["data"])
                            {
                                desc = string.Concat(desc, dataValue.Name, ": ", dataValue.Values().First(), "\n");
                            }
                        }
                        
                    }
                    
                }

                
                color = Color.LighterGrey;
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
