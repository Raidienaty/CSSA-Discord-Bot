using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CSSA_Discord_Bot.Core.Data
{
    class JSONParser
    {
        public JSONParser() { }

        public CourseLevel[] courseLevelsJSONParser()
        {
            CourseLevel[] courseLevels = new CourseLevel[4];

            using (FileStream fileStream = File.OpenRead(@"../../../Core/Data/CS100s.JSON"))
            {
                courseLevels[0] = JsonSerializer.DeserializeAsync<CourseLevel>(fileStream).Result;
            }
            using (FileStream fileStream = File.OpenRead(@"../../../Core/Data/CS200s.JSON"))
            {
                courseLevels[1] = JsonSerializer.DeserializeAsync<CourseLevel>(fileStream).Result;
            }
            using (FileStream fileStream = File.OpenRead(@"../../../Core/Data/CS300s.JSON"))
            {
                courseLevels[2] = JsonSerializer.DeserializeAsync<CourseLevel>(fileStream).Result;
            }
            using (FileStream fileStream = File.OpenRead(@"../../../Core/Data/CS400s.JSON"))
            {
                courseLevels[3] = JsonSerializer.DeserializeAsync<CourseLevel>(fileStream).Result;
            }

            return courseLevels;
        }
    }
}
