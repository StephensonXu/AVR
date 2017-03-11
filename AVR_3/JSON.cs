using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVR_3
{
    class JSON
    {
        public String FilePath { get; set; }

        public String TcpLogName { get; set; }

        public String GpioLogName { get; set; }

        public String MainLogName { get; set; }

        public String CurrentLogName { get; set; }

        public String Temp1Logname { get; set; }

        public String Temp2Logname { get; set; }

        public String Temp3Logname { get; set; }

        public String Temp4Logname { get; set; }

        public String SerialPortName { get; set; }

        public int TcpSeverPort { get; set; }

        public int TcpSeverNum { get; set; }

        public int TransErrorCount { get; set; }

        public int GpioGroup { get; set; }

        public int SerialTime { get; set; }

        public int CpuLoopTime { get; set; }

        public int TimerInterval { get; set; }

        public int VelocityCoeffient { set; get; }
    }
}
