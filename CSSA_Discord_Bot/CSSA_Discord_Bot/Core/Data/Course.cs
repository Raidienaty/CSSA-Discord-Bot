using System;
using System.Collections.Generic;
using System.Text;

namespace CSSA_Discord_Bot.Core.Data
{
    class Course
    {
        public string courseID { get; set; }
        public string courseName { get; set; }
        public string courseEmoteUnicode { get; set; }
        public ulong courseRoleID { get; set; }
    }
}
