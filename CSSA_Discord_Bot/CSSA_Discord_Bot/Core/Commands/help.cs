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
            string[] commandNames = { "" };

            EmbedBuilder build = new EmbedBuilder();

            build.WithTitle("List of Commands");

            /*Console.WriteLine("Before foreach");
            foreach (var item in getCommandNames())
            {
                build.AddField(item, "");
            }*/

            build.AddField("help", "Lists the commands possible");

            build.WithColor(Color.Blue);

            await Context.Channel.SendMessageAsync("", false, build.Build());
        }

        private string[] getCommandNames()
        {
            return Directory.GetFiles(@"../Commands", "*.cs").Select(Path.GetFileName).ToArray();
        }
    }
}