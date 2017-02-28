using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;

namespace AVR_3
{
    class Program
    {
        static log mainLog = new log("D://log", "/main.log");//log

        static TcpServer tcpServer = new TcpServer(9966, 10);//tcpsever
        static bool Transmit = false;
        static byte[] buffer = new byte[1024];//tcp-data
        static byte[] startbuffer = new byte[1024];//start-data
        static byte[] send = { 0xff, 0xff, 0xff, 0xff, 0xff, 0x06 };//send-data
        static int tansError = 10;//max error tans count
        static int Trans_error_count = 0;//Trans_error_count

        static SerialPort com = new SerialPort("COM2", 9600, Parity.Even, 8, StopBits.One);//串口
        static int serial_time = 30;//serial interval time

        static GPIO gpio = new GPIO(1);//gpio-1
        static Boolean SetIoH = false;//io-flag   
        
        static Video myVideo=new Video();
        static bool videotrans = false;//video-flag

        private static bool emergyStop = false;//emeergy-stop flag

        static void Main(string[] args)
        {
            Main_init();//初始化
            Console.WriteLine("主程序初始化成功");
            mainLog.WriteLog("主程序初始化成功","");

            CarCtr();//car-process                     
        }

        //time_tick
        static void time_tick(object source, System.Timers.ElapsedEventArgs e)
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
                    Trans_error_count++;//异常记录
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
        static void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int n = com.BytesToRead;
                byte[] buf = new byte[n];
                com.Read(buf, 0, n);
            }
            catch (Exception ex)
            {
                mainLog.WriteLog("串口接收数据失败", ex.ToString());
            }
        }
        static void Main_init()
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
            System.Timers.Timer dogTimer = new System.Timers.Timer(200);
            dogTimer.Elapsed += time_tick; //到达时间的时候执行事件；   
            dogTimer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)； 
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
        static void CarCtr()
        {
            Init();
            while (true)
            {
                if (Trans_error_count >= tansError) Trans_Error();
                if (buffer[0] == 0x01) Car_LowSpeed();
                if (buffer[0] == 0x02) Car_HighSpeed();
                if (buffer[0] == 0x03) Car_LowTurn();
                if (buffer[0] == 0x04) Car_HighTurn();
                if (buffer[0] == 0x05) Car_Demo();
                if (buffer[0] == 0x0e && buffer[1] == 0xff && buffer[2] == 0xff && buffer[3] == 0xff) Car_EmergencyStop();
                if (buffer[0] == 0x40) Openvideo();
                if (buffer[0] == 0x41) Closevideo();
                if (buffer[0] == 0x11) yao_1();
                if (buffer[0] == 0x12) yao_2();
            }
        }

        // 车初始化程序
        static void Init()
        {
        }

        #region Trans_Error
        /// <summary>
        /// 通信断,程序退出
        /// </summary>
        static void Trans_Error()
        {
            try
            {
                #region 如果抱闸打开，则写入急停，并关闭通讯，这样子由于没有抱闸关闭，导致就程序一开始启动时会可能暴动下，后面则无
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
                    #region 帧间隔30ms
                    Thread.Sleep(30);
                    #endregion
                }
                Thread.Sleep(1000);                    
                com.Close();
                #endregion
                if (videotrans == true)
                {
                    myVideo.CloseVideo();
                }
                mainLog.WriteLog("通信中断", "强制退出");               
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                mainLog.WriteLog("通信中断","强制退出失败");
            }          
        }
        #endregion
        #region Car_LowSpeed
        /// <summary>
        /// 低速运行
        /// </summary>
        static void Car_LowSpeed()
        {
            #region 速度表示
            float[] temp = new float[4];//四个电机的速度
            int v, vdif, v_f;//车速度和差速
            #endregion
            while (true)
            {
                
                #region 速度获取
                v = buffer[2];
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                vdif = buffer[3] * v / 90;
                #endregion
                #region 通信失常
                if (Trans_error_count >= tansError) Trans_Error();
                #endregion
                #region 跳出循环判断
                if (buffer[0] != 0x01) break;
                #endregion
                #region 获取速度
                switch (buffer[1])
                {
                    case 0:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                    case 1:
                        temp[0] = v - vdif;
                        temp[1] = v - vdif;
                        temp[2] = v;
                        temp[3] = v;
                        break;
                    case 2:
                        temp[0] = v;
                        temp[1] = v;
                        temp[2] = v - vdif;
                        temp[3] = v - vdif;
                        break;
                    case 3:
                        v_f = v * 8 / 10;
                        temp[0] = -v_f;
                        temp[1] = -v_f;
                        temp[2] = -v_f + vdif;
                        temp[3] = -v_f + vdif;
                        break;
                    case 4:
                        v_f = v * 8 / 10;
                        temp[0] = -v_f + vdif;
                        temp[1] = -v_f + vdif;
                        temp[2] = -v_f;
                        temp[3] = -v_f;
                        break;
                    default:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                }
                #endregion
                #region 写入速度
                if (buffer[1] != 255)
                {
                    
                    #region 写入速度
                    for (byte add = 0x01; add < 0x05; add++)
                    {
                        #region 校正电机正反方向
                        float a = 0;
                        if (add == 0x01 || add == 0x02)
                        {
                            a = 1;
                        }
                        else
                        {
                            a = -1;
                        }
                        #endregion
                        try
                        {
                            com.Write(new CarControl().CarSpeedWrite(add, a*temp[add - 1]*20), 0, 8);
                        }
                        catch (Exception e)
                        {
                            mainLog.WriteLog("串口写入失败", e.ToString());
                        }

                        #region 帧间隔30ms
                        Thread.Sleep(30);
                        #endregion
                    }
                    #endregion
                    
                }
                #endregion                
                #region 延时降低CPU负担                
                Thread.Sleep(100);
                #endregion
            }
        }
        #endregion
        #region Car_HighSpeed
        /// <summary>
        /// 低速运行
        /// </summary>
        static void Car_HighSpeed()
        {
            #region 速度表示
            float[] temp = new float[4];//四个电机的速度
            int v, vdif, v_f;//车速度和差速
            #endregion
            while (true)
            {

                #region 速度获取
                v = buffer[2];
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                vdif = buffer[3] * v / 90;
                #endregion
                #region 通信失常
                if (Trans_error_count >= tansError) Trans_Error();
                #endregion
                #region 跳出循环判断
                if (buffer[0] != 0x02) break;
                #endregion
                #region 获取速度
                switch (buffer[1])
                {
                    case 0:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                    case 1:
                        temp[0] = v - vdif;
                        temp[1] = v - vdif;
                        temp[2] = v;
                        temp[3] = v;
                        break;
                    case 2:
                        temp[0] = v;
                        temp[1] = v;
                        temp[2] = v - vdif;
                        temp[3] = v - vdif;
                        break;
                    case 3:
                        v_f = v * 8 / 10;
                        temp[0] = -v_f;
                        temp[1] = -v_f;
                        temp[2] = -v_f + vdif;
                        temp[3] = -v_f + vdif;
                        break;
                    case 4:
                        v_f = v * 8 / 10;
                        temp[0] = -v_f + vdif;
                        temp[1] = -v_f + vdif;
                        temp[2] = -v_f;
                        temp[3] = -v_f;
                        break;
                    default:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                }
                #endregion
                #region 写入速度
                if (buffer[1] != 255)
                {

                    #region 写入速度
                    for (byte add = 0x01; add < 0x05; add++)
                    {
                        #region 校正电机正反方向
                        float a = 0;
                        if (add == 0x01 || add == 0x02)
                        {
                            a = 1;
                        }
                        if (add == 0x03 || add == 0x04)
                        {
                            a = -1;
                        }
                        #endregion
                        try
                        {
                            com.Write(new CarControl().CarSpeedWrite(add, a * temp[add - 1] * 40), 0, 8);
                        }
                        catch (Exception e)
                        {
                            mainLog.WriteLog("串口写入失败", e.ToString());
                        }

                        #region 帧间隔30ms
                        Thread.Sleep(30);
                        #endregion
                    }
                    #endregion

                }
                #endregion
                #region 延时降低CPU负担
                Thread.Sleep(100);
                #endregion
            }
        }
        #endregion
        #region Car_LowTurn
        /// <summary>
        /// 低速运行
        /// </summary>
        static void Car_LowTurn()
        {
            #region 速度表示
            float[] temp = new float[4];//四个电机的速度
            int v, vdif, v_f;//车速度和差速
            #endregion
            while (true)
            {
                #region 速度获取
                v = buffer[2];
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                #endregion
                #region 通信失常
                if (Trans_error_count >= tansError) Trans_Error();
                #endregion
                #region 跳出循环判断
                if (buffer[0] != 0x03) break;
                #endregion
                #region 获取速度
                switch (buffer[1])
                {
                    case 0:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                    case 2:
                        temp[0] = v;
                        temp[1] = v;
                        temp[2] = -v;
                        temp[3] = -v;
                        break;
                    case 4:
                        temp[0] = -v;
                        temp[1] = -v;
                        temp[2] = v;
                        temp[3] = v;
                        break;
                    default:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                }
                #endregion
                #region 写入速度
                if (buffer[1] != 255)
                {

                    #region 写入速度
                    for (byte add = 0x01; add < 0x05; add++)
                    {
                        #region 校正电机正反方向
                        float a = 0;
                        if (add == 0x01 || add == 0x02)
                        {
                            a = 1;
                        }
                        if (add == 0x03 || add == 0x04)
                        {
                            a = -1;
                        }
                        #endregion
                        try
                        {
                            com.Write(new CarControl().CarSpeedWrite(add, a * temp[add - 1] * 20), 0, 8);
                        }
                        catch (Exception e)
                        {
                            mainLog.WriteLog("串口写入失败", e.ToString());
                        }

                        #region 帧间隔30ms
                        Thread.Sleep(30);
                        #endregion
                    }
                    #endregion

                }
                #endregion
                #region 延时降低CPU负担
                Thread.Sleep(100);
                #endregion
            }
        }
        #endregion
        #region Car_HighTurn
        /// <summary>
        /// 低速运行
        /// </summary>
        static void Car_HighTurn()
        {
            #region 速度表示
            float[] temp = new float[4];//四个电机的速度
            int v, vdif, v_f;//车速度和差速
            #endregion
            while (true)
            {
                #region 速度获取
                v = buffer[2];
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                #endregion
                #region 通信失常
                if (Trans_error_count >= tansError) Trans_Error();
                #endregion
                #region 跳出循环判断
                if (buffer[0] != 0x04) break;
                #endregion
                #region 获取速度
                switch (buffer[1])
                {
                    case 0:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                    case 2:
                        temp[0] = v;
                        temp[1] = v;
                        temp[2] = -v;
                        temp[3] = -v;
                        break;
                    case 4:
                        temp[0] = -v;
                        temp[1] = -v;
                        temp[2] = v;
                        temp[3] = v;
                        break;
                    default:
                        temp[0] = 0;
                        temp[1] = 0;
                        temp[2] = 0;
                        temp[3] = 0;
                        break;
                }
                #endregion
                #region 写入速度
                if (buffer[1] != 255)
                {

                    #region 写入速度
                    for (byte add = 0x01; add < 0x05; add++)
                    {
                        #region 校正电机正反方向
                        float a = 0;
                        if (add == 0x01 || add == 0x02)
                        {
                            a = 1;
                        }
                        if (add == 0x03 || add == 0x04)
                        {
                            a = -1;
                        }
                        #endregion
                        try
                        {
                            com.Write(new CarControl().CarSpeedWrite(add, a * temp[add - 1] * 40), 0, 8);
                        }
                        catch (Exception e)
                        {
                            mainLog.WriteLog("串口写入失败", e.ToString());
                        }

                        #region 帧间隔30ms
                        Thread.Sleep(30);
                        #endregion
                    }
                    #endregion

                }
                #endregion
                #region 延时降低CPU负担
                Thread.Sleep(100);
                #endregion
            }
        }
        #endregion
        #region Car_Demo
        /// <summary>
        /// 车演示程序
        /// </summary>
        static void Car_Demo()
        {
        }
        #endregion
        #region Car_EmergencyStop
        /// <summary>
        /// 车急停
        /// </summary>
        static void Car_EmergencyStop()
        {
            if (emergyStop == false)
            {
                #region 紧急停止
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
                    #region 帧间隔30ms
                    Thread.Sleep(30);
                    #endregion
                }
                #endregion
                #region 延时
                Thread.Sleep(500);
                #endregion
                #region 关闭抱闸
                gpio.SetIoLow();
                SetIoH = false;
                #endregion 
                emergyStop = true;
            }
            else
            {
                gpio.SetIoHigh();
                emergyStop = false;
            }
            
        }
        #endregion
        #region yao_1,pwm -1000-1000,设为最大为500
        private static void yao_1()
        {
            #region 速度表示
            int v;//腰速度
            #endregion
            while (true)
            {
                #region 速度获取
                v = buffer[2];                
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                v = (buffer[1] == 0x02) ? v : v*(-1);
                #endregion
                #region 通信失常
                if (Trans_error_count >= 20) Trans_Error();
                #endregion
                #region 跳出循环判断
                if (buffer[0] != 0x11) break;
                #endregion
                #region 写入速度                    
                try
                {
                    com.Write(new YaoControl().yaoSpeedWrite(0x05, v * 5), 0, 8);
                }
                catch (Exception e)
                {
                    mainLog.WriteLog("串口写入失败", e.ToString());
                }
                #endregion
                #region 延时降低CPU负担
                Thread.Sleep(100);
                #endregion
            }
        }
        #endregion
        #region yao_2,pwm -1000-1000,设为最大为500
        private static void yao_2()
        {
            #region 速度表示
            int v;//腰速度
            #endregion

            while (true)
            {
                #region 速度获取

                v = buffer[2];
                if (v >= 99)
                {
                    v = 99;
                }
                if (v <= 2)
                {
                    v = 0;
                }
                v = (buffer[1] == 0x02) ? v : v*(-1);

                #endregion

                #region 通信失常

                if (Trans_error_count >= 20) Trans_Error();

                #endregion

                #region 跳出循环判断

                if (buffer[0] != 0x11) break;

                #endregion

                #region 写入速度                    

                try
                {
                    com.Write(new YaoControl().yaoSpeedWrite(0x06, v*5), 0, 8);
                }
                catch (Exception e)
                {
                    mainLog.WriteLog("串口写入失败", e.ToString());
                }

                #endregion

                #region 延时降低CPU负担

                Thread.Sleep(100);

                #endregion
            }
        }
        #endregion
        #region 开摄像头
        static void Openvideo()
        {
            if (videotrans == false)
            {
                myVideo.OpenVideo();
                videotrans = true;
            }            
        }
        #endregion
        #region 关摄像头
        static void Closevideo()
        {
            if (videotrans == true)
            {
                myVideo.CloseVideo();
                videotrans = false;
            }            
        }
        #endregion                

    }
}
