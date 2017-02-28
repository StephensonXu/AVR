using System;
using System.Runtime.InteropServices;

namespace AVR_3
{
    internal class GPIO
    {
        #region GpioLog

        private readonly log gpioLog = new log("D://log", "/gpio.log");

        #endregion

        #region constructor

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="group"></param>
        public GPIO(int group)
        {
            GROUP = group;
        }

        #endregion

        #region init

        /// <summary>
        ///     init
        /// </summary>
        public void InitIo()
        {
            if (HANDLE != null)
            {
                aaeonSmbusClose(HANDLE);
            }
            HANDLE = aaeonSmbusOpen();
            try
            {
                aaeonSmbusWriteByte(HANDLE, 0X6E, (byte) (GROUP*16), 0xFF); //all output
                aaeonSmbusWriteByte(HANDLE, 0X6E, (byte) (GROUP*16 + 1), 0x00); //all low
            }
            catch (Exception e)
            {
                gpioLog.WriteLog("gpio初始化失败", e.ToString());
            }
        }

        #endregion

        #region setiolow

        /// <summary>
        ///     low
        /// </summary>
        public void SetIoLow()
        {
            try
            {
                aaeonSmbusWriteByte(HANDLE, 0X6E, (byte) (GROUP*16 + 1), 0x00); //all low
            }
            catch (Exception e)
            {
                gpioLog.WriteLog("gpio置低位失败", e.ToString());
            }
        }

        #endregion

        #region setiohigh

        /// <summary>
        ///     high
        /// </summary>
        public void SetIoHigh()
        {
            try
            {
                aaeonSmbusWriteByte(HANDLE, 0X6E, (byte) (GROUP*16 + 1), 0xff); //all high
            }
            catch (Exception e)
            {
                gpioLog.WriteLog("gpio置高位失败", e.ToString());
            }
        }

        #endregion

        #region close

        /// <summary>
        ///     close
        /// </summary>
        public void CloseIo()
        {
            try
            {
                aaeonSmbusWriteByte(HANDLE, 0X6E, (byte) (GROUP*16 + 1), 0x00); //all low
                aaeonSmbusClose(HANDLE);
            }
            catch (Exception e)
            {
                gpioLog.WriteLog("gpio关闭失败", e.ToString());
            }
        }

        #endregion

        #region import dll

        /// <summary>
        ///     import dll
        /// </summary>
        /// <returns></returns>
        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr aaeonSmbusOpen();

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool aaeonSmbusClose(IntPtr hInst);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte aaeonSmbusGetStatus(IntPtr hInst);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int aaeonSmbusSetBaseAddr(ushort wAddr);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int aaeonSmbusGetBaseAddr(ref ushort pwAddr);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int aaeonSmbusReadByte(IntPtr hInst, byte bSlaveAddr, byte bReg, ref byte pbData);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int aaeonSmbusWriteByte(IntPtr hInst, byte bSlaveAddr, byte bReg, byte bData);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int aaeonSmbusReadWord(IntPtr hInst, byte bSlaveAddr, byte bReg, ref ushort pwData);

        [DllImport("aaeonSmbus.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int aaeonSmbusWriteWord(IntPtr hInst, byte bSlaveAddr, byte bReg, ushort wData);

        #endregion

        #region Handle

        /// <summary>
        ///     Handle
        /// </summary>
        private IntPtr Handle;

        public IntPtr HANDLE
        {
            get { return Handle; }
            set { Handle = value; }
        }

        #endregion

        #region Group

        /// <summary>
        ///     Group
        /// </summary>
        private int Group;

        public int GROUP
        {
            get { return Group; }
            set { Group = value; }
        }

        #endregion
    }
}