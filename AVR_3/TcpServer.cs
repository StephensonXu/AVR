using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AVR_3
{
    class TcpServer
    {
        #region tcpLog
        log tcpLog = new log("D://log", "/tcp.log");
        #endregion
        
        #region 线程
        /// <summary>
        /// 线程 
        /// </summary>
        private Thread _mServerThread;
        private Thread _mReceiveThread;
        #endregion

        #region LisClient_Flag
        private bool LisClient_Flag = true;
        #endregion

        #region 缓存数据
        /// <summary>
        /// 缓存数据
        /// </summary>
        private byte[] buffer = new byte[1024];
        public byte[] Buffer
        {
            get { return buffer; }
        }
        #endregion

        #region 数据长度
        /// <summary>
        /// 数据长度
        /// </summary>
        private int receiveLength;
        public int ReceiveLength
        {
            get { return receiveLength; }
            set { receiveLength = value; }
        }
        #endregion

        #region IP
        /// <summary>
        /// IP
        /// </summary>
        private string _ip;
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        #endregion

        #region 获取IPV4
        /// <summary>
        /// 获取本机IPV4地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalIPV4()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region 端口号
        /// <summary>
        /// 端口号
        /// </summary>
        private int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        #endregion

        #region IP终端
        /// <summary>
        /// IP终端
        /// </summary>
        private IPEndPoint _ipEndPoint;
        #endregion

        #region 服务器端
        /// <summary>
        /// 服务器端socket
        /// </summary>
        private Socket _mServerSocket;
        #endregion

        #region 监听数量
        /// <summary>
        /// 监听数量
        /// </summary>
        private int _maxClientCount;
        public int MaxClientCount
        {
            get { return _maxClientCount; }
            set { _maxClientCount = value; }
        }
        #endregion

        #region 客户端列表
        /// <summary>
        /// 客户端列表
        /// </summary>
        private List<Socket> _mClientSockets;
        public List<Socket> ClientSockets
        {
            get { return _mClientSockets; }
        }
        #endregion

        #region 当前客户端socket
        /// <summary>
        /// 当前客户端socket
        /// </summary>
        private Socket _mClientSocket;
        public Socket ClienSocket
        {
            get { return _mClientSocket; }
            set { _mClientSocket = value; }
        }
        #endregion

        #region 构造函数1/以本机IPV4构造
        /// <summary>
        /// TCPServer 构造函数
        /// </summary>
        /// <param name="port"></param>
        /// <param name="count"></param>
        public TcpServer(int port, int count)
        {
            this._ip = GetLocalIPV4();
            this.Port = port;
            this._maxClientCount = count;
            this._mClientSockets = new List<Socket>();
            //初始化IP终端
            this._ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            //初始化服务器端Socket
            this._mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Socket绑定端口
            this._mServerSocket.Bind(this._ipEndPoint);
            //设置监听数目
            this._mServerSocket.Listen(_maxClientCount);
            //写日志记录
            tcpLog.WriteLog("Tcp init", IP);
        }
        #endregion

        #region 构造函数2
        /// <summary>
        /// TCPServer 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="count"></param>
        public TcpServer(string ip, int port, int count)
        {
            this._ip = ip;
            this.Port = port;
            this._maxClientCount = count;
            this._mClientSockets = new List<Socket>();
            //初始化IP终端
            this._ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            //初始化服务器端Socket
            this._mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Socket绑定端口
            this._mServerSocket.Bind(this._ipEndPoint);
            //设置监听数目
            this._mServerSocket.Listen(_maxClientCount);
        }
        #endregion

        #region 开启服务器线程
        /// <summary>
        /// 定义个start方法
        /// </summary>
        public void Start()
        {
            //创建服务器线程，实现客户端连接请求的循环监听
            _mServerThread = new Thread(this.ListenClientConnect);
            //服务器端线程开启
            _mServerThread.Start();
        }
        #endregion

        #region 结束方法
        /// <summary>
        /// 定义个close方法
        /// </summary>
        public void Close()
        {
            #region 关闭线程1
            if (_mReceiveThread != null)
            {
                _mReceiveThread.Abort();
            }
            #endregion
            #region 关闭套接字
            if (_mServerSocket != null)
            {
                _mServerSocket.Close();
            }
            #endregion
            #region 关闭线程2
            if (_mServerThread != null)
            {
                LisClient_Flag = false;
                _mServerThread.Abort();
            }
            #endregion
            #region 关闭日志
            tcpLog.WriteLog("tcpClose","");
            #endregion
        }
        #endregion

        #region 监听客户端连接
        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private void ListenClientConnect()
        {
            while (LisClient_Flag)
            {
                #region 用捕获异常来解决关闭Accept阻塞线程
                try
                {
                    //获取连接到服务器端的客户端
                    this._mClientSocket = this._mServerSocket.Accept();
                    //将获取到的客户端添加到客户端列表
                    this._mClientSockets.Add(this._mClientSocket);
                    //创建客户端消息线程，实现客户端消息的循环监听
                    _mReceiveThread = new Thread(this.ReceiveClient);
                    //启动线程
                    _mReceiveThread.Start(this.ClienSocket);
                    //写日志连接成功
                    tcpLog.WriteLog("连接成功", "");
                    Console.WriteLine("连接成功");
                }
                catch (Exception e)
                {
                    ;
                }
                #endregion
            }
        }
        #endregion

        #region 接收客户端消息
        /// <summary>
        /// 接收客户端消息的方法
        /// </summary>
        /// <param name="obj"></param>
        public void ReceiveClient(object obj)
        {
            var mClientSocket = (Socket)obj;
            //循环标识位
            bool flag = true;
            while (flag)
            {
                try
                {
                    //获取数据长度                    
                    receiveLength = mClientSocket.Receive(buffer);
                }
                catch (Exception e)
                {
                    //从客户端列表中移除该客户端
                    this._mClientSockets.Remove(mClientSocket);
                    //断开连接
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    tcpLog.WriteLog("断开连接", e.ToString());
                    Array.Clear(buffer, 0, 8);
                    break;
                }
            }
        }
        #endregion

        #region 向客户端群发消息
        /// <summary>
        /// 向客户端群发string消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string msg)
        {
            if (msg == string.Empty || this._mClientSockets.Count <= 0) return;
            try
            {
                foreach (Socket s in this._mClientSockets)
                {
                    (s as Socket).Send(Encoding.UTF8.GetBytes(msg));
                }
            }
            catch (Exception e)
            {
                tcpLog.WriteLog("发送失败", e.ToString());
            }
        }
        /// <summary>
        /// 向客户端群发byte消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(byte[] msg)
        {
            if (msg == null || this._mClientSockets.Count <= 0) return;
            try
            {
                foreach (Socket s in this._mClientSockets)
                {
                    (s as Socket).Send(msg);
                }
            }            
            catch (Exception e)
            {
                tcpLog.WriteLog("发送失败", e.ToString());
            }
        }
        #endregion

        #region 向指定客户端发消息
        /// <summary>
        /// 向指定客户端发送string消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="msg"></param>
        public void SendMessage(string ip, int port, string msg)
        {
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
            if (msg == string.Empty) return;
            try
            {
                foreach (Socket s in this._mClientSockets)
                {
                    if (ipEnd.Equals((IPEndPoint)s.RemoteEndPoint))
                    {
                        s.Send(Encoding.UTF8.GetBytes(msg));
                    }
                }
            }            
            catch (Exception e)
            {
                tcpLog.WriteLog("发送失败", e.ToString());
            }
        }
        /// <summary>
        /// 向指定客户端发送byte消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="msg"></param>
        public void SendMessage(string ip, int port, byte[] msg)
        {
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
            if (msg == null) return;
            try
            {
                foreach (Socket s in this._mClientSockets)
                {
                    if (ipEnd.Equals((IPEndPoint)s.RemoteEndPoint))
                    {
                        s.Send(msg);
                    }
                }
            }            
            catch (Exception e)
            {
                tcpLog.WriteLog("发送失败", e.ToString());
            }
        }
        #endregion
    }
}
