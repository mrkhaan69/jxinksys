using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JxInkSys
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [Serializable()]

    struct CommParameter
    {
        public byte ClientAdder;    //通讯从机地址
        public byte CommandText;    //发送命令
        public byte DataLen;        //数据长度

        public short np1;			    //设定的负压值1。
        public short np2;			    //设定的负压值2。
        public short np3;			    //设定的负压值3。
        public short np4;			    //设定的负压值4。
        public short npu;			    //设定的压墨值。

        public ushort PressTime;		//控制压墨时间。

        public ushort M_Low1;           //负压泵低速转速1。
        public ushort M_Low2;           //负压泵低速转速2。
        public ushort M_Low3;           //负压泵低速转速3。
        public ushort M_Low4;           //负压泵低速转速4。

        public ushort Temperature0;	//设定的第一路温度。
        public ushort Temperature1;	//设定的第二路温度。
        public ushort Temperature2;	//设定的第三路温度。
        public ushort Temperature3;	//设定的第四路温度。

        public ushort AddInkMotor;	//加墨泵转速。

        public ushort EnableAirAdjust;	//是否屏蔽负压调节。
        public ushort EnableAddInk;		//是否屏蔽加墨调节。
        public ushort EnableTempAdjust;	//是否屏蔽加热功能。
        public ushort EnablePressInk;	//是否屏蔽压墨功能。
        public ushort InkSensorStatic;	//液位传感器常开常闭选项。 1 常开 0 常闭
        public ushort EnableIndex;		//启用或者屏蔽那一路索引。

        public ushort DataCRC;      //数据校验
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [Serializable()]
    struct CommReadAir
    {
        public byte ClientAdder;    //通讯从机地址
        public byte CommandText;    //发送命令
        public byte DataLen;        //数据长度

        public short CurrNp1;       //当前负压值1。
        public short CurrNp2;       //当前负压值2。
        public short CurrNp3;       //当前负压值3。
        public short CurrNp4;       //当前负压值4。
        public short PCurrAirs;     //当前正压值。
        public ushort CurrTemp1;	//当前的温度值1。
        public ushort CurrTemp2;	//当前的温度值1。
        public ushort CurrTemp3;	//当前的温度值1。
        public ushort CurrTemp4;	//当前的温度值1。
        public ushort M_InkM_RunT1; //上一分钟墨泵工作时间1
        public ushort M_InkM_RunT2; //上一分钟墨泵工作时间2
        public ushort M_InkM_RunT3; //上一分钟墨泵工作时间3
        public ushort M_InkM_RunT4; //上一分钟墨泵工作时间4
        public ushort CurrInkS;		//当前的液位传感器。

        public ushort DataCRC;      //数据校验
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    [Serializable()]
    struct SendCommParameter
    {
        public ushort np1;			    //设定的负压值1。
        public ushort np2;			    //设定的负压值2。
        public ushort np3;			    //设定的负压值3。
        public ushort np4;			    //设定的负压值4。
        public ushort npu;			    //设定的压墨值。

        public ushort PressTime;		//控制压墨时间。

        public ushort M_Low1;           //负压泵低速转速1。
        public ushort M_Low2;           //负压泵低速转速2。
        public ushort M_Low3;           //负压泵低速转速3。
        public ushort M_Low4;           //负压泵低速转速4。

        public ushort Temperature0;	//设定的第一路温度。
        public ushort Temperature1;	//设定的第二路温度。
        public ushort Temperature2;	//设定的第三路温度。
        public ushort Temperature3;	//设定的第四路温度。

        public ushort AddInkMotor;	//加墨泵转速。

        public ushort EnableAirAdjust;	//是否屏蔽负压调节。
        public ushort EnableAddInk;		//是否屏蔽加墨调节。
        public ushort EnableTempAdjust;	//是否屏蔽加热功能。
        public ushort EnablePressInk;	//是否屏蔽压墨功能。
        public ushort InkSensorStatic;	//液位传感器常开常闭选项。 1 常开 0 常闭
        public ushort EnableIndex;		//启用或者屏蔽那一路索引。

    }

    public class Struct_Transform
    {
        //struct转换为byte[]
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        //byte[]转换为struct
        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static ushort GetCrcData(byte[] buffer, int len)
        {
            byte CRC_L;							//定义数组
            byte CRC_H;
            int i = 0, j = 0;				//计数

            ushort crc = 0xFFFF;

            for (i = 0; i < len; i++)
            {
                crc = (ushort)(crc ^ (buffer[i]));
                for (j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                }
            }

            CRC_L = (byte)(crc & 0xFF);				//crc的低八位
            CRC_H = (byte)(crc >> 8);				//crc的高八位
            return (ushort)((CRC_L << 8) | CRC_H);

        }
    }
}
