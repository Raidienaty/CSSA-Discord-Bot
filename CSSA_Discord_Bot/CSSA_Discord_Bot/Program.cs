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

            //Events
            client.Ready += ClientReady;
            client.Log += ClientLog;
            client.MessageReceived += MessageReceived;
            client.MessageUpdated += MessageUpdate;
            client.MessageDeleted += MessageDeleted;
            client.ReactionAdded += ReactionAdded;
            
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            await client.LoginAsync(TokenType.Bot, keyPull.FilePull(@"D:/Storage/Keys/CSSADiscordBotKey.txt"));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ClientLog(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message} ");
        }

        private async Task ClientReady()
        {
            await client.SetGameAsync("Helping people");
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_MessageReceived
        private async Task MessageReceived(SocketMessage MessageParam)
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
        private async Task MessageUpdate(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel socketChannel) //Message Update Event
        {
            var beforeMessage = before.DownloadAsync().Result;

            if (checkForEmbeds(beforeMessage, after))
                return;

            await logUpdatedMessage(beforeMessage, after, socketChannel);
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_MessageDeleted
        private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel socketChannel) //Message Deleted Event
        {
            var messageChannel = socketChannel as IMentionable; 
            var message = await cachedMessage.GetOrDownloadAsync();

            if (checkForEmbeds(message))
                return;

            await logDeletedMessage(message, messageChannel);
        }

        //https://docs.stillu.cc/api/Discord.WebSocket.BaseSocketClient.html#Discord_WebSocket_BaseSocketClient_ReactionAdded
        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            var messagesInChannel = originChannel.GetMessagesAsync().FlattenAsync().Result;
            var messageReactedTo = cachedMessage.DownloadAsync().Result;
            var originalPoster = messageReactedTo.Author as SocketGuildUser;
            var user = reaction.User.Value as SocketGuildUser;

            Console.WriteLine("Reaction Added");

            if (user.IsBot)
                return;
            if (!originalPoster.IsBot)
                return;
            if (!(originChannel.Id == 752925552842637383)) //Checks for channel being #reaction-roles
                return;

            Console.WriteLine("Reaction Role channel");

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
                    if (checkIfMessagesContainMessage(messagesInChannel, messageReactedTo, i))
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
            var logChannel = client.GetChannel(674307924662812682) as IMessageChannel;

            EmbedBuilder builder = new EmbedBuilder() 
                .WithAuthor(deletedMessage.Author)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + deletedMessage.Author.Id + " | Message ID: " + deletedMessage.Id)
                .WithDescription("**Message sent by " + deletedMessage.Author.Mention + " deleted in **" + messageChannelDeletedFrom.Mention + "\n" + deletedMessage.Content);

            var embed = builder.Build(); 
            await logChannel.SendMessageAsync("", false, embed: embed); 
        }

        private async Task logUpdatedMessage(IMessage beforeMessage, SocketMessage afterMessage, ISocketMessageChannel socketChannel)
        {
            var _logChannel = client.GetChannel(674307924662812682) as IMessageChannel;

            EmbedBuilder builder = new EmbedBuilder() 
                .WithAuthor(beforeMessage.Author)
                .WithColor(Color.Blue)
                .AddField("Before", beforeMessage)
                .AddField("After", afterMessage)
                .AddField("\u200B", "\n" + beforeMessage.GetJumpUrl())
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + beforeMessage.Author.Id + " | Message ID: " + beforeMessage.Id);

            var embed = builder.Build();
            await _logChannel.SendMessageAsync("", false, embed: embed);
        }

        private bool checkForEmbeds(IMessage message)
        {
            if (message == null)
                return true;
            if (message.Embeds.Count > 0)
                return true;
            else
                return false;
        }

        private bool checkForEmbeds(IMessage message, SocketMessage secondMessage)
        {
            if (message.Embeds.Count > 0)
                return true;
            else if (secondMessage.Embeds.Count > 0)
                return true;
            else
                return false;
        }

        private bool checkIfReactionsAreSame(SocketReaction reaction, string reactionUnicode)
        {
            if (reaction.Emote.Name == reactionUnicode)
                return true;
            else
                return false;
        }

        private bool checkIfMessagesContainMessage(IEnumerable<IMessage> messagesInChannel, IUserMessage userMessage, int spotInArray)
        {
            CourseLevel[] courseLevels = GetCourseLevels();

            if (messagesInChannel.ToArray()[courseLevels.Length - 1 - spotInArray].Id == userMessage.Id)
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
