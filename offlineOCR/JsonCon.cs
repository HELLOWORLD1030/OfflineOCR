using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace offlineOCR
{
    public class JsonCon
    {
        public JsonData JsonToObject()
        {
            string myStr;
            string file = Environment.CurrentDirectory + @"\MainConfig.json";
            using (FileStream fsRead = new FileStream(file, FileMode.Open)) 
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);
                myStr = System.Text.Encoding.UTF8.GetString(heByte);
                
            } 
            JsonData rb1 = JsonConvert.DeserializeObject<JsonData>(myStr);
            // Console.Write(rb1);
            return rb1;
        }
    }
    public class JsonData
    {
        public string ocrpath { get; set; }
        public string dbname { get; set; }
        public string dbpass { get; set; }
        public string db { get; set; }
       
    }
}