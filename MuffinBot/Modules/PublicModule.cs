using Discord.Commands;
using System.Threading.Tasks;
using MuffinBot.Services;
using System.IO;
using Discord.Interactions;

namespace MuffinBot.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        //public TestService TestService { get; set; }

        //[SlashCommand("hello", "test")]
        //public Task TestAsync() => ReplyAsync("Working!!!");

        //[SlashCommand("img", "sends an image")]
        //public async Task ImgTestAsync()
        //{
        //    var stream = await TestService.GetImgAsync();
        //    if (stream == null) await ReplyAsync($"There was an error processing the URL");
        //    else
        //    {
        //        stream.Seek(0, SeekOrigin.Begin);
        //        await Context.Channel.SendFileAsync(stream, "file.jpg");
        //    }
        //}
    }
}
