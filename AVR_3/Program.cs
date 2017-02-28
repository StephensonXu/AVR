using System;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AVR_3
{
    internal class Program
    {
        private static readonly log mainLog = new log("D://log", "/main.log"); //log

        private static readonly TcpServer tcpServer = new TcpServer(9966, 10); //tcpsever
        private static bool Transmit;
        private static byte[] buffer = new byte[1024]; //tcp-data
        private static byte[] startbuffer = new byte[1024]; //start-data
        private static readonly byte[] send = {0xff, 0xff, 0xff, 0xff, 0xff, 0x06}; //send-data
        private static readonly int tansError = 10; //max error tans count
        private static int Trans_error_count; //Trans_error_count

        private static SerialPort com = new SerialPort("COM2", 9600, Parity.Even, 8, StopBits.One); //串口
        private static readonly int serial_time = 30; //serial interval time
        private static readonly int cpu_time = 100; //cpu time

        private static readonly GPIO gpio = new GPIO(1); //gpio-1
        private static bool SetIoH; //io-flag   

        private static readonly Video myVideo = new Video();
        private static bool videotrans; //video-flag

        private static bool emergyStop; //emeergy-stop flag

        private static void Main(string[] args)
        {
            Main_init(); //初始化
            Console.WriteLine("主程序初始化成功");
            mainLog.WriteLog("主程序初始化成功", "");

            CarCtr(); //car-process                     
        }

        //time_tick
        private static void time_tick(object source, ElapsedEventArgs e)
        {
            //第一次接收数据，置通信位
            startbuffer = tcpServer.Buffer;
            if (startbuffer[5] == 6 && Transmit == false)
            {
                Transmit = true;
            }
            //已经通信
            if (Transmit)
            {
                buffer = tcpServer.Buffer;
                //mainLog.WriteLog("缓冲数据", buffer[0].ToString() + " " + buffer[1].ToString() + " " + buffer[2].ToString() + " " + buffer[3].ToString() + " " + buffer[4].ToString() + " " + buffer[5].ToString());                
                if (buffer[4] == 0 && buffer[5] == 0x06)
                {
                    send[4] = 1;
                }
                else
                {
                    Trans_error_count++; //异常记录
                    mainLog.WriteLog("异常计数", Trans_error_count.ToString());
                    Console.WriteLine(Trans_error_count.ToString());
                    if (Trans_error_count < tansError)
                    {
                        send[4] = 1;
                    }
                }
                tcpServer.SendMessage(send);
            }
        }

        // 串口数据接收
        private static void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var n = com.BytesToRead;
                var buf = new byte[n];
                com.Read(buf, 0, n);
            }
            catch (Exception ex)
            {
                mainLog.WriteLog("串口接收数据失败", ex.ToString());
            }
        }

        private static void Main_init()
        {
            //打开服务器
            tcpServer.Start();
            //注册串口接收方法
            com.NewLine = "\r\n";
            com.DataReceived += com_DataReceived;
            //打开io口及继电器
            gpio.InitIo();
            gpio.SetIoHigh();
            SetIoH = true;
            //打开串口
            if (!com.IsOpen)
            {
                try
                {
                    com.Open();
                }
                catch (Exception e)
                {
                    com = new SerialPort();
                    mainLog.WriteLog("打开串口失败", e.ToString());
                }
            }
            //启动timer
            var dogTimer = new Timer(200);
            dogTimer.Elapsed += time_tick; //到达时间的时候执行事件；   
            dogTimer.AutoReset = true; //设置是执行一次（false）还是一直执行(true)； 
            try
            {
                dogTimer.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；  
            }
            catch (Exception e)
            {
                mainLog.WriteLog("看门狗使能失败", e.ToString());
            }
        }

        // 车控总程序
        private static void CarCtr()
        {
            Init();
            while (true)
            {
                if (Trans_error_count >= tansError) Trans_Error();
                if (buffer[0] == 0x01 || buffer[0] == 0x02 || buffer[0] == 0x03 || buffer[0] == 0x04) Car_run();
                if (buffer[0] == 0x05) Car_Demo();
                if (buffer[0] == 0x0e && buffer[1] == 0xff && buffer[2] == 0xff && buffer[3] == 0xff)
                    Car_EmergencyStop();
                if (buffer[0] == 0x40) Openvideo();
                if (buffer[0] == 0x41) Closevideo();
                if (buffer[0] == 0x11) yao();
            }
        }

        // 车初始化程序
        private static void Init()
        {
        }


        // 通信断,程序退出
        private static void Trans_Error()
        {
            try
            {
                for (byte add = 0x01; add < 0x05; add++)
                {
                    try
                    {
                        com.Write(new CarControl().EmergeStop(add), 0, 8);
                    }
                    catch (Exception e)
                    {
                        mainLog.WriteLog("串口写入失败", e.ToString());
                    }
                    Thread.Sleep(serial_time);
                }
                Thread.Sleep(1000);
                com.Close();
                if (videotrans)
                {
                    myVideo.CloseVideo();
                }
                mainLog.WriteLog("通信中断", "强制退出");
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                mainLog.WriteLog("通信中断", "强制退出失败");
            }
        }


        // 车运行
        private static void Car_run()
        {
            var temp = new float[4]; //四个电机的速度
            int v, vdif, v_f; //车速度和差速
            var coefficient = 0;
            while (true)
            {
                if (Trans_error_count >= tansError) Trans_Error(); //通信失常
                if (buffer[0] != 0x01 || buffer[0] != 0x02 || buffer[0] != 0x03 || buffer[0] != 0x04) break; //跳出循环判断

                v = buffer[2];
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                vdif = buffer[3]*v/90;
                switch (buffer[0]*10 + buffer[1]) //10,11,12,13,14,20,21,22,23,24,30,32,34,40,42,44
                {
                    case 10:
                    case 20:
                    case 30:
                    case 40:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                    case 11:
                    case 21:
                        temp[0] = v - vdif;
                        temp[1] = v - vdif;
                        temp[2] = v*-1;
                        temp[3] = v*-1;
                        break;
                    case 12:
                    case 22:
                        temp[0] = v;
                        temp[1] = v;
                        temp[2] = (v - vdif)*-1;
                        temp[3] = (v - vdif)*-1;
                        break;
                    case 13:
                    case 23:
                        v_f = v*8/10;
                        temp[0] = -v_f;
                        temp[1] = -v_f;
                        temp[2] = (-v_f + vdif)*-1;
                        temp[3] = (-v_f + vdif)*-1;
                        break;
                    case 14:
                    case 24:
                        v_f = v*8/10;
                        temp[0] = -v_f + vdif;
                        temp[1] = -v_f + vdif;
                        temp[2] = -v_f*-1;
                        temp[3] = -v_f*-1;
                        break;
                    case 32:
                    case 42:
                        temp[0] = v;
                        temp[1] = v;
                        temp[2] = v;
                        temp[3] = v;
                        break;
                    case 34:
                    case 44:
                        temp[0] = -v;
                        temp[1] = -v;
                        temp[2] = -v;
                        temp[3] = -v;
                        break;
                    default:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                }
                switch (buffer[0])
                {
                    case 1:
                    case 3:
                        coefficient = 20;
                        break;
                    case 2:
                    case 4:
                        coefficient = 40;
                        break;
                }
                //写入速度
                if (buffer[1] != 255)
                {
                    for (byte add = 0x01; add < 0x05; add++)
                    {
                        try
                        {
                            com.Write(new CarControl().CarSpeedWrite(add, coefficient*temp[add - 1]), 0, 8);
                        }
                        catch (Exception e)
                        {
                            mainLog.WriteLog("串口写入失败", e.ToString());
                        }
                        Thread.Sleep(serial_time);
                    }
                }

                Thread.Sleep(cpu_time); //延时降低CPU负担
            }
        }


        // 车演示程序
        private static void Car_Demo()
        {
        }

        // 车急停
        private static void Car_EmergencyStop()
        {
            if (emergyStop == false)
            {
                for (byte add = 0x01; add < 0x05; add++)
                {
                    try
                    {
                        com.Write(new CarControl().EmergeStop(add), 0, 8);
                    }
                    catch (Exception e)
                    {
                        mainLog.WriteLog("串口写入失败", e.ToString());
                    }
                    Thread.Sleep(serial_time);
                }
                Thread.Sleep(500);

                gpio.SetIoLow(); //关闭抱闸
                SetIoH = false;

                emergyStop = true;
            }
            else
            {
                gpio.SetIoHigh();
                emergyStop = false;
            }
        }

        // yao
        private static void yao()
        {
            ;
        }

        #region 开摄像头

        private static void Openvideo()
        {
            if (videotrans == false)
            {
                myVideo.OpenVideo();
                videotrans = true;
            }
        }

        #endregion

        #region 关摄像头

        private static void Closevideo()
        {
            if (videotrans)
            {
                myVideo.CloseVideo();
                videotrans = false;
            }
        }

        #endregion
    }
}