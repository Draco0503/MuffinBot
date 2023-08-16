using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MuffinBot.Services.Anime.Interfaces;
using MuffinBot.Services.Misc.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MuffinBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;
        private ISlashCommandHandler _commandHandler;

        public CommandHandlingService(IServiceProvider serviceProvider)
        {
            _commandService = serviceProvider.GetRequiredService<CommandService>();
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            _serviceProvider = serviceProvider;

            //_commandService.CommandExecuted += CommandExecutedAsync;
            //_client.MessageReceived += MessageReceivedAsync;
            _client.SlashCommandExecuted += SlashCommandHandler;
        }

        public async Task InitialiteAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            // await command.RespondAsync($"You executed {command.Data.Name} command");
            _commandHandler = null;
            var cmd = command.Data.Name;
            switch (cmd) 
            {
                // TEST
                case var _ when cmd == CommandConstants.TEST_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<ITestService>();
                    break;
                // HELP
                case var _ when cmd == CommandConstants.HELP_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<IHelpService>();
                    break;
                // REACTIONS
                case var _ when cmd == CommandConstants.REACTION_YES_COMMAND_NAME || cmd == CommandConstants.REACTION_NO_COMMAND_NAME ||
                                cmd == CommandConstants.REACTION_PRAY_COMMAND_NAME || cmd == CommandConstants.REACTION_PLEASE_COMMAND_NAME ||
                                cmd == CommandConstants.REACTION_SMUG_COMMAND_NAME || cmd == CommandConstants.REACTION_TREMBLING_COMMAND_NAME ||
                                cmd == CommandConstants.REACTION_PEKORA_COMMAND_NAME || cmd == CommandConstants.REACTION_HAACHAM_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<IReactionService>();
                    break;
                // PEKOFY
                case var _ when cmd == CommandConstants.PEKOFY_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<IPekofyService>();
                    break;
                // ANIME - ANILIST
                case var _ when cmd == CommandConstants.ANILIST_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<IAnilistService>();
                    break;
                case var _ when cmd == CommandConstants.SAUCENAO_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<ISaucenaoService>();
                    break;
                case var _ when cmd == CommandConstants.DANBOORU_COMMAND_NAME:
                    _commandHandler = _serviceProvider.GetRequiredService<IDanbooruService>();
                    break;
            }
            if (_commandHandler != null) 
            {
                await _commandHandler.HandleCommand(command);
            }
            else
            {
                await command.RespondAsync($"{command.Data.Name} command in progress...");
            }
        }

        //public async Task MessageReceivedAsync(SocketMessage socketMessage)
        //{
        //    if (!(socketMessage is SocketUserMessage msg)) return;
        //    if (msg.Source != MessageSource.User) return;

        //    var argPos = 0;
        //    Console.WriteLine(msg.Content.ToString());
        //    if (!msg.HasCharPrefix('/', ref argPos)) return;

        //    var context = new SocketCommandContext(_client, msg);
        //    await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
        //}

        //public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        //{
        //    if (!command.IsSpecified) return;
        //    if (result.IsSuccess) return;
        //    await context.Channel.SendMessageAsync($"error: {result}");
        //}
    }
}
