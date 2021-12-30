using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Common;

namespace offlineOCR
{
    /// <summary>
    /// Cmd 的摘要说明。
    /// </summary>
    public class Cmd
    {
        
        public static string result = null;
        private JsonCon _jsonCon = null;
        private TaskCompletionSource<bool> eventHandled;
        /// <summary>
        /// 构造方法
        /// </summary>
        public Cmd()
        {
             _jsonCon = new JsonCon();
        }
        /// <summary>
        /// 执行CMD语句
        /// </summary>
        /// <param name="cmd">要执行的CMD命令</param>
        public async Task<string> RunCmd(string cmd)
        {
            JsonData json=_jsonCon.JsonToObject();
            Console.WriteLine(json.ocrpath);
            
            eventHandled = new TaskCompletionSource<bool>();
            Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "python.exe";
            p.StartInfo.Arguments =Environment.CurrentDirectory.Replace("\\","\\\\")+"\\ocr.py"+$" {cmd}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.BeginOutputReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            p.ErrorDataReceived += ErrorDel;
            p.EnableRaisingEvents = true;
            p.Exited +=new EventHandler(CmdProcess_Exited);
            await Task.WhenAny(eventHandled.Task,Task.Delay(30000));
            
            return result;
        }
         private static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Console.WriteLine(e.Data);
            if (!string.IsNullOrEmpty(e.Data))
            {
                result = e.Data;
                // Console.WriteLine(e.Data + Environment.NewLine);

            }
        }
         public void ErrorDel(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            // 执行结束后触发
            
        }

        
        
        
    }
}