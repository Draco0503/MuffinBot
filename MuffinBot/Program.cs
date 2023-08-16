using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MuffinBot.Services;
using System.Collections.Generic;
using Discord.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System.IO;
using MuffinBot.Services.Misc.Interfaces;
using MuffinBot.Services.Misc;
using MuffinBot.Services.Anime.Interfaces;
using MuffinBot.Services.Anime;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace MuffinBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IConfiguration _config;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            // LOAD THE CONFIG FILE
            _config = new ConfigurationBuilder()
                          .SetBasePath(basePath: string.Concat(Directory.GetCurrentDirectory(), @"\config"))
                          .AddJsonFile(path: "config.json")
                          .Build();
            // LOAD DEPENDENCY INJECTION
            using (var services = CreateProvider())
            {
                // Discord Web Socket
                _client = services.GetRequiredService<DiscordSocketClient>();

                // Log Funcs
                _client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Load the custom commands
                _client.Ready += ClientReady;

                // Getting the token from the config file
                var token = _config["Bot_Data:token"];

                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                // Initialite Services
                await services.GetRequiredService<CommandHandlingService>().InitialiteAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private ServiceProvider CreateProvider()
        {   
            // DEPENDENCY INJECTION
            return new ServiceCollection()
                       .AddSingleton(new DiscordSocketConfig
                       {
                           GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                       })
                       .AddSingleton<DiscordSocketClient>()
                       .AddSingleton<CommandService>()
                       .AddSingleton<CommandHandlingService>()
                       .AddSingleton<ITestService, TestService>()
                       .AddSingleton<IHelpService, HelpService>()
                       .AddSingleton<IReactionService, ReactionService>()
                       .AddSingleton<IPekofyService, PekofyService>()
                       .AddSingleton<IGraphQLClient>(s => new GraphQLHttpClient(_config["URLs:anilist"], new NewtonsoftJsonSerializer()))
                       .AddSingleton<IAnilistService, AnilistService>()
                       .AddSingleton<ISaucenaoService, SaucenaoService>(s => new SaucenaoService(new HttpClient(), _config["URLs:saucenao"], _config["Bot_Data:saucenao_key"]))
                       .AddSingleton<IDanbooruService, DanbooruService>(s => new DanbooruService(new HttpClient(), _config["URLs:danbooru:search"], _config["URLs:danbooru:random"], _config["URLs:danbooru:similar_tags"]))
                       .BuildServiceProvider();
        }

        #region ADD COMMANDS TO GUILDS
        public async Task ClientReady()
        {
            List<SlashCommandBuilder> commands = new List<SlashCommandBuilder>();
            var auxCommand = new SlashCommandBuilder();
            // TEST COMMAND
            auxCommand.WithName(CommandConstants.TEST_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.TEST_COMMAND_DESC);
            commands.Add(auxCommand);
            // HELP COMMAND
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.HELP_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.HELP_COMMAND_DESC);
            commands.Add(auxCommand);
            // COMMUN MUSIC COMMANDS

            // MUSIC GOT BY ANILIST COMMANDS

            // DANBOORU COMMAND
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.DANBOORU_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.DANBOORU_COMMAND_DESC);
            auxCommand.AddOption(name: CommandConstants.DANBOORU_COMMAND_OPTION_NAME,
                                  type: ApplicationCommandOptionType.String,
                                  description: CommandConstants.DANBOORU_COMMAND_OPTION_DESC,
                                  isRequired: true);    // SHOULD BE OPTIONAL BUT NOT NOW
            commands.Add(auxCommand);
            // SAUCENAO COMMAND
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.SAUCENAO_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.SAUCENAO_COMMAND_DESC);
            auxCommand.AddOption(name: CommandConstants.SAUCENAO_COMMAND_OPTION_NAME,
                                  type: ApplicationCommandOptionType.String,
                                  description: CommandConstants.SAUCENAO_COMMAND_OPTION_DESC,
                                  isRequired: true);
            commands.Add(auxCommand);
            // ANILIST COMMAND
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.ANILIST_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.ANILIST_COMMAND_DESC);
            auxCommand.AddOption(name: CommandConstants.ANILIST_COMMAND_OPTION_NAME,
                                  type: ApplicationCommandOptionType.String,
                                  description: CommandConstants.ANILIST_COMMAND_OPTION_DESC,
                                  isRequired: true);
            commands.Add(auxCommand);
            // REACTION COMMANDS
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_YES_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_NO_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_PRAY_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand); 
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_PLEASE_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_SMUG_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_TREMBLING_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_PEKORA_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.REACTION_HAACHAM_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.REACTION_COMMUN_DESC);
            commands.Add(auxCommand);
            // PEKOFY COMMAND
            auxCommand = new SlashCommandBuilder();
            auxCommand.WithName(CommandConstants.PEKOFY_COMMAND_NAME);
            auxCommand.WithDescription(CommandConstants.PEKOFY_COMMAND_DESC);
            auxCommand.AddOption(name: CommandConstants.PEKOFY_COMMAND_OPTION_NAME, 
                                  type: ApplicationCommandOptionType.String, 
                                  description: CommandConstants.PEKOFY_COMMAND_OPTION_DESC, 
                                  isRequired: true);
            commands.Add(auxCommand);

            // TODO: REVISE IF THERE IS A BETTER WAY
            // Adds the commands to each guild
            foreach (var guild in _client.Guilds)
            {
                foreach (var command in commands)
                {
                    try
                    {
                        await guild.CreateApplicationCommandAsync(command.Build()); // THIS WORKS
                        //await _client.CreateGlobalApplicationCommandAsync(command.Build()); // THIS TOO BUT I CANNOT SEE IT
                    }
                    catch (HttpException acex)
                    {
                        var json = JsonConvert.SerializeObject(acex.Errors, Formatting.Indented);
                        Console.WriteLine(json);
                    }
                }
            }
            
        }
        #endregion ADD COMMANDS TO GUILDS

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        public static bool IsDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }
    // CLASS THAT CONTAINS MOST OF THE CONSTANTS OF THE PROGRAM
    #region COMMAND NAMES AND ITS DESCRIPTIONS
    public static class CommandConstants
    {
        // TEST COMMANDS
        public static readonly string TEST_COMMAND_NAME = "test";
        public static readonly string TEST_COMMAND_DESC = "It is what it is, just TEST";
        // HELP COMMAND
        public static readonly string HELP_COMMAND_NAME = "help";
        public static readonly string HELP_COMMAND_DESC = "Shows help message where you can find the usages of this bot";
        public static readonly string HELP_CONTEXT_TITLE = "Help Command";

        // COMMUN MUSIC COMMANDS
        public static readonly string MUSIC_PLAY_COMMAND_NAME = "play";
        public static readonly string MUSIC_PLAY_COMMAND_DESC = "Plays the song given if its possible";
        public static readonly string MUSIC_PLAY_COMMAND_OPTION_NAME = "option"; // REQUIRED
        public static readonly string MUSIC_PLAY_COMMAND_OPTION_DESC = "Song's name or the URL or the index of the search [1 - 5]";
        public static readonly string MUSIC_LIST_COMMAND_NAME = "playlist";
        public static readonly string MUSIC_LIST_COMMAND_DESC = "Shows current playlist";
        public static readonly string MUSIC_LOOP_COMMAND_NAME = "loop";
        public static readonly string MUSIC_LOOP_COMMAND_DESC = "Modifies loop settings [off/single/all]";
        public static readonly string MUSIC_LOOP_COMMAND_OPTION_NAME = "mode"; // REQUIRED
        public static readonly string MUSIC_LOOP_COMMAND_OPTION_DESC = "Loop modes";
        public static readonly string MUSIC_LOOP_COMMAND_OPTION_MODE_OFF = "off";
        public static readonly string MUSIC_LOOP_COMMAND_OPTION_MODE_SINGLE = "single";
        public static readonly string MUSIC_LOOP_COMMAND_OPTION_MODE_ALL = "all";
        public static readonly string MUSIC_SKIP_COMMAND_NAME = "skip";
        public static readonly string MUSIC_SKIP_COMMAND_DESC = "Skips the current song";
        public static readonly string MUSIC_SKIP_COMMAND_OPTION_NAME = "number"; // OPTIONAL
        public static readonly string MUSIC_SKIP_COMMAND_OPTION_DESC = "If no number provided, skips to the next song";
        public static readonly string MUSIC_SONG_COMMAND_NAME = "song";
        public static readonly string MUSIC_SONG_COMMAND_DESC = "Displays information of the song that is currently playing";

        public static readonly string MUSIC_EMPTY_COMMAND_NAME = "empty";
        public static readonly string MUSIC_EMPTY_COMMAND_DESC = "Clears the playlist";
        public static readonly string MUSIC_REMOVE_COMMAND_NAME = "remove";
        public static readonly string MUSIC_REMOVE_COMMAND_DESC = "Removes specific song from the playlist";
        public static readonly string MUSIC_REMOVE_COMMAND_OPTION_NAME = "number"; // REQUIRED
        public static readonly string MUSIC_REMOVE_COMMAND_OPTION_DESC = "The index of the song in the playlist";
        public static readonly string MUSIC_SHUFFLE_COMMAND_NAME = "shuffle";
        public static readonly string MUSIC_SHUFFLE_COMMAND_DESC = "Shuffles the playlist";
        // MUSIC GOT BY ANILIST COMMANDS
        public static readonly string MUSIC_RLOAD_COMMAND_NAME = "rload";
        public static readonly string MUSIC_RLOAD_COMMAND_DESC = "Loads completed anime list songs from anilist username given";
        public static readonly string MUSIC_RLOAD_COMMAND_OPTION_NAME = "anilist-username"; // REQUIRED
        public static readonly string MUSIC_RLOAD_COMMAND_OPTION_DESC = "AniList username";
        public static readonly string MUSIC_RPLAY_COMMAND_NAME = "rplay";
        public static readonly string MUSIC_RPLAY_COMMAND_DESC = "Plays random songs from the list previously loaded";
        public static readonly string MUSIC_RSTOP_COMMAND_NAME = "rstop";
        public static readonly string MUSIC_RSTOP_COMMAND_DESC = "Stop playing random theme";
        public static readonly string MUSIC_RUSER_COMMAND_NAME = "ruser";
        public static readonly string MUSIC_RUSER_COMMAND_DESC = "Shows current list owner";
        // DANBOORU COMMAND
        public static readonly string DANBOORU_COMMAND_NAME = "danbooru";
        public static readonly string DANBOORU_COMMAND_DESC = "Sends a random image from danbooru with the specified tag, if no tag is given sends random image";
        public static readonly string DANBOORU_COMMAND_OPTION_NAME = "tags"; // OPTIONAL
        public static readonly string DANBOORU_COMMAND_OPTION_DESC = "Tags from danbooru";
        // SAUCENAO COMMAND
        public static readonly string SAUCENAO_COMMAND_NAME = "sauce";
        public static readonly string SAUCENAO_COMMAND_DESC = "Tries to find the source for the image given by URL";
        public static readonly string SAUCENAO_COMMAND_OPTION_NAME = "url"; // REQUIRED
        public static readonly string SAUCENAO_COMMAND_OPTION_DESC = "Source given";
        // ANILIST COMMAND
        public static readonly string ANILIST_COMMAND_NAME = "anime";
        public static readonly string ANILIST_COMMAND_DESC = "Gets the remaining time until the next episode of the specified anime";
        public static readonly string ANILIST_COMMAND_OPTION_NAME = "name"; // REQUIRED
        public static readonly string ANILIST_COMMAND_OPTION_DESC = "The name of the anime";
        // REACTION COMMANDS
        public static readonly string REACTION_YES_COMMAND_NAME = "yes";
        public static readonly string REACTION_NO_COMMAND_NAME = "no";
        public static readonly string REACTION_PRAY_COMMAND_NAME = "pray";
        public static readonly string REACTION_PLEASE_COMMAND_NAME = "please";
        public static readonly string REACTION_SMUG_COMMAND_NAME = "smug";
        public static readonly string REACTION_TREMBLING_COMMAND_NAME = "trembling";
        public static readonly string REACTION_PEKORA_COMMAND_NAME = "pekora";
        public static readonly string REACTION_HAACHAM_COMMAND_NAME = "haacham";
        public static readonly string REACTION_COMMUN_DESC = "Reaction";
        // PEKOFY COMMAND
        public static readonly string PEKOFY_COMMAND_NAME = "pekofy";
        public static readonly string PEKOFY_COMMAND_DESC = "Pekofies the text replied";
        public static readonly string PEKOFY_COMMAND_OPTION_NAME = "text";
        public static readonly string PEKOFY_COMMAND_OPTION_DESC = "The text that you wanna pekofy";

        public static readonly string HELP_CONTEXT_MSG = $@"TEST
    **• {TEST_COMMAND_NAME}:** _{TEST_COMMAND_DESC}_
HELP
    **• {HELP_COMMAND_NAME}:** _{HELP_COMMAND_DESC}_
MUSIC
    **• {MUSIC_PLAY_COMMAND_NAME} <{MUSIC_PLAY_COMMAND_OPTION_NAME}>:** _{MUSIC_PLAY_COMMAND_DESC}_
    **• {MUSIC_LIST_COMMAND_NAME}:** _{MUSIC_LIST_COMMAND_DESC}_
    **• {MUSIC_LOOP_COMMAND_NAME} <{MUSIC_LOOP_COMMAND_OPTION_NAME}>:** _{MUSIC_LOOP_COMMAND_DESC}_
    **• {MUSIC_SKIP_COMMAND_NAME} <{MUSIC_SKIP_COMMAND_OPTION_NAME}>:** _{MUSIC_SKIP_COMMAND_DESC}_
    **• {MUSIC_SONG_COMMAND_NAME}:** _{MUSIC_SONG_COMMAND_DESC}_
    **• {MUSIC_EMPTY_COMMAND_NAME}:** _{MUSIC_EMPTY_COMMAND_DESC}_
    **• {MUSIC_REMOVE_COMMAND_NAME} <{MUSIC_REMOVE_COMMAND_OPTION_NAME}>:** _{MUSIC_REMOVE_COMMAND_DESC}_
    **• {MUSIC_SHUFFLE_COMMAND_NAME}:** _{MUSIC_SHUFFLE_COMMAND_DESC}_
    **• {MUSIC_RLOAD_COMMAND_NAME} <{MUSIC_RLOAD_COMMAND_OPTION_NAME}>:** _{MUSIC_RLOAD_COMMAND_OPTION_DESC}_
    **• {MUSIC_RPLAY_COMMAND_NAME}:** _{MUSIC_RPLAY_COMMAND_DESC}_
    **• {MUSIC_RSTOP_COMMAND_NAME}:** _{MUSIC_RSTOP_COMMAND_DESC}_
    **• {MUSIC_RUSER_COMMAND_NAME}:** _{MUSIC_RUSER_COMMAND_DESC}_
DANBOORU
    **• {DANBOORU_COMMAND_NAME} <{DANBOORU_COMMAND_OPTION_NAME}>:** _{DANBOORU_COMMAND_DESC}_
SAUCE
    **• {SAUCENAO_COMMAND_NAME} <{SAUCENAO_COMMAND_OPTION_NAME}>:** _{SAUCENAO_COMMAND_DESC}_
ANILIST
    **• {ANILIST_COMMAND_NAME} <{ANILIST_COMMAND_OPTION_NAME}>:** _{ANILIST_COMMAND_DESC}_
REACTIONS
    **• {REACTION_YES_COMMAND_NAME}**
    **• {REACTION_NO_COMMAND_NAME}**
    **• {REACTION_PRAY_COMMAND_NAME}**
    **• {REACTION_SMUG_COMMAND_NAME}**
    **• {REACTION_PLEASE_COMMAND_NAME}**
    **• {REACTION_TREMBLING_COMMAND_NAME}**
    **• {REACTION_PEKORA_COMMAND_NAME}**
    **• {REACTION_HAACHAM_COMMAND_NAME}**
MISC
    **• {PEKOFY_COMMAND_NAME} <{PEKOFY_COMMAND_OPTION_NAME}>:** _{PEKOFY_COMMAND_DESC}_
";

    }
    #endregion COMMAND NAMES
}
