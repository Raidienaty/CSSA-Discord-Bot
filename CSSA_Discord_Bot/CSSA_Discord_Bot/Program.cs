using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

            if (messages.Result.ToArray()[4].Id == cachedMessage.DownloadAsync().Result.Id)
            {
                if (reaction.Emote.Name == "\u0031\u20E3") //110
                {
                    if (user.Roles.Contains(guild.GetRole(747137070169849987)))
                        await user.RemoveRoleAsync(guild.GetRole(747137070169849987));
                    else
                        await user.AddRoleAsync(guild.GetRole(747137070169849987));
                }
                else if (reaction.Emote.Name == "\u0032\u20E3") //113
                {
                    if (user.Roles.Contains(guild.GetRole(747137158707544135)))
                        await user.RemoveRoleAsync(guild.GetRole(747137158707544135));
                    else
                        await user.AddRoleAsync(guild.GetRole(747137158707544135));
                }
                else if (reaction.Emote.Name == "\u0033\u20E3") //114
                {
                    if (user.Roles.Contains(guild.GetRole(747137233647173672)))
                        await user.RemoveRoleAsync(guild.GetRole(747137233647173672));
                    else
                        await user.AddRoleAsync(guild.GetRole(747137233647173672));
                }

            }
            else if (messages.Result.ToArray()[3].Id == cachedMessage.DownloadAsync().Result.Id)
            {
                if (reaction.Emote.Name == "\u0031\u20E3") //203
                {
                    if (user.Roles.Contains(guild.GetRole(747138267862007889)))
                        await user.RemoveRoleAsync(guild.GetRole(747138267862007889));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138267862007889));
                }
                else if (reaction.Emote.Name == "\u0032\u20E3") //204
                {
                    if (user.Roles.Contains(guild.GetRole(747138342877003847)))
                        await user.RemoveRoleAsync(guild.GetRole(747138342877003847));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138342877003847));
                }
                else if (reaction.Emote.Name == "\u0033\u20E3") //217
                {
                    if (user.Roles.Contains(guild.GetRole(747138368709722303)))
                        await user.RemoveRoleAsync(guild.GetRole(747138368709722303));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138368709722303));
                }
                else if (reaction.Emote.Name == "\u0034\u20E3") //218
                {
                    if (user.Roles.Contains(guild.GetRole(747138411747475497)))
                        await user.RemoveRoleAsync(guild.GetRole(747138411747475497));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138411747475497));
                }
                else if (reaction.Emote.Name == "\u0035\u20E3") //219
                {
                    if (user.Roles.Contains(guild.GetRole(747138434765815979)))
                        await user.RemoveRoleAsync(guild.GetRole(747138434765815979));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138434765815979));
                }
                else if (reaction.Emote.Name == "\u0036\u20E3") //231
                {
                    if (user.Roles.Contains(guild.GetRole(747138462540365894)))
                        await user.RemoveRoleAsync(guild.GetRole(747138462540365894));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138462540365894));
                }
                else if (reaction.Emote.Name == "\u0037\u20E3") //238
                {
                    if (user.Roles.Contains(guild.GetRole(747138481641488500)))
                        await user.RemoveRoleAsync(guild.GetRole(747138481641488500));
                    else
                        await user.AddRoleAsync(guild.GetRole(747138481641488500));
                }
            }
            else if (messages.Result.ToArray()[2].Id == cachedMessage.DownloadAsync().Result.Id)
            {
                if (reaction.Emote.Name == "\u0031\u20E3") //303
                {
                    if (user.Roles.Contains(guild.GetRole(747142417173446737)))
                        await user.RemoveRoleAsync(guild.GetRole(747142417173446737));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142417173446737));
                }
                else if (reaction.Emote.Name == "\u0032\u20E3") //304
                {
                    if (user.Roles.Contains(guild.GetRole(747142452988739615)))
                        await user.RemoveRoleAsync(guild.GetRole(747142452988739615));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142452988739615));
                }
                else if (reaction.Emote.Name == "\u0033\u20E3") //312
                {
                    if (user.Roles.Contains(guild.GetRole(747142474228564070)))
                        await user.RemoveRoleAsync(guild.GetRole(747142474228564070));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142474228564070));
                }
                else if (reaction.Emote.Name == "\u0034\u20E3") //317
                {
                    if (user.Roles.Contains(guild.GetRole(747142496869416972)))
                        await user.RemoveRoleAsync(guild.GetRole(747142496869416972));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142496869416972));
                }
                else if (reaction.Emote.Name == "\u0035\u20E3") //321
                {
                    if (user.Roles.Contains(guild.GetRole(747142517073641615)))
                        await user.RemoveRoleAsync(guild.GetRole(747142517073641615));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142517073641615));
                }
                else if (reaction.Emote.Name == "\u0036\u20E3") //328
                {
                    if (user.Roles.Contains(guild.GetRole(747142539206983780)))
                        await user.RemoveRoleAsync(guild.GetRole(747142539206983780));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142539206983780));
                }
                else if (reaction.Emote.Name == "\u0037\u20E3") //331
                {
                    if (user.Roles.Contains(guild.GetRole(747142573595820133)))
                        await user.RemoveRoleAsync(guild.GetRole(747142573595820133));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142573595820133));
                }
                else if (reaction.Emote.Name == "\u0038\u20E3") //361
                {
                    if (user.Roles.Contains(guild.GetRole(747142589752541224)))
                        await user.RemoveRoleAsync(guild.GetRole(747142589752541224));
                    else
                        await user.AddRoleAsync(guild.GetRole(747142589752541224));
                }
            }
            else if (messages.Result.ToArray()[1].Id == cachedMessage.DownloadAsync().Result.Id)
            {
                if (reaction.Emote.Name == "\u0031\u20E3") //407
                {
                    if (user.Roles.Contains(guild.GetRole(739995044575969300)))
                        await user.RemoveRoleAsync(guild.GetRole(739995044575969300));
                    else
                        await user.AddRoleAsync(guild.GetRole(739995044575969300));
                }
                else if (reaction.Emote.Name == "\u0032\u20E3") //411
                {
                    if (user.Roles.Contains(guild.GetRole(747144264282013992)))
                        await user.RemoveRoleAsync(guild.GetRole(747144264282013992));
                    else
                        await user.AddRoleAsync(guild.GetRole(747144264282013992));
                }
                else if (reaction.Emote.Name == "\u0033\u20E3") //413
                {
                    if (user.Roles.Contains(guild.GetRole(747144286109302854)))
                        await user.RemoveRoleAsync(guild.GetRole(747144286109302854));
                    else
                        await user.AddRoleAsync(guild.GetRole(747144286109302854));
                }
                else if (reaction.Emote.Name == "\u0034\u20E3") //414
                {
                    if (user.Roles.Contains(guild.GetRole(747144359455096935)))
                        await user.RemoveRoleAsync(guild.GetRole(747144359455096935));
                    else
                        await user.AddRoleAsync(guild.GetRole(747144359455096935));
                }
                else if (reaction.Emote.Name == "\u0035\u20E3") //425
                {
                    if (user.Roles.Contains(guild.GetRole(747144391306772601)))
                        await user.RemoveRoleAsync(guild.GetRole(747144391306772601));
                    else
                        await user.AddRoleAsync(guild.GetRole(747144391306772601));
                }
            }

            await cachedMessage.DownloadAsync().Result.RemoveReactionAsync(reaction.Emote, user);
        }
    }
} 
