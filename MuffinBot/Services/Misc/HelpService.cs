using Discord;
using Discord.WebSocket;
using MuffinBot.Services.Misc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinBot.Services.Misc
{
    public class HelpService : IHelpService
    {

        public async Task HandleCommand(SocketSlashCommand command)
        {
            var embedMsg = new EmbedBuilder()
                .WithTitle(CommandConstants.HELP_CONTEXT_TITLE)
                .WithDescription(CommandConstants.HELP_CONTEXT_MSG)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedMsg.Build());
        }
    }
}
