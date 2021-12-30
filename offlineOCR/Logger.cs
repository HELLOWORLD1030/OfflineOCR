using System;
using System.Net.NetworkInformation;
using System.Windows.Controls;

namespace offlineOCR
{
    public class Logger
    {
        private TextBox Textarea;
        public Logger(TextBox InfoArea)
        {
            this.Textarea = InfoArea;
        }

        public void AddInfo(string text)
        {
            dynamic inputText = "[信息]" + " " + text;
            
            this.Textarea.Dispatcher.Invoke(new Action(() =>
            { 
                this.Textarea.AppendText(inputText);
                this.Textarea.AppendText("\n");
                this.Textarea.ScrollToEnd();
            }));
            
        }
        public void AddError(string text)
        {
            dynamic inputText = "[错误]" + " " + text;
            this.Textarea.Dispatcher.Invoke(new Action(() =>
            { 
                this.Textarea.AppendText(inputText);
                this.Textarea.AppendText("\n");
                this.Textarea.ScrollToEnd();
            }));
        }
    }
}