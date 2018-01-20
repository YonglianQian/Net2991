// ��������ʾ���޲ɼ����ض����ݹ���
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

            Console.WriteLine("������Ϊ����ģʽ���ض����ݲɼ�����,�����������...");
            _getch();

            NET2991.NET2991_DEV_Init(4096);

            // �����豸IP
            Console.WriteLine("�������豸IP:");
            string strIP = Console.ReadLine();

            // �����豸�˿ں�
            Console.WriteLine("�������豸�˿ں�:");
            string strDevPort = Console.ReadLine();

            // ���뱾�ض˿ں�
            Console.WriteLine("�����뱾�ض˿ں�:");
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

            // �����豸
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP,
                                                                                               CfgPara.AIParam.nDevicePort,
                                                                                               CfgPara.AIParam.nLocalPort,
                                                                                               nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                Console.WriteLine("�����豸ʧ��!");
                goto ExitRead;
            }
            // �ж��Ƿ�����
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice) == 0)
            {
                Console.WriteLine("�豸û������!");
                goto ExitRead;
            }

            // ���ö�ȡƫ�ƺͶ�ȡ����
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara.hDevice, CfgPara.nReadOffset, CfgPara.nReadLength);

            // �����Ƿ��ض�
            UInt32 nLastSamps = 0;
            if (NET2991.NET2991_AI_SetReReadData(CfgPara.hDevice, ref CfgPara.AIParam, ref nLastSamps, bReRead) == 0)
            {
                Console.WriteLine("�����ض�ʧ��!");
                goto ExitRead;
            }

            // �Ƿ񱣴��ļ�
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice, 0) == 0)
            {
                Console.WriteLine("�����ļ�����ʧ��!");
                goto ExitRead;
            }

            // ��ʼ���豸
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1)) == 0)
            {
                Console.WriteLine("��ʼ���豸ʧ��!");
                goto ExitRead;
            }

            if (NET2991.NET2991_AI_GetClockStatus(CfgPara.hDevice, ref Sts) == 0)
            {
                Console.WriteLine("��ȡʱ��״̬ʧ��!");
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

            // ʵ���ܹ���Ҫ��ȡ��������
            CfgPara.dwRealReadLen = 2 * dwChanCnt * CfgPara.nReadLength;

            // ��ʼ��
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;

            // ��ȡ���ݵĳ���
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;

            // �������
            NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice);

            // ��������
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice) == 0)
            {
                Console.WriteLine("��������ʧ��!");
                goto ExitRead;
            }

            // ״̬
            UInt32 dwStsCnt = 0;
            while (true)
            {
                Console.WriteLine("�õ�״̬��{0}", dwStsCnt);
                dwStsCnt++;
                if (_kbhit() != 0)
                    goto ExitRead;
                NET2991.NET2991_AI_SendSoftTrig(CfgPara.hDevice);
                Thread.Sleep(1);
                // �õ�״̬
                if (NET2991.NET2991_AI_GetStatus(CfgPara.hDevice, ref Sts) == 0)
                {
                    Console.WriteLine("�õ�״̬ʧ��!");
                    goto ExitRead;
                }
                if (Sts.bTaskDone == 1)
                {
                    Console.WriteLine("�õ�״̬");
                    CfgPara.bAIStatus = 1;
                    break;
                }
                Thread.Sleep(80);
            }
            Thread tDataRead = new Thread(ReadDataFun);
            tDataRead.Start();
            return;

        ExitRead:
            Console.WriteLine("�˳���....,��������˳�");
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
            // �����û����õĶ�ȡ��������ÿͨ�����ݵĶ�ȡ����
            UInt32 nChanSize = (uint)(CfgPara.nReadLength * 2);
            // ������
            dwTotalDataSize = 0;
            UInt32 bFirst = 1;
            UInt32 bFirstSum = 1;
            while (true)
            {
                if (_kbhit() != 0)
                    break;
                dwReadSampsPerChan = 0;

                // ������,dwReadChanΪ���ص�ͨ����(0~16)
                // ���ݷ���˳�����Ƚ���16ͨ��(��DIͨ��)����ȫ�����أ���һ�η���CH0 CH1 CH2.......CH15
                // ���������ÿ�ζ�ȡ���ص�dwSampsPerChanReadΪ640
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
                    Console.WriteLine("DIͨ������:");
                    bFirst = 0;
                }
                // ��������ֻ��ʾ��ÿ�ζ�ȡ��640���е�ǰ6�����ݵ�ԭ��ֵ
                for (nIndex = 0; nIndex < 6; nIndex++)
                {
                    Console.Write(" {0} ", nAIArray[nIndex]);
                }
                Console.WriteLine("");
                if ((dwTotalDataSize > 0) && (dwTotalDataSize % nChanSize == 0))
                {
                    if (bFirstSum == 1)
                    {
                        Console.WriteLine("DIͨ��,�����ۼƴ�С:{0}", dwTotalDataSize);
                        Console.WriteLine("ͨ��{0}����:");
                    }
                    else
                    {
                        Console.WriteLine("ͨ����{0},�����ۼƴ�С:{1}", dwReadChan, dwTotalDataSize);
                    }
                    bFirstSum = 0;
                }
                if (dwTotalDataSize >= CfgPara.dwRealReadLen)
                {
                    NET2991.NET2991_AI_SetReadComplete(CfgPara.hDevice);
                    Console.WriteLine("���ݶ�ȡ���");
                    if (CfgPara.hDevice != (IntPtr)(-1))
                    {
                        NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                        NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                        CfgPara.hDevice = (IntPtr)(-1);
                    }
                    break;
                }
            }
            Console.WriteLine("�˳���....,��������˳�");
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
