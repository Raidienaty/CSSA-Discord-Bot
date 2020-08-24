using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using CSSA_Discord_Bot.Core.Data;

namespace CSSA_Discord_Bot.Core.Commands
{
    public class ReactionRole : ModuleBase<SocketCommandContext>
    {
        [Command("courseReactionRoles"), Summary("Posts the reaction role embeds")]
        public async Task courseReactionRoles()
        {
            Embed[] courseEmbeds = createEmbeds();

            foreach (var embed in courseEmbeds)
            {
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }

        private Embed[] createEmbeds()
        {
            var courseLevels = GetCourseLevels();

            var cs100sCourseLevel = courseLevels[0];
            var cs200sCourseLevel = courseLevels[1];
            var cs300sCourseLevel = courseLevels[2];
            var cs400sCourseLevel = courseLevels[3];

            var CS100sEmbed = createEmbedFromCourseLevel(cs100sCourseLevel);
            var CS200sEmbed = createEmbedFromCourseLevel(cs200sCourseLevel);
            var CS300sEmbed = createEmbedFromCourseLevel(cs300sCourseLevel);
            var CS400sEmbed = createEmbedFromCourseLevel(cs400sCourseLevel);

            Embed[] courseEmbeds = { CS100sEmbed, CS200sEmbed, CS300sEmbed, CS400sEmbed };

            return courseEmbeds;
        }

        private Embed createEmbedFromCourseLevel(CourseLevel courseLevel)
        {
            var embedBuilder = new EmbedBuilder();

            embedBuilder.Title = courseLevel.courseLevelName + " Course Levels";

            foreach (var course in courseLevel.Course)
            {
                embedBuilder.Description += course.courseEmoteUnicode + " " + course.courseID + " " + course.courseName + "\n";
            }

            return embedBuilder.Build();
        }

        private CourseLevel[] GetCourseLevels()
        {
            var parser = new JSONParser();

            CourseLevel[] courseLevels = parser.courseLevelsJSONParser();

            return courseLevels;
        }
    }
}
