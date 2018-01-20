// 本程序演示有限采集不重读数据过程
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sys
{
    class Program
    {
        [DllImport("msvcrt.dll")]
        public static extern int _getch();
        [DllImport("msvcrt.dll")]
        public static extern int _kbhit();
        public const int WAIT_OBJECT_0 = 0;
        [DllImport("Kernel32.dll")]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMillisenconds);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateEvent(String lpEventAttributes, bool bManualReset, bool bInitialState, String lpName);
        [DllImport("Ws2_32.dll")]
        public static extern int inet_addr(string ipaddr);

        public static UserDef.CFGPARA CfgPara;
        public static NET2991.NET2991_AI_STATUS Sts;
        static void Main(string[] args)
        {
            CfgPara.AIParam.szDevName = new SByte[32];
            CfgPara.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            Int32 nIndex = 0;
            Int32 dwChanCnt = 0;

            Console.WriteLine("此例程为有限模式不重读数据采集过程,按任意键继续...");
            _getch();

            NET2991.NET2991_DEV_Init(4096);

            // 输入设备IP
            Console.WriteLine("请输入设备IP:");
            string strIP = Console.ReadLine();

            // 输入设备端口号
            Console.WriteLine("请输入设备端口号:");
            string strDevPort = Console.ReadLine();

            // 输入本地端口号
            Console.WriteLine("请输入本地端口号:");
            string strLocalPort = Console.ReadLine();

            for (nIndex = 0; nIndex < 32; nIndex++)
            {
                CfgPara.AIParam.szDevName[nIndex] = 0;
            }

            Int32 ipaddr = inet_addr(strIP);
            CfgPara.AIParam.nDeviceIP = (UInt32)(System.Net.IPAddress.NetworkToHostOrder(ipaddr));
            CfgPara.AIParam.nDevicePort = Convert.ToUInt16(strDevPort);
            CfgPara.AIParam.nLocalPort = Convert.ToUInt16(strLocalPort);

            for (nIndex = 0; nIndex < 17; nIndex++)
            {
                CfgPara.AIParam.CHParam[nIndex].bChannelEn = 1;
                CfgPara.AIParam.CHParam[nIndex].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara.AIParam.CHParam[nIndex].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
            }

            CfgPara.AIParam.fSampleRate = 1000000;
            CfgPara.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_FINITE;
            CfgPara.AIParam.nSampsPerChan = 10240000;
            CfgPara.AIParam.nClockSource = NET2991.NET2991_AI_CLOCKSRC_LOCAL;
            CfgPara.AIParam.nReserved0 = 0;

            CfgPara.AIParam.nTriggerSource = NET2991.NET2991_AI_TRIGSRC_NONE;
            CfgPara.AIParam.nTriggerDir = NET2991.NET2991_AI_TRIGDIR_FALLING;
            CfgPara.AIParam.fTriggerLevel = 10;
            CfgPara.AIParam.nDelaySamps = 0;
            CfgPara.AIParam.nReTriggerCount = 1;

            CfgPara.AIParam.bMasterEn = 0;
            CfgPara.AIParam.nReserved1 = 0;
            CfgPara.AIParam.nReserved2 = 0;

            CfgPara.nReadOffset = 0;
            CfgPara.nReadLength = 10240000;
            CfgPara.hDevice = (IntPtr)(-1);

            UInt32 bReRead = 0;
            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;

            // 创建设备
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP,
                                                                                               CfgPara.AIParam.nDevicePort,
                                                                                               CfgPara.AIParam.nLocalPort,
                                                                                               nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                Console.WriteLine("创建设备失败!");
                goto ExitRead;
            }
            // 判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice) == 0)
            {
                Console.WriteLine("设备没有连接!");
                goto ExitRead;
            }

            // 设置读取偏移和读取长度
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara.hDevice, CfgPara.nReadOffset, CfgPara.nReadLength);

            // 设置是否重读
            UInt32 nLastSamps = 0;
            if (NET2991.NET2991_AI_SetReReadData(CfgPara.hDevice, ref CfgPara.AIParam, ref nLastSamps, bReRead) == 0)
            {
                Console.WriteLine("设置重读失败!");
                goto ExitRead;
            }

            // 是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice, 0) == 0)
            {
                Console.WriteLine("设置文件保存失败!");
                goto ExitRead;
            }

            // 初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1)) == 0)
            {
                Console.WriteLine("初始化设备失败!");
                goto ExitRead;
            }

            if (NET2991.NET2991_AI_GetClockStatus(CfgPara.hDevice, ref Sts) == 0)
            {
                Console.WriteLine("获取时钟状态失败!");
                goto ExitRead;
            }

            dwChanCnt = 0;
            for (nIndex = 0; nIndex < NET2991.NET2991_MAX_CHANNELS; nIndex++)
            {
                if (CfgPara.AIParam.CHParam[nIndex].bChannelEn == 1)
                {
                    dwChanCnt++;
                }
            }

            // 实际总共需要读取的数据数
            CfgPara.dwRealReadLen = 2 * dwChanCnt * CfgPara.nReadLength;

            // 初始化
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;

            // 读取数据的长度
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;

            // 清除缓存
            NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice);

            // 启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice) == 0)
            {
                Console.WriteLine("启动任务失败!");
                goto ExitRead;
            }

            // 状态
            UInt32 dwStsCnt = 0;
            while (true)
            {
                Console.WriteLine("得到状态中{0}", dwStsCnt);
                dwStsCnt++;
                if (_kbhit() != 0)
                    goto ExitRead;
                NET2991.NET2991_AI_SendSoftTrig(CfgPara.hDevice);
                Thread.Sleep(1);
                // 得到状态
                if (NET2991.NET2991_AI_GetStatus(CfgPara.hDevice, ref Sts) == 0)
                {
                    Console.WriteLine("得到状态失败!");
                    goto ExitRead;
                }
                if (Sts.bTaskDone == 1)
                {
                    Console.WriteLine("得到状态");
                    CfgPara.bAIStatus = 1;
                    break;
                }
                Thread.Sleep(80);
            }
            Thread tDataRead = new Thread(ReadDataFun);
            tDataRead.Start();
            return;

        ExitRead:
            Console.WriteLine("退出中....,按任意键退出");
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            _getch();
        }

        static void ReadDataFun()
        {
            UInt16[] nAIArray = new UInt16[2048];
            UInt32 dwReadChan = 0;
            UInt32 dwReadSampsPerChan = 0;
            UInt32 dwSampsPerChanRead = 0;
            UInt32 dwAvailSampsPerChan = 0;
            UInt32 dwTotalDataSize = 0;
            double fTimeOut = -1.0;
            Int32 nIndex = 0;
            // 根据用户设置的读取长度设置每通道数据的读取长度
            UInt32 nChanSize = (uint)(CfgPara.nReadLength * 2);
            // 读数据
            dwTotalDataSize = 0;
            UInt32 bFirst = 1;
            UInt32 bFirstSum = 1;
            while (true)
            {
                if (_kbhit() != 0)
                    break;
                dwReadSampsPerChan = 0;

                // 读数据,dwReadChan为返回的通道号(0~16)
                // 数据返回顺序是先将第16通道(即DI通道)数据全部返回，再一次返回CH0 CH1 CH2.......CH15
                // 正常情况下每次读取返回的dwSampsPerChanRead为640
                if (NET2991.NET2991_AI_ReadBinary(CfgPara.hDevice, ref dwReadChan, nAIArray, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut) == 0)
                {
                    if (_kbhit() != 0)
                        break;
                    continue;
                }
                if (_kbhit() != 0)
                    break;

                dwTotalDataSize += (dwSampsPerChanRead * 2);
                if (bFirst == 1)
                {
                    Console.WriteLine("DI通道数据:");
                    bFirst = 0;
                }
                // 这里数据只显示了每次读取的640个中的前6个数据的原码值
                for (nIndex = 0; nIndex < 6; nIndex++)
                {
                    Console.Write(" {0} ", nAIArray[nIndex]);
                }
                Console.WriteLine("");
                if ((dwTotalDataSize > 0) && (dwTotalDataSize % nChanSize == 0))
                {
                    if (bFirstSum == 1)
                    {
                        Console.WriteLine("DI通道,数据累计大小:{0}", dwTotalDataSize);
                        Console.WriteLine("通道{0}数据:");
                    }
                    else
                    {
                        Console.WriteLine("通道号{0},数据累计大小:{1}", dwReadChan, dwTotalDataSize);
                    }
                    bFirstSum = 0;
                }
                if (dwTotalDataSize >= CfgPara.dwRealReadLen)
                {
                    NET2991.NET2991_AI_SetReadComplete(CfgPara.hDevice);
                    Console.WriteLine("数据读取完成");
                    if (CfgPara.hDevice != (IntPtr)(-1))
                    {
                        NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                        NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                        CfgPara.hDevice = (IntPtr)(-1);
                    }
                    break;
                }
            }
            Console.WriteLine("退出中....,按任意键退出");
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            _getch();
        }
    }
}
