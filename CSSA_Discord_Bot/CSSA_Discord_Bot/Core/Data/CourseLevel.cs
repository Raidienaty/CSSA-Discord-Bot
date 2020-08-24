using System;
using System.Collections.Generic;
using System.Text;

namespace CSSA_Discord_Bot.Core.Data
{
    class CourseLevel
    {
        public string courseLevelName { get; set; }
        public List<Course> Course { get; set; }
    }
}
