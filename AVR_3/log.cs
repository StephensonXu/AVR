using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AVR_3
{
    class log
    {
        private string LogPath; //= "C://log";  
        private string LogName; // = "/event.log";  

        public log(string path, string name)
        {
            LogPath = path;
            LogName = "/" + name;
        }

        public void ClearLog()
        {
            DirectoryInfo d = Directory.CreateDirectory(LogPath);
            FileStream fs = new FileStream(LogPath + LogName, System.IO.FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            sw.Close();
            fs.Close();
        }

        public void WriteLog(string log1, string log2)
        {
            try
            {
                DirectoryInfo d = Directory.CreateDirectory(LogPath);
                FileStream fs = new FileStream(LogPath + LogName, System.IO.FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine(DateTime.Now.ToString() + "\t" + log1 + "\t" + log2);
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
