using Discord.WebSocket;
using MuffinBot.Services.Misc.Interfaces;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MuffinBot.Services.Misc
{
    public class TestService : ITestService
    {
        //private readonly HttpClient _httpClient;

        //public TestService(HttpClient httpClient) => _httpClient = httpClient;

        //public async Task<Stream> GetImgAsync()
        //{
        //    var resp = await _httpClient.GetAsync("https://ichef.bbci.co.uk/news/640/cpsprodpb/14EC6/production/_124820758_pug1.jpg");
        //    if (resp.IsSuccessStatusCode)
        //        return await resp.Content.ReadAsStreamAsync();
        //    else
        //        return null;
        //}
        

        public TestService() { }

        public async Task HandleCommand(SocketSlashCommand command)
        {
            await command.RespondAsync($"Working!!!");
        }
    }
}
