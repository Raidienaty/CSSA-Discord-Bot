using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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

            client.Ready += Client_Ready;
            client.Log += Client_Log;
            client.MessageUpdated += MessageUpdate;
            client.MessageDeleted += MessageDeleted;

            await client.LoginAsync(TokenType.Bot, keyPull.FilePull(@"F:/Storage/CSSA-Key.txt"));
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

        private async Task MessageUpdate(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel socketChannel) //Message Update Handler
        {
            var _logChannel = client.GetChannel(674027105167868003) as IMessageChannel; //Reads log channel for posting
            var message = await before.GetOrDownloadAsync(); //Reads message prior to the change which is stored in cache

            EmbedBuilder builder = new EmbedBuilder() //Handles Embed building, formatted with Timestamp of update to message 
                .WithAuthor(message.Author)
                .WithColor(Color.Blue)
                .AddField("Before", message)
                .AddField("After", after)
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + message.Author.Id + " | Message ID: " + message.Id);
            
            var embed = builder.Build(); //Turns Embed builder to an Embed ready for posting
            await _logChannel.SendMessageAsync("", false, embed: embed); //Posting Embed to log channel
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel socketChannel) //Message Deleted Handler
        {
            var _messageChannel = socketChannel as IMentionable; //Reads the socketChannel as a mentionable channel
            var _logChannel = client.GetChannel(674027105167868003) as IMessageChannel; //Reads in the log channel for posting
            var message = await cachedMessage.GetOrDownloadAsync(); //Reads the message in the cache (deleted message)

            EmbedBuilder builder = new EmbedBuilder() //Handles Embed building, formatted with Timestamp of original deletion 
                .WithAuthor(message.Author)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter("Author ID: " + message.Author.Id + " | Message ID: " + message.Id);
                builder.WithDescription("**Message sent by " + message.Author.Mention + " deleted in **" + _messageChannel.Mention + "\n" + message.Content);
            var embed = builder.Build(); //Turns into actual embed for posting
            await _logChannel.SendMessageAsync("", false, embed: embed); //Posting to the log channel
        }
    }
} 
