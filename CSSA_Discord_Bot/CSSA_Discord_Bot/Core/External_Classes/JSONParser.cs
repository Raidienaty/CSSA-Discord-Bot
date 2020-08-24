using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CSSA_Discord_Bot.Core.Data
{
    class JSONParser
    {
        public JSONParser() { }

        public CourseLevel[] courseLevelsJSONParser()
        {
            CourseLevel[] courseLevels = new CourseLevel[3];
            courseLevels[0] = JsonConvert.DeserializeObject<CourseLevel>(File.ReadAllText(@"../../../Core/Data/CS100s.JSON"));
            courseLevels[1] = JsonConvert.DeserializeObject<CourseLevel>(File.ReadAllText(@"../../../Core/Data/CS200s.JSON"));
            courseLevels[2] = JsonConvert.DeserializeObject<CourseLevel>(File.ReadAllText(@"../../../Core/Data/CS300s.JSON"));
            courseLevels[3] = JsonConvert.DeserializeObject<CourseLevel>(File.ReadAllText(@"../../../Core/Data/CS400s.JSON"));

            return courseLevels;
        }
    }
}
