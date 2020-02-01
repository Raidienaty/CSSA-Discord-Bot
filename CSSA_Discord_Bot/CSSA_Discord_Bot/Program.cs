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
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }
        }
    }
}
