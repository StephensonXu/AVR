using System;
using System.Diagnostics;

namespace AVR_3
{
    internal class Video
    {
        private readonly Process p = new Process();

        #region GpioLog

        private readonly log videoLog = new log("D://log", "/video.log");

        #endregion

        public void OpenVideo()
        {
            p.StartInfo.FileName = "ffmpeg.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments =
                "-f dshow -i video=\"USB2.0 Camera\" -vcodec mpeg2video -f mpeg2video udp://192.168.1.104:8888";
                //此处后面需要更改为上位机IP
            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                videoLog.WriteLog("打开ffmpeg失败", e.ToString());
            }
        }

        public void CloseVideo()
        {
            try
            {
                p.Kill();
                //p.CloseMainWindow();
            }
            catch (Exception e)
            {
                videoLog.WriteLog("关闭ffmpeg失败", e.ToString());
            }
        }
    }
}