using System;
using System.Collections.Generic;
using System.Text;

namespace offlineOCR
{
    class year
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public static List<year> GetYearList()
         {
            List<year> Result1 = new List<year>();
            var year = DateTime.Now.Year.ToString();
            Result1.Add(new year() { Key ="",Value="" });
            for(int p = int.Parse(year); p > 2000; p--)
            {
                Result1.Add(new year() { Key = p.ToString(),Value=p.ToString() });
            }
            return Result1;
         }
        public static List<year> GetLevelList()
        {
            List<year> Result1 = new List<year>();
            Result1.Add(new year(){Key = "",Value = ""});
            Result1.Add(new year() { Key = "本科", Value = "本科" });
            Result1.Add(new year() { Key = "专科", Value = "专科" });
            return Result1;
        }
        }
}
