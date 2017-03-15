using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using CarControl;

namespace ChangeBoudRate
{
    class Program
    {
        private static SerialPort com;
        private static volatile byte[] check = {0x01, 0x03};//0x06;0x10
        private static AutoResetEvent myEvent = new AutoResetEvent(false);
        private static int New_BaudRate = 9600;
        static void Main(string[] args)
        {
            String[] enter = { "Please enter the BaudRate now use:", "Please Set the BaudRate you want to change (9600/19200/38400/57600, perfer 38400):"};
            int Oral_BaudRate = 9600;                       
            Oral_BaudRate = getBaudRate(0,enter);
            New_BaudRate = getBaudRate(1, enter);
            String comName = "COM2";
            Console.WriteLine("Please enter COM port name (COM2/COM6..):");
            comName = Console.ReadLine().Length == 3 ? Console.ReadLine() : comName;
            Console.WriteLine(comName);
            com = new SerialPort(comName, Oral_BaudRate, Parity.Odd, 8, StopBits.One);
            com.NewLine = "\r\n";
            com.DataReceived += com_DataReceived;
            if (!com.IsOpen)
            {
                try
                {
                    com.Open();
                    Console.WriteLine("打开串口成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine("打开串口失败" + e.ToString());
                    Console.ReadKey();
                    return;
                }
            }
            int count = 0;//记录未check次数，>5次显示串口有问题
            int sum = 0;
            for (byte add = 0x01; add < 0x05; add++)
            {
                sum += SerialWrite(add, new byte[] {add, 0x10}, count);
            }
            if (sum == 0)
                Console.WriteLine("BoudRate changes successful! And please change the BaudRate in AVR_Release_Set.json!");
            else Console.WriteLine("BoudRate changes failed! Please check!");
            if (com.IsOpen)
            {
                try
                {
                    com.Close();
                    Console.WriteLine("关闭串口成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine("关闭串口失败" + e.ToString());
                    Console.ReadKey();
                    return;
                }
            }
            Thread.Sleep(4000);
        }

        static int  SerialWrite(byte add, byte[] newcode, int count)
        {
            if (count >= 5)
            {
                Console.WriteLine("串口通讯超时: "+add+"驱动器");
                return -1;
            }
            com.Write(CarModbus.BoudRateWrite(add, New_BaudRate), 0, 13);
            check = newcode;
            if (myEvent.WaitOne(500)) return 0;
            return SerialWrite(add, newcode,++count);
        }
        static int getBaudRate(int Case, String[] enter)
        {
            Console.WriteLine(enter[Case]);
            int BaudRate = 9600;
            try
            {
                BaudRate = int.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("please Enter the correct Number!");
                BaudRate =getBaudRate(Case,enter);
            }
            Console.WriteLine("Please enter ok/wrong to check the num is: " + BaudRate);
            if (Console.ReadLine()=="ok") return BaudRate;
            return getBaudRate(Case,enter);
        }

        private static void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var n = com.BytesToRead;//进行数据分段
                var buf = new byte[n];
                com.Read(buf, 0, n);
                if (buf[0] == check[0] && buf[1] == check[1])
                    myEvent.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口接收数据失败 " + ex.ToString());
            }
        }
    }
}
