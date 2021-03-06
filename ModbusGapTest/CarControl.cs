﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusGapTest
{
    internal class CarControl
    {
        #region 车写入速度

        /// <summary>
        ///     车写入速度
        /// </summary>
        /// <param name="address"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static byte[] CarSpeedWrite(byte address, float velocity)
        {
            var velocitySend = new byte[8];
            velocitySend[0] = address;
            velocitySend[1] = 0x06;
            velocitySend[2] = 0x00;
            velocitySend[3] = 0x43;
            var velocity0 = (short)velocity;
            velocitySend[4] = (byte)(velocity0 >> 8);
            velocitySend[5] = (byte)velocity0;
            velocitySend[6] = CalCrc16_CRCHi(velocitySend, 6);
            velocitySend[7] = CalCrc16_CRCLo(velocitySend, 6);
            return velocitySend;
        }

        #endregion

        #region 车写入位置

        /// <summary>
        ///     车写入位置
        /// </summary>
        /// <param name="address"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static byte[] CarPositionWrite(byte address, float position)
        {
            var positionSend = new byte[8];
            positionSend[0] = address;
            positionSend[1] = 0x06;
            positionSend[2] = 0x00;
            positionSend[3] = 0x47;
            var position0 = (short)position;
            positionSend[4] = (byte)(position0 >> 8);
            positionSend[5] = (byte)position0;
            positionSend[6] = CalCrc16_CRCHi(positionSend, 6);
            positionSend[7] = CalCrc16_CRCLo(positionSend, 6);
            return positionSend;
        }

        #endregion

        #region 车读取速度

        /// <summary>
        ///     车读取速度
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] SpeedRead(byte address)
        {
            var velocityReceive = new byte[8];
            velocityReceive[0] = address;
            velocityReceive[1] = 0x03;
            velocityReceive[2] = 0x00;
            velocityReceive[3] = 0x22;
            velocityReceive[4] = 0x00;
            velocityReceive[5] = 0x01;
            velocityReceive[6] = CalCrc16_CRCHi(velocityReceive, 6);
            velocityReceive[7] = CalCrc16_CRCLo(velocityReceive, 6);
            return velocityReceive;
        }

        #endregion

        #region 车读取电流

        /// <summary>
        ///     车读取电流
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] CurrentRead(byte address)
        {
            var CurrentReceive = new byte[8];
            CurrentReceive[0] = address;
            CurrentReceive[1] = 0x03;
            CurrentReceive[2] = 0x00;
            CurrentReceive[3] = 0x21;
            CurrentReceive[4] = 0x00;
            CurrentReceive[5] = 0x01;
            CurrentReceive[6] = CalCrc16_CRCHi(CurrentReceive, 6);
            CurrentReceive[7] = CalCrc16_CRCLo(CurrentReceive, 6);
            return CurrentReceive;
        }

        #endregion

        #region 车读取位置

        /// <summary>
        ///     车读取位置
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] PositionReed(byte address)
        {
            var positionReceive = new byte[8];
            positionReceive[0] = address;
            positionReceive[1] = 0x03;
            positionReceive[2] = 0x00;
            positionReceive[3] = 0x24;
            positionReceive[4] = 0x00;
            positionReceive[5] = 0x02;
            positionReceive[6] = CalCrc16_CRCHi(positionReceive, 6);
            positionReceive[7] = CalCrc16_CRCLo(positionReceive, 6);
            return positionReceive;
        }

        #endregion

        #region 车读取电流与位置与速度等状态信息

        /// <summary>
        ///     车读取电流与位置与速度等状态信息
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] CPVRead(byte address)
        {
            var CPVReceive = new byte[8];
            CPVReceive[0] = address;
            CPVReceive[1] = 0x03;
            CPVReceive[2] = 0x00;
            CPVReceive[3] = 0x20;
            CPVReceive[4] = 0x00;
            CPVReceive[5] = 0x15;
            CPVReceive[6] = CalCrc16_CRCHi(CPVReceive, 6);
            CPVReceive[7] = CalCrc16_CRCLo(CPVReceive, 6);
            return CPVReceive;
        }

        #endregion

        #region 车读取波特率

        /// <summary>
        ///     车读取波特率
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] BoudRateRead(byte address)
        {
            var BoudRateReceive = new byte[8];
            BoudRateReceive[0] = address;
            BoudRateReceive[1] = 0x03;
            BoudRateReceive[2] = 0x00;
            BoudRateReceive[3] = 0x90;
            BoudRateReceive[4] = 0x00;
            BoudRateReceive[5] = 0x02;
            BoudRateReceive[6] = CalCrc16_CRCHi(BoudRateReceive, 6);
            BoudRateReceive[7] = CalCrc16_CRCLo(BoudRateReceive, 6);
            return BoudRateReceive;
        }

        #endregion

        #region 车写波特率

        /// <summary>
        ///     车写波特率
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] BoudRateWrite(byte address,int BoudRate)
        {
            var BoudRateSend = new byte[13];
            BoudRateSend[0] = address;
            BoudRateSend[1] = 0x10;
            BoudRateSend[2] = 0x00;
            BoudRateSend[3] = 0x90;
            BoudRateSend[4] = 0x00;
            BoudRateSend[5] = 0x02;
            BoudRateSend[6] = 0x04;
            BoudRateSend[7] = (byte)(BoudRate >> 24);
            BoudRateSend[8] = (byte)(BoudRate >> 16);
            BoudRateSend[9] = (byte)(BoudRate >> 8);
            BoudRateSend[10] = (byte) BoudRate;
            BoudRateSend[11] = CalCrc16_CRCHi(BoudRateSend, 11);
            BoudRateSend[12] = CalCrc16_CRCLo(BoudRateSend, 11);
            return BoudRateSend;
        }

        #endregion

        #region 车软停

        /// <summary>
        ///     车软停
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] SoftStop(byte address)
        {
            var softStop = new byte[8];
            softStop[0] = address;
            softStop[1] = 0x06;
            softStop[2] = 0x00;
            softStop[3] = 0x40;
            softStop[4] = 0x00;
            softStop[5] = 0x00;
            softStop[6] = CalCrc16_CRCHi(softStop, 6);
            softStop[7] = CalCrc16_CRCLo(softStop, 6);
            return softStop;
        }

        #endregion

        #region 车急停

        /// <summary>
        ///     车急停
        /// </summary>
        /// <param name="address"></param>
        public static byte[] EmergeStop(byte address)
        {
            var emergeStop = new byte[8];
            emergeStop[0] = address;
            emergeStop[1] = 0x06;
            emergeStop[2] = 0x00;
            emergeStop[3] = 0x40;
            emergeStop[4] = 0x00;
            emergeStop[5] = 0x01;
            emergeStop[6] = CalCrc16_CRCHi(emergeStop, 6);
            emergeStop[7] = CalCrc16_CRCLo(emergeStop, 6);
            return emergeStop;
        }

        #endregion

        #region CRC校验

        /// <summary>
        ///     CRC校验
        /// </summary>
        /// <param name="pBuf"></param>
        /// <param name="dwlen"></param>
        /// <returns></returns>
        private static byte CalCrc16_CRCLo(byte[] pBuf, int dwlen)
        {
            byte bCRCLo = 0xFF, bCRCHi = 0xFF;
            int index, i;

            for (i = 0; i < dwlen; i++)
            {
                index = bCRCHi ^ pBuf[i];
                bCRCHi = (byte)(bCRCLo ^ auchCRCHi[index]);
                bCRCLo = auchCRCLo[index];
            }
            return bCRCLo;
        }

        private static byte CalCrc16_CRCHi(byte[] pBuf, int dwlen)
        {
            byte bCRCLo = 0xFF, bCRCHi = 0xFF;
            int index, i;

            for (i = 0; i < dwlen; i++)
            {
                index = bCRCHi ^ pBuf[i];
                bCRCHi = (byte)(bCRCLo ^ auchCRCHi[index]);
                bCRCLo = auchCRCLo[index];
            }
            return bCRCHi;
        }

        private static readonly byte[] auchCRCHi = new byte[256]
        {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
        };

        private static readonly byte[] auchCRCLo = new byte[256]
        {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
            0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
            0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
            0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
            0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
            0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
            0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
            0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
            0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
            0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
            0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
            0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
            0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
            0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
            0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
            0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
            0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
            0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };

        #endregion
    }
}
