using System;
using System.IO;
using System.Text;

namespace AVR_3
{
    internal class log
    {
        private readonly string LogName; // = "/event.log";  
        private readonly string LogPath; //= "C://log";  

        public log(string path, string name)
        {
            LogPath = path;
            LogName = "/" + name;
        }

        public void ClearLog()
        {
            try
            {
                var d = Directory.CreateDirectory(LogPath);
                var fs = new FileStream(LogPath + LogName, FileMode.Create);
                var sw = new StreamWriter(fs, Encoding.Default);
                sw.Close();
                fs.Close();
            }
            catch
            {
                //nothing to do
            }
        }

        public void WriteLog(string log1, string log2)
        {
            try
            {
                var d = Directory.CreateDirectory(LogPath);
                var fs = new FileStream(LogPath + LogName, FileMode.Append);
                var sw = new StreamWriter(fs, Encoding.Default);
                sw.WriteLine(DateTime.Now + "\t" + log1 + "\t" + log2);
                sw.Close();
                fs.Close();
            }
            catch
            {
                //    // Nothing to do  
            }
        }
    }
}