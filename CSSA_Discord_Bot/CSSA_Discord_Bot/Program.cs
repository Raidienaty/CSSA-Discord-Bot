using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSSA_Discord_Bot.Core.Data;

namespace CSSA_Discord_Bot
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService Commands;

        public object Message { get; private set; }

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            PullFromFile keyPull = new PullFromFile();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 20,
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            //Events
            client.Ready += Client_Ready;
            client.Log += Client_Log;
            client.MessageUpdated += MessageUpdate;
            client.MessageDeleted += MessageDeleted;
            client.ReactionAdded += ReactionAdded;

            await client.LoginAsync(TokenType.Bot, keyPull.FilePull(@"D:/Storage/Keys/CSSADiscordBotKey.txt"));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message} ");
        }

        private async Task Client_Ready()
        {
            await client.SetGameAsync("Helping people");
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            if (Message == null)
                return;
            var Context = new SocketCommandContext(client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;
            if (!(Message.HasCharPrefix('?', ref ArgPos) || Message.HasMentionPrefix(client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

            if (!Result.IsSuccess)
            {
                Console.WriteLine($"[{DateTime.Now} at Commands] Something went wrong with executing a command Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_MessageUpdated
        private async Task MessageUpdate(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel socketChannel) //Message Update Handler
        {
            if (before.DownloadAsync().Result.Embeds.Count > 0)
                return;
            if (after.Embeds.Count > 0)
                return;
            var _logChannel = client.GetChannel(674307924662812682) as IMessageChannel; //Reads log channel for posting
            var message = await before.GetOrDownloadAsync(); //Reads message prior to the change which is stored in cache

            EmbedBuilder builder = new EmbedBuilder() //Handles Embed building, formatted with Timestamp of update to message 
                .WithAuthor(message.Author)
                .WithColor(Color.Blue)
                .AddField("Before", message)
                .AddField("After", after)
                .AddField("\u200B", "\n" + message.GetJumpUrl())
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + message.Author.Id + " | Message ID: " + message.Id);
            
            var embed = builder.Build(); //Turns Embed builder to an Embed ready for posting
            await _logChannel.SendMessageAsync("", false, embed: embed); //Posting Embed to log channel
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_MessageDeleted
        private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel socketChannel) //Message Deleted Handler
        {
            var _messageChannel = socketChannel as IMentionable; //Reads the socketChannel as a mentionable channel
            var _logChannel = client.GetChannel(674307924662812682) as IMessageChannel; //Reads in the log channel for posting
            var message = await cachedMessage.GetOrDownloadAsync(); //Reads the message in the cache (deleted message)

            EmbedBuilder builder = new EmbedBuilder() //Handles Embed building, formatted with Timestamp of original deletion 
                .WithAuthor(message.Author)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + message.Author.Id + " | Message ID: " + message.Id)
                .WithDescription("**Message sent by " + message.Author.Mention + " deleted in **" + _messageChannel.Mention + "\n" + message.Content);
            
            var embed = builder.Build(); //Turns into actual embed for posting
            await _logChannel.SendMessageAsync("", false, embed: embed); //Posting to the log channel
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_ReactionAdded
        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            var messages = originChannel.GetMessagesAsync().FlattenAsync();
            var postedUser = cachedMessage.DownloadAsync().Result.Author as SocketGuildUser;
            var user = reaction.User.Value as SocketGuildUser;
            if (!(originChannel.Id == 747115357571121193))
                return;
            if (user.IsBot)
                return;
            if (!postedUser.IsBot)
                return;

            Console.WriteLine("Reaction added!");
            
            var userReactions = await cachedMessage.DownloadAsync().Result.GetReactionUsersAsync(reaction.Emote, 100).FlattenAsync();
            var guild = user.Guild;

            Console.WriteLine(messages.Result.ToArray().Length + " " + cachedMessage.DownloadAsync().Result.Id);

            CourseLevel[] courseLevels = GetCourseLevels();

            for (int i = 0; i < courseLevels.Length; i++)
            {
                foreach (var course in courseLevels[i].Course)
                {
                    if (messages.Result.ToArray()[courseLevels.Length - 1 - i].Id == cachedMessage.DownloadAsync().Result.Id)
                    {
                        if (reaction.Emote.Name == course.courseEmoteUnicode)
                        {
                            if (user.Roles.Contains(guild.GetRole(course.courseRoleID)))
                                await user.RemoveRoleAsync(guild.GetRole(course.courseRoleID));
                            else
                                await user.AddRoleAsync(guild.GetRole(course.courseRoleID));

                            await cachedMessage.DownloadAsync().Result.RemoveReactionAsync(reaction.Emote, user);
                            return;
                        }
                    }
                }
            }
        }

        private CourseLevel[] GetCourseLevels()
        {
            var parser = new JSONParser();

            CourseLevel[] courseLevels = parser.courseLevelsJSONParser();

            return courseLevels;
        }
    }
} 
