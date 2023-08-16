using Discord.WebSocket;
using MuffinBot.Services.Misc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinBot.Services.Misc
{
    public class ReactionService : IReactionService
    {
        private readonly string REACTION_YES_CONTENT = "https://cdn.discordapp.com/attachments/734750766895595581/843267625975021568/yes.gif";
        private readonly string REACTION_NO_CONTENT = "https://cdn.discordapp.com/attachments/734750766895595581/843267512022859816/no.gif";
        private readonly string REACTION_PRAY_CONTENT = "https://cdn.discordapp.com/attachments/649025469219340288/853772957028319292/unknown.png";
        private readonly string REACTION_PLEASE_CONTENT = "https://i1.sndcdn.com/avatars-Izsdy6YmsiXZk1Sr-8AXfwA-t500x500.jpg";
        private readonly string REACTION_SMUG_CONTENT = "https://cdn.discordapp.com/attachments/734750766895595581/843268167089258517/smug.jpg";
        private readonly string REACTION_TREMBLING_CONTENT = "https://cdn.discordapp.com/attachments/709788450408366162/972992631456030830/the-quintessential-quintuplets-itsuki.gif";
        private readonly string REACTION_PEKORA_CONTENT = "https://cdn.discordapp.com/attachments/734750766895595581/843268060445016105/pekora.jpg";
        private readonly string REACTION_HAACHAM_CONTENT = "https://cdn.discordapp.com/attachments/734750766895595581/843267894686908442/haachama.jpg";

        public async Task HandleCommand(SocketSlashCommand command)
        {
            var cmd = command.Data.Name;
            var content = "That reaction is not covered";
            switch (cmd) 
            {
                case var _ when cmd == CommandConstants.REACTION_YES_COMMAND_NAME:
                    content = REACTION_YES_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_NO_COMMAND_NAME:
                    content = REACTION_NO_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_PRAY_COMMAND_NAME:
                    content = REACTION_PRAY_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_PLEASE_COMMAND_NAME:
                    content = REACTION_PLEASE_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_SMUG_COMMAND_NAME:
                    content = REACTION_SMUG_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_TREMBLING_COMMAND_NAME:
                    content = REACTION_TREMBLING_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_PEKORA_COMMAND_NAME:
                    content = REACTION_PEKORA_CONTENT;
                    break;
                case var _ when cmd == CommandConstants.REACTION_HAACHAM_COMMAND_NAME:
                    content = REACTION_HAACHAM_CONTENT;
                    break;
            }
            await command.RespondAsync(content);
        }
    }
}
