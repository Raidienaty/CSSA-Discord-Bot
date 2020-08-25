using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSSA_Discord_Bot.Core.Data;
using System.Collections.Generic;

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
        private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel socketChannel) //Message Deleted Event
        {
            var messageChannel = socketChannel as IMentionable; 
            var message = await cachedMessage.GetOrDownloadAsync();

            await logDeletedMessage(message, messageChannel);
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_ReactionAdded
        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            var messagesInChannel = originChannel.GetMessagesAsync().FlattenAsync().Result;
            var messageReactedTo = cachedMessage.DownloadAsync().Result;
            var originalPoster = messageReactedTo.Author as SocketGuildUser;
            var user = reaction.User.Value as SocketGuildUser;

            if (!(originChannel.Id == 747115357571121193)) //Checks for channel being #reaction-roles
                return;
            if (user.IsBot)
                return;
            if (!originalPoster.IsBot)
                return;

            await roleSetForReactionRole(messageReactedTo, reaction, messagesInChannel);
        }

        private async Task roleSetForReactionRole(IUserMessage messageReactedTo, SocketReaction reaction, IEnumerable<IMessage> messagesInChannel)
        {
            var user = reaction.User.Value as SocketGuildUser;
            var guild = user.Guild;
            CourseLevel[] courseLevels = GetCourseLevels();

            for (int i = 0; i < courseLevels.Length; i++)
            {
                foreach (var course in courseLevels[i].Course)
                {
                    if (checkIfMessagesContainMessage(messagesInChannel, messageReactedTo))
                    {
                        if (checkIfReactionsAreSame(reaction, course.courseEmoteUnicode))
                        {
                            if (user.Roles.Contains(guild.GetRole(course.courseRoleID)))
                                await user.RemoveRoleAsync(guild.GetRole(course.courseRoleID));
                            else
                                await user.AddRoleAsync(guild.GetRole(course.courseRoleID));

                            await messageReactedTo.RemoveReactionAsync(reaction.Emote, user);
                            return;
                        }
                    }
                }
            }
        }

        private async Task logDeletedMessage(IMessage deletedMessage, IMentionable messageChannelDeletedFrom)
        {
            var logChannel = client.GetChannel(674307924662812682) as IMessageChannel; //Reads in the log channel for posting

            EmbedBuilder builder = new EmbedBuilder() //Handles Embed building, formatted with Timestamp of original deletion 
                .WithAuthor(deletedMessage.Author)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + deletedMessage.Author.Id + " | Message ID: " + deletedMessage.Id)
                .WithDescription("**Message sent by " + deletedMessage.Author.Mention + " deleted in **" + messageChannelDeletedFrom.Mention + "\n" + deletedMessage.Content);

            var embed = builder.Build(); //Turns into actual embed for posting
            await logChannel.SendMessageAsync("", false, embed: embed); //Posting to the log channel
        }

        private bool checkIfReactionsAreSame(SocketReaction reaction, string reactionUnicode)
        {
            if (reaction.Emote.Name == reactionUnicode)
                return true;
            else
                return false;
        }

        private bool checkIfMessagesContainMessage(IEnumerable<IMessage> messagesInChannel, IUserMessage userMessage)
        {
            if (messagesInChannel.Contains(userMessage as IMessage))
                return true;
            else
                return false;
        }

        private CourseLevel[] GetCourseLevels()
        {
            var parser = new JSONParser();

            CourseLevel[] courseLevels = parser.courseLevelsJSONParser();

            return courseLevels;
        }
    }
} 
