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
    public class PekofyService : IPekofyService
    {
        public async Task HandleCommand(SocketSlashCommand command)
        {
            // NOTE: Discord API does not allow to reply the same message that executes the command
            // so there is a modification of the command, so now you must give it the text right after
            var msg = (string)command.Data.Options.First().Value;
            string pekofiedMsg;

            if (msg.EndsWith("?")) 
            {
                pekofiedMsg = string.Concat(msg, " peko?");
            }
            else if (msg.EndsWith("!"))
            {
                pekofiedMsg = string.Concat(msg, " peko!");
            } else
            {
                pekofiedMsg = string.Concat(msg, " peko.");
            }

            await command.RespondAsync(pekofiedMsg);
        }
    }
}
