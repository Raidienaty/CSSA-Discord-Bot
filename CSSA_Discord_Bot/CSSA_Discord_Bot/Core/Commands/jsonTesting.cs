using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using CSSA_Discord_Bot.Core.Data;

namespace CSSA_Discord_Bot.Core.Commands
{
    public class jsonTesting : ModuleBase<SocketCommandContext>
    {
        [Command("jsonTesting"), Summary("json testing")]
        public async Task jsonTest()
        {
            await Context.Channel.SendMessageAsync("Working...");

            JSONParser jsonParser = new JSONParser();

            var courseLevels = jsonParser.courseLevelsJSONParser();

            var cs100s = courseLevels[0];
            var cs200s = courseLevels[1];

            await Context.Channel.SendMessageAsync(cs100s.courseLevelName);
            await Context.Channel.SendMessageAsync(cs100s.Course[0].courseRoleID.ToString());
        }
    }
}
