﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinBot.Services
{
    public interface ISlashCommandHandler
    {
        Task HandleCommand(SocketSlashCommand command);
    }
}
