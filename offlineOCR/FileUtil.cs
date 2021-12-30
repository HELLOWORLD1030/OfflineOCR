using System;
using System.IO;
using System.Text;

namespace offlineOCR
{
    public class FileUtil
    {
        public static string ReadResultFromFile(string FilePath)
        {
            try
            {
                string line;

                using (StreamReader sr = new StreamReader(FilePath,Encoding.UTF8))
                {
                   
                    line = sr.ReadLine();
                    
                }
                File.Delete(FilePath);
                return line;

            }
            catch (Exception e)
            {
                
                // 向用户显示出错消息
                return e.StackTrace;
            }

            
        }

        
    }
}