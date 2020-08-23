using Discord;
using Discord.Commands;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSSA_Discord_Bot.Core.Commands
{
    public class ReactionClass : ModuleBase<SocketCommandContext>
    {
        [Command("reactionClassPost"), Summary("This function posts all the classes with their reaction roles.")]
        public async Task postReaction()
        {
            DataTable CS100s = new DataTable(); //Classes Data
            DataTable CS200s = new DataTable();
            DataTable CS300s = new DataTable();
            DataTable CS400s = new DataTable();

            DataColumn newColumn;
            DataRow newRow;

            /* classID className reactionEmoji
            1
            2
            .
            .
            .
            N

            */

            #region CS100s Table Setup
            //Column 1
            newColumn = new DataColumn();
            newColumn.ColumnName = "classID";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS100s.Columns.Add(newColumn);

            //Column 2
            newColumn = new DataColumn();
            newColumn.ColumnName = "className";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS100s.Columns.Add(newColumn);

            //Column 3
            newColumn = new DataColumn();
            newColumn.ColumnName = "reactionEmoji";
            newColumn.ReadOnly = true;
            newColumn.Unique = false;

            CS100s.Columns.Add(newColumn);
            #endregion

            #region CS200s Table Setup
            newColumn = new DataColumn();
            newColumn.ColumnName = "classID";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS200s.Columns.Add(newColumn);

            //Column 2
            newColumn = new DataColumn();
            newColumn.ColumnName = "className";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS200s.Columns.Add(newColumn);

            //Column 3
            newColumn = new DataColumn();
            newColumn.ColumnName = "reactionEmoji";
            newColumn.ReadOnly = true;
            newColumn.Unique = false;

            CS200s.Columns.Add(newColumn);
            #endregion

            #region CS300s Table Setup
            newColumn = new DataColumn();
            newColumn.ColumnName = "classID";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS300s.Columns.Add(newColumn);

            //Column 2
            newColumn = new DataColumn();
            newColumn.ColumnName = "className";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS300s.Columns.Add(newColumn);

            //Column 3
            newColumn = new DataColumn();
            newColumn.ColumnName = "reactionEmoji";
            newColumn.ReadOnly = true;
            newColumn.Unique = false;

            CS300s.Columns.Add(newColumn);
            #endregion

            #region CS400s Table Setup
            newColumn = new DataColumn();
            newColumn.ColumnName = "classID";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS400s.Columns.Add(newColumn);

            //Column 2
            newColumn = new DataColumn();
            newColumn.ColumnName = "className";
            newColumn.ReadOnly = true;
            newColumn.Unique = true;

            CS400s.Columns.Add(newColumn);

            //Column 3
            newColumn = new DataColumn();
            newColumn.ColumnName = "reactionEmoji";
            newColumn.ReadOnly = true;
            newColumn.Unique = false;

            CS400s.Columns.Add(newColumn);
            #endregion

            //Rows
            #region CS100s Data
            newRow = CS100s.NewRow();
            newRow["classID"] = "CS110";
            newRow["className"] = "Fundamentals of Programming";
            newRow["reactionEmoji"] = "\u0031\u20E3";
            CS100s.Rows.Add(newRow);

            newRow = CS100s.NewRow();
            newRow["classID"] = "CS113";
            newRow["className"] = "Introduction to Programming";
            newRow["reactionEmoji"] = "\u0032\u20E3";
            CS100s.Rows.Add(newRow);

            newRow = CS100s.NewRow();
            newRow["classID"] = "CS114";
            newRow["className"] = "Introduction to Software Engineering";
            newRow["reactionEmoji"] = "\u0033\u20E3";
            CS100s.Rows.Add(newRow);
            #endregion

            #region CS200s Data
            newRow = CS200s.NewRow();
            newRow["classID"] = "CS203";
            newRow["className"] = "Sophomore Software Engineering 1";
            newRow["reactionEmoji"] = "\u0031\u20E3";
            CS200s.Rows.Add(newRow);

            newRow = CS200s.NewRow();
            newRow["classID"] = "CS204";
            newRow["className"] = "Sophomore Software Engineering 2";
            newRow["reactionEmoji"] = "\u0032\u20E3";
            CS200s.Rows.Add(newRow);

            newRow = CS200s.NewRow();
            newRow["classID"] = "CS217";
            newRow["className"] = "Object Oriented Programming";
            newRow["reactionEmoji"] = "\u0033\u20E3";
            CS200s.Rows.Add(newRow);

            newRow = CS200s.NewRow();
            newRow["classID"] = "CS218";
            newRow["className"] = "Data Structures and Algorithms";
            newRow["reactionEmoji"] = "\u0034\u20E3";
            CS200s.Rows.Add(newRow);

            newRow = CS200s.NewRow();
            newRow["classID"] = "CS219";
            newRow["className"] = "Computer Architecture";
            newRow["reactionEmoji"] = "\u0035\u20E3";
            CS200s.Rows.Add(newRow);

            newRow = CS200s.NewRow();
            newRow["classID"] = "CS231";
            newRow["className"] = "Database Systems";
            newRow["reactionEmoji"] = "\u0036\u20E3";
            CS200s.Rows.Add(newRow);

            newRow = CS200s.NewRow();
            newRow["classID"] = "CS238";
            newRow["className"] = "UNIX Programming";
            newRow["reactionEmoji"] = "\u0037\u20E3";
            CS200s.Rows.Add(newRow);
            #endregion

            #region CS300s Data
            newRow = CS300s.NewRow();
            newRow["classID"] = "CS303";
            newRow["className"] = "Junior Software Engineering 1";
            newRow["reactionEmoji"] = "\u0031\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS304";
            newRow["className"] = "Junior Software Engineering 2";
            newRow["reactionEmoji"] = "\u0032\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS312";
            newRow["className"] = "Analysis of Algorithms";
            newRow["reactionEmoji"] = "\u0033\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS317";
            newRow["className"] = "Computer Networks";
            newRow["reactionEmoji"] = "\u0034\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS321";
            newRow["className"] = "Programming Language Concepts";
            newRow["reactionEmoji"] = "\u0035\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS328";
            newRow["className"] = "Embedded Systems";
            newRow["reactionEmoji"] = "\u0036\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS331";
            newRow["className"] = "Computer Security";
            newRow["reactionEmoji"] = "\u0037\u20E3";
            CS300s.Rows.Add(newRow);

            newRow = CS300s.NewRow();
            newRow["classID"] = "CS361";
            newRow["className"] = "Computer Software and Operating Systems";
            newRow["reactionEmoji"] = "\u0038\u20E3";
            CS300s.Rows.Add(newRow);
            #endregion

            #region CS400s Data
            newRow = CS400s.NewRow();
            newRow["classID"] = "CS407";
            newRow["className"] = "Principles of Machine Learning";
            newRow["reactionEmoji"] = "\u0031\u20E3";
            CS400s.Rows.Add(newRow);

            newRow = CS400s.NewRow();
            newRow["classID"] = "CS411";
            newRow["className"] = "Artificial Intelligence";
            newRow["reactionEmoji"] = "\u0032\u20E3";
            CS400s.Rows.Add(newRow);

            newRow = CS400s.NewRow();
            newRow["classID"] = "CS413";
            newRow["className"] = "Software Engineering Project I";
            newRow["reactionEmoji"] = "\u0033\u20E3";
            CS400s.Rows.Add(newRow);

            newRow = CS400s.NewRow();
            newRow["classID"] = "CS414";
            newRow["className"] = "Software Engineering Project II";
            newRow["reactionEmoji"] = "\u0034\u20E3";
            CS400s.Rows.Add(newRow);

            newRow = CS400s.NewRow();
            newRow["classID"] = "CS425";
            newRow["className"] = "Systems Architecture";
            newRow["reactionEmoji"] = "\u0035\u20E3";
            CS400s.Rows.Add(newRow);
            #endregion

            #region CS100 Level Courses Embed
            EmbedBuilder CS100Level = new EmbedBuilder();
            CS100Level.Title = "CS100 Level Courses";

            for (int i = 0; i < CS100s.Rows.Count; i++)
            {
                CS100Level.Description += CS100s.Rows[i].ItemArray[2].ToString() + " ";
                for (int j = 0; j < 2; j++)
                {
                    CS100Level.Description += CS100s.Rows[i].ItemArray[j].ToString() + " ";
                }
                CS100Level.Description += "\n";
            }
            #endregion

            #region CS200 Level Courses Embed
            EmbedBuilder CS200Level = new EmbedBuilder();
            CS200Level.Title = "CS200 Level Courses";

            for (int i = 0; i < CS200s.Rows.Count; i++)
            {
                CS200Level.Description += CS200s.Rows[i].ItemArray[2].ToString() + " ";
                for (int j = 0; j < 2; j++)
                {
                    CS200Level.Description += CS200s.Rows[i].ItemArray[j].ToString() + " ";
                }
                CS200Level.Description += "\n";
            }
            #endregion

            #region CS300 Level Courses Embed
            EmbedBuilder CS300Level = new EmbedBuilder();
            CS300Level.Title = "CS300 Level Courses";

            for (int i = 0; i < CS300s.Rows.Count; i++)
            {
                CS300Level.Description += CS300s.Rows[i].ItemArray[2].ToString() + " ";
                for (int j = 0; j < 2; j++)
                {
                    CS300Level.Description += CS300s.Rows[i].ItemArray[j].ToString() + " ";
                }
                CS300Level.Description += "\n";
            }
            #endregion

            #region CS400 Level Courses Embed
            EmbedBuilder CS400Level = new EmbedBuilder();
            CS400Level.Title = "CS400 Level Courses";

            for (int i = 0; i < CS400s.Rows.Count; i++)
            {
                CS400Level.Description += CS400s.Rows[i].ItemArray[2].ToString() + " ";
                for (int j = 0; j < 2; j++)
                {
                    CS400Level.Description += CS400s.Rows[i].ItemArray[j].ToString() + " ";
                }
                CS400Level.Description += "\n";
            }
            #endregion

            var cs100Message = Context.Channel.SendMessageAsync("", false, CS100Level.Build()).Result;
            var cs200Message = Context.Channel.SendMessageAsync("", false, CS200Level.Build()).Result;
            var cs300Message = Context.Channel.SendMessageAsync("", false, CS300Level.Build()).Result;
            var cs400Message = Context.Channel.SendMessageAsync("", false, CS400Level.Build()).Result;

            //Console.WriteLine();new Emoji("\U00000031")
            for (int i = 0; i < CS100s.Rows.Count; i++)
            {
                await cs100Message.AddReactionAsync(new Emoji(CS100s.Rows[i].ItemArray[2].ToString()));
            }
            for (int i = 0; i < CS200s.Rows.Count; i++)
            {
                await cs200Message.AddReactionAsync(new Emoji(CS200s.Rows[i].ItemArray[2].ToString()));
            }
            for (int i = 0; i < CS300s.Rows.Count; i++)
            {
                await cs300Message.AddReactionAsync(new Emoji(CS300s.Rows[i].ItemArray[2].ToString()));
            }
            for (int i = 0; i < CS400s.Rows.Count; i++)
            {
                await cs400Message.AddReactionAsync(new Emoji(CS400s.Rows[i].ItemArray[2].ToString()));
            }

            await Context.Channel.SendMessageAsync("Posted!");
        }
    }
}
