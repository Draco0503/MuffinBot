using Discord;
using Discord.WebSocket;
using MuffinBot.Services.Anime.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace MuffinBot.Services.Anime
{
    public class DanbooruService : IDanbooruService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlSearch;
        private readonly string _urlRandom;
        private readonly string _urlTags;

        public DanbooruService(HttpClient httpClient, string urlSearch, string urlRandom, string urlTags) 
        {
            _httpClient = httpClient;
            // Little trick to make Danbooru API work, otherwise it shows 403 for being a bot request and wont work
            // In this case i make it as if the requests is sent by Mozilla client
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Mozilla", "5.0"));
            _urlSearch = urlSearch;
            _urlRandom = urlRandom;
            _urlTags = urlTags;
        }

        public async Task HandleCommand(SocketSlashCommand command)
        {
            var tag = command.Data.Options.First().Value;

            string title = "DANBOORU", desc = string.Concat("**TAG: ", tag, "**\n"), imgUrl = "";
            Color color;

            var response = await _httpClient.GetAsync(string.Format(_urlSearch, tag));

            if (response == null) 
            {
                desc = string.Concat(desc, "The response return null value");
                color = Color.Red;
            }
            else if (!response.IsSuccessStatusCode)
            {
                desc = string.Concat(desc, "The response was not successfull code:", response.StatusCode);
                color = Color.Gold;
            } else
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var responseContentAsJson = JsonConvert.DeserializeObject<JToken>(jsonContent);
                if (responseContentAsJson.Count() == 0 || Convert.ToInt32(responseContentAsJson.First["post_count"]) == 0) 
                {
                    string similarTags = await GetSimilarTags(tag.ToString());
                    desc = string.Concat(desc, similarTags);
                } else
                {
                    var (s1, s2) = await GetRandomImage(tag.ToString());
                    desc = string.Concat(desc, s1);
                    imgUrl = s2;
                }

                color = Color.Blue;
            }

            var embedMsg = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(desc)
                .WithImageUrl(imgUrl)
                .WithColor(color)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedMsg.Build());
        }

        private async Task<string> GetSimilarTags(string tag) 
        {
            var response = await _httpClient.GetAsync(string.Format(_urlTags, tag));

            if (response == null)
            {
                return "[SIM-TAGS]: Unknown error _null response_ contact with mod.";
            }
            else if (!response.IsSuccessStatusCode)
            {
                return string.Concat("There was an error trying to get similar tags code:", response.StatusCode, ".");
            } else
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var responseContentAsJson = JsonConvert.DeserializeObject<JToken>(jsonContent);
                if (responseContentAsJson.Count() == 0)
                {
                    return "Tag or similar tags not found.";
                } else
                {
                    string msg = "Maybe you meant:\n";
                    foreach (var r in responseContentAsJson)
                    {
                        msg = string.Concat(msg, "**• ", r["name"], ":** ", r["post_count"], " posts\n");
                    }
                    return msg;
                }
            }

        }

        private async Task<(string desc, string imgUrl)> GetRandomImage(string tag)
        {
            var response = await _httpClient.GetAsync(string.Format(_urlRandom, tag));

            if (response == null)
            {
                return (desc: "[RANDOM]: Unknown error _null response_ contact with mod.", imgUrl: "");
            }
            else if (!response.IsSuccessStatusCode)
            {
                return (desc: string.Concat("There was an error trying to get an image code:", response.StatusCode, "."), imgUrl: "");
            }
            else
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var responseContentAsJson = JsonConvert.DeserializeObject<JToken>(jsonContent);

                string characters = responseContentAsJson["tag_string_character"].ToString().Substring(0, 128);
                string desc = $"**character:** {characters}[...]\n**artist:** {responseContentAsJson["tag_string_artist"]}\n**size:** {responseContentAsJson["media_asset"]["image_width"]}x{responseContentAsJson["media_asset"]["image_height"]}\n**source:** {responseContentAsJson["source"]}";
                string imgUrl = responseContentAsJson["file_url"].ToString();

                return (desc, imgUrl);
            }
        }

    }
}
