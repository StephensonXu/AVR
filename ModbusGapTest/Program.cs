using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ModbusGapTest
{
    class Program
    {
        static SerialPort com = new SerialPort("COM2", 9600, Parity.Even, 8, StopBits.One);
        static void Main(string[] args)
        {           
            //注册串口接收方法
            com.NewLine = "\r\n";
            com.DataReceived += com_DataReceived;
            com.ReadTimeout = 5;
            //打开串口
            if (!com.IsOpen)
            {
                try
                {
                    com.Open();
                    Console.WriteLine("打开串口成功");
                }
                catch (Exception e)
                {
                    com = new SerialPort();
                    Console.WriteLine("打开串口失败" + e.ToString());
                }
            }
            Console.WriteLine(com.WriteBufferSize);
            //com.WriteTimeout = 5;
            /*
            for (byte add = 0x01; add < 0x05; add++)
            {
                try
                {
                    com.Write(CarControl.SpeedRead(add), 0, 8);
                    Thread.Sleep(5);

                }
                catch (Exception e)
                {
                    Console.WriteLine("串口写入失败 " + e.ToString());
                }
            }
            */
            com.Write(CarControl.BoudRateRead(0x01),0,8);
            Thread.Sleep(40);
            com.Write(CarControl.BoudRateWrite(0x01,19200), 0, 8);
            Thread.Sleep(40);
            Console.ReadKey();
            if (com.IsOpen)
            {
                try
                {
                    com.Close();
                    Console.WriteLine("关闭串口成功");
                }
                catch (Exception e)
                {
                    com = new SerialPort();
                    Console.WriteLine("关闭串口失败" + e.ToString());
                }
            }
        }

        // 串口数据接收
        private static void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var n = com.BytesToRead;//进行数据分段
                var buf = new byte[n];
                com.Read(buf, 0, n);
                for (int i = 0; i < n; i++)
                {
                    Console.Write(buf[i]+" time:"+DateTime.Now.Millisecond+" ");
                }
                Console.Write("\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口接收数据失败 "+ ex.ToString());
            }
        }
    }
}
