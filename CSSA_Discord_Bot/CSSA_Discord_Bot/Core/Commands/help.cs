using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Net;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace CSSA_Discord_Bot
{
    public class helpCommand : ModuleBase<SocketCommandContext>
    {
        [Command("help"), Alias("cmds", "HELP", "CMD"), Summary("Base help command, lists commands possible")]
        public async Task help()
        {
            EmbedBuilder build = new EmbedBuilder();

            build.WithTitle("List of Commands");

            build.AddField("help", "Lists the commands possible");

            build.WithColor(Color.Blue);

            await Context.Channel.SendMessageAsync("", false, build.Build());
        }
    }
}