using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using Frm.Common;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraSplashScreen;
using System.IO;
using System.Configuration;

namespace Frm.Module
{
    
    public partial class Test : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public Test()
        {
            InitializeComponent();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            //设置数据采集卡参数
            InitSettings();

            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;

            CfgPara1.hDevice = NET2991.NET2991_DEV_Create(CfgPara1.AIParam.nDeviceIP,
                                                                                               CfgPara1.AIParam.nDevicePort,
                                                                                               CfgPara1.AIParam.nLocalPort,
                                                                                               nDataTranDir, 0.2, 0.2, 2);
            CfgPara2.hDevice = NET2991.NET2991_DEV_Create(CfgPara2.AIParam.nDeviceIP,
                                                                                               CfgPara2.AIParam.nDevicePort,
                                                                                               CfgPara2.AIParam.nLocalPort,
                                                                                               nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara1.hDevice==(IntPtr)(-1)||CfgPara2.hDevice==(IntPtr)(-1))
            {
                XtraMessageBox.Show("创建设备失败");
                goto ExitRead;
            }
            // 判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara1.hDevice) == 0||NET2991.NET2991_DEV_IsLink(CfgPara2.hDevice)==0)
            {
                XtraMessageBox.Show("设备没有连接!");
                goto ExitRead;
            }

            // 设置读取偏移和读取长度
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara1.hDevice, CfgPara1.nReadOffset, CfgPara1.nReadLength);
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara2.hDevice, CfgPara2.nReadOffset, CfgPara2.nReadLength);

            // 设置是否重读
            UInt32 nLastSamps = 0;
            if (NET2991.NET2991_AI_SetReReadData(CfgPara1.hDevice, ref CfgPara1.AIParam, ref nLastSamps, 0) == 0|| NET2991.NET2991_AI_SetReReadData(CfgPara2.hDevice, ref CfgPara2.AIParam, ref nLastSamps, 0) == 0)
            {
                XtraMessageBox.Show("设置重读失败!");
                goto ExitRead;
            }

            // 是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara1.hDevice, 0) == 0||NET2991.NET2991_AI_IsSaveFile(CfgPara2.hDevice,0)==0)
            {
                XtraMessageBox.Show("设置文件保存失败!");
                goto ExitRead;
            }

            // 初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara1.hDevice, ref CfgPara1.AIParam, (IntPtr)(-1)) == 0||NET2991.NET2991_AI_InitTask(CfgPara2.hDevice,ref CfgPara2.AIParam,(IntPtr)(-1))==0)
            {
                XtraMessageBox.Show("初始化设备失败!");
                goto ExitRead;
            }

            if (NET2991.NET2991_AI_GetClockStatus(CfgPara1.hDevice, ref Sts) == 0||NET2991.NET2991_AI_GetClockStatus(CfgPara2.hDevice,ref Sts1)==0)
            {
                XtraMessageBox.Show("获取时钟状态失败!");
                goto ExitRead;
            }


            CfgPara1.dwRealReadLen = 2 * 16* CfgPara1.nReadLength;
            CfgPara2.dwRealReadLen = 2 * 16 * CfgPara2.nReadLength;

            CfgPara1.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara1.dwReadDataSize = 0;
            CfgPara1.bAIStatus = 0;
            CfgPara1.dwFrameLen = UserDef.LMT_FRMCNT;
            NET2991.NET2991_AI_ClearBuffer(CfgPara1.hDevice);


            CfgPara2.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara2.dwReadDataSize = 0;
            CfgPara2.bAIStatus = 0;
            CfgPara2.dwFrameLen = UserDef.LMT_FRMCNT;
            NET2991.NET2991_AI_ClearBuffer(CfgPara2.hDevice);

            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara1.hDevice)==0||NET2991.NET2991_AI_StartTask(CfgPara2.hDevice)==0)
            {
                XtraMessageBox.Show("启动任务失败");
                goto ExitRead;
            }

            while (true)
            {
                NET2991.NET2991_AI_SendSoftTrig(CfgPara1.hDevice);
                NET2991.NET2991_AI_SendSoftTrig(CfgPara2.hDevice);
                Thread.Sleep(1);
                if (NET2991.NET2991_AI_GetStatus(CfgPara1.hDevice,ref Sts)==0||NET2991.NET2991_AI_GetStatus(CfgPara2.hDevice,ref Sts1)==0)
                {
                    XtraMessageBox.Show("得到状态失败");
                    goto ExitRead;
                }
                if (Sts.bTaskDone==1&&Sts1.bTaskDone==1)
                {
                    XtraMessageBox.Show("得到状态成功");
                    CfgPara1.bAIStatus = 1;
                    CfgPara2.bAIStatus = 1;
                    break;
                }
                Thread.Sleep(80);
            }
            
            //异步方法执行采集数据任务
            Start_MyTask();
            return;



        ExitRead:
            XtraMessageBox.Show("退出中....");
            if (CfgPara1.hDevice != (IntPtr)(-1)||CfgPara2.hDevice!=(IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                CfgPara1.hDevice = (IntPtr)(-1);
                NET2991.NET2991_AI_StopTask(CfgPara2.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara2.hDevice);
                CfgPara2.hDevice = (IntPtr)(-1);
            }
        }

        //同步采样数据处理异步方法
       async void Start_MyTask()
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            await Task.Run(() =>
            {
            UInt16[] nAIArray1 = new ushort[2048];
            UInt16[] nAIArray2 = new ushort[2048];
            UInt32 dwReadChan1 = 0;
            UInt32 dwReadChan2 = 0;
            UInt32 dwReadSampsPerChan = 0;
            UInt32 dwSampsPerChanRead = 0;
            UInt32 dwAvailSampsPerChan = 0;
            UInt32 dwTotalDataSize = 0;
            double fTimeOut =-1.0;
            UInt32 nChanSize = (uint)(CfgPara1.nReadLength * 2);//每通道数据读取长度

                
            FileStream fs = new FileStream(@"C:\1.dat", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
                while (true)
                {
                    dwReadSampsPerChan = 0;
                    if (NET2991.NET2991_AI_ReadBinary(CfgPara1.hDevice, ref dwReadChan1, nAIArray1, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut) == 0)
                    {
                        continue;
                    }

                    dwTotalDataSize += (dwSampsPerChanRead * 2);
                    for (int j = 0; j < dwSampsPerChanRead; j++)
                    {
                        bw.Write(Convert.ToSingle((nAIArray1[j] & 0xFFFF) * (20000.00 / 65536) - 10000.00));
                    }
                    if (dwTotalDataSize >= CfgPara1.dwRealReadLen)
                    {

                        NET2991.NET2991_AI_SetReadComplete(CfgPara1.hDevice);
                            break;
                    }
                }
                dwTotalDataSize = 0;
                while (true)
                {
                    dwReadSampsPerChan = 0;
                    if (NET2991.NET2991_AI_ReadBinary(CfgPara2.hDevice, ref dwReadChan2, nAIArray2, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut) == 0)
                    {
                        continue;
                    }
                    dwTotalDataSize += (dwSampsPerChanRead * 2);

                    for (int i = 0; i < dwSampsPerChanRead; i++)
                    {
                        bw.Write(Convert.ToSingle((nAIArray2[i] & 0xFFFF) * (20000.00 / 65536) - 10000.00));
                    }
                        
                    if (dwTotalDataSize>=CfgPara2.dwRealReadLen)
                    {
                        NET2991.NET2991_AI_SetReadComplete(CfgPara2.hDevice);
                        XtraMessageBox.Show("文件保存完毕");
                        if (CfgPara2.hDevice != (IntPtr)(-1)||CfgPara1.hDevice!=(IntPtr)(-1))
                        {
                            NET2991.NET2991_AI_StopTask(CfgPara2.hDevice);
                            NET2991.NET2991_DEV_Release(CfgPara2.hDevice);
                            CfgPara2.hDevice = (IntPtr)(-1);
                            NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                            NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                            CfgPara1.hDevice = (IntPtr)(-1);
                        }
                        break;
                    }


                }
                
                bw.Close();
                fs.Close();

                if (CfgPara1.hDevice != (IntPtr)(-1) || CfgPara2.hDevice != (IntPtr)(-1))
                {
                    NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                    NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                    CfgPara1.hDevice = (IntPtr)(-1);
                    NET2991.NET2991_AI_StopTask(CfgPara2.hDevice);
                    NET2991.NET2991_DEV_Release(CfgPara2.hDevice);
                    CfgPara2.hDevice = (IntPtr)(-1);
                }

            });
            SplashScreenManager.CloseForm();
        }



        [DllImport("Ws2_32.dll")]
        public static extern int inet_addr(string ipaddr);
        public static UserDef.CFGPARA CfgPara1;
        public static NET2991.NET2991_AI_STATUS Sts;
        public static UserDef.CFGPARA CfgPara2;
        public static NET2991.NET2991_AI_STATUS Sts1;
        protected void InitSettings()
        {
            CfgPara1.AIParam.szDevName = new sbyte[32];
            CfgPara1.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            NET2991.NET2991_DEV_Init(4096);
            for (int i = 0; i < 32; i++)
            {
                CfgPara1.AIParam.szDevName[i] = 0;
            }
            CfgPara1.AIParam.nDeviceIP = (UInt32)(System.Net.IPAddress.NetworkToHostOrder(inet_addr("192.168.0.150")));
            CfgPara1.AIParam.nDevicePort = 9000;
            CfgPara1.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {
                if (i==16)
                {
                    CfgPara1.AIParam.CHParam[i].bChannelEn = 0;
                }
                else
                {
                CfgPara1.AIParam.CHParam[i].bChannelEn = 1;
                CfgPara1.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara1.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
                }
            }
            CfgPara1.AIParam.fSampleRate = 1000000;
            CfgPara1.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_FINITE;
            CfgPara1.AIParam.nSampsPerChan = 1024;
            CfgPara1.AIParam.nClockSource = NET2991.NET2991_AI_CLOCKSRC_LOCAL;
            CfgPara1.AIParam.nReserved0 = 0;

            CfgPara1.AIParam.nTriggerSource = NET2991.NET2991_AI_TRIGSRC_ANALOG;
            CfgPara1.AIParam.nTriggerDir = NET2991.NET2991_AI_TRIGDIR_FALLING;
            CfgPara1.AIParam.fTriggerLevel = 0; //触发电平
            CfgPara1.AIParam.nDelaySamps = 0;
            CfgPara1.AIParam.nReTriggerCount = 1;

            CfgPara1.AIParam.bMasterEn = 1;
            CfgPara1.AIParam.nReserved1 = 0;
            CfgPara1.AIParam.nReserved2 = 0;

            CfgPara1.nReadOffset = 0;
            CfgPara1.nReadLength = 1024;
            CfgPara1.hDevice = (IntPtr)(-1);


            //设置第二块卡参数
            CfgPara2.AIParam.szDevName = new sbyte[32];
            CfgPara2.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            //NET2991.NET2991_DEV_Init(4096);
            for (int i = 0; i < 32; i++)
            {
                if (i==16)
                {
                    CfgPara2.AIParam.szDevName[i] = 1;
                }
                else
                {
                    CfgPara2.AIParam.szDevName[i] = 0;
                }
            }
            CfgPara2.AIParam.nDeviceIP = (UInt32)(System.Net.IPAddress.NetworkToHostOrder(inet_addr("192.168.0.151")));
            CfgPara2.AIParam.nDevicePort = 9001;
            CfgPara2.AIParam.nLocalPort = 8001;
            for (int i = 0; i < 17; i++)
            {
                if (i==16)
                {
                    CfgPara2.AIParam.CHParam[i].bChannelEn = 0;
                }
                else
                {
                    CfgPara2.AIParam.CHParam[i].bChannelEn = 1;
                    CfgPara2.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                    CfgPara2.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
                }
                
            }
            CfgPara2.AIParam.fSampleRate = 1000000;
            CfgPara2.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_FINITE;
            CfgPara2.AIParam.nSampsPerChan = 1024;
            CfgPara2.AIParam.nClockSource = NET2991.NET2991_AI_CLOCKSRC_CLKIN_10M;
            CfgPara2.AIParam.nReserved0 = 0;

            CfgPara2.AIParam.nTriggerSource = NET2991.NET2991_AI_TRIGSRC_ANALOG;
            CfgPara2.AIParam.nTriggerDir = NET2991.NET2991_AI_TRIGDIR_FALLING;
            CfgPara2.AIParam.fTriggerLevel = 0; //触发电平
            CfgPara2.AIParam.nDelaySamps = 0;
            CfgPara2.AIParam.nReTriggerCount = 1;

            CfgPara2.AIParam.bMasterEn = 0;
            CfgPara2.AIParam.nReserved1 = 0;
            CfgPara2.AIParam.nReserved2 = 0;

            CfgPara2.nReadOffset = 0;
            CfgPara2.nReadLength = 1024;
            CfgPara2.hDevice = (IntPtr)(-1);
        }

        //读取两张卡数据文件并显示
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable dt = new DataTable();
            DataColumn[] dcArray = new DataColumn[1025];
            for (int i = 0; i < dcArray.Length; i++)
            {
                dcArray[i] = new DataColumn();
                if (i==0)
                {
                    dcArray[i].ColumnName = "通道号";
                }
                else
                {
                    dcArray[i].ColumnName = "第" + i + "个采样点";
                }
            }
            dt.Columns.AddRange(dcArray);
            FileStream fs = new FileStream(@"C:\1.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            for (int i = 0; i < 32; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "通道" + i;
                for (int j = 1; j< 1025; j++)
                {
                    dr[j] = br.ReadSingle();
                }
                dt.Rows.Add(dr);
            }
            br.Close();
            fs.Close();
            this.vGridControl1.DataSource = dt;
        }

        //单卡有限采集保存文件
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            InitOneCardSettings();
            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;
            //创建设备
            CfgPara1.hDevice = NET2991.NET2991_DEV_Create(CfgPara1.AIParam.nDeviceIP, CfgPara1.AIParam.nDevicePort, CfgPara1.AIParam.nLocalPort, nDataTranDir, 0.2, 0.2, 2);

            //配置参数
            UInt32 nLastSamps = 0;
            bool flag = CfgPara1.hDevice == (IntPtr)(-1)||NET2991.NET2991_DEV_IsLink(CfgPara1.hDevice)==0||NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara1.hDevice,CfgPara1.nReadOffset,CfgPara1.nReadLength)==0||NET2991.NET2991_AI_SetReReadData(CfgPara1.hDevice,ref CfgPara1.AIParam,ref nLastSamps,0)==0||NET2991.NET2991_AI_IsSaveFile(CfgPara1.hDevice,0)==0||NET2991.NET2991_AI_InitTask(CfgPara1.hDevice,ref CfgPara1.AIParam,(IntPtr)(-1))==0||NET2991.NET2991_AI_GetClockStatus(CfgPara1.hDevice,ref Sts)==0;

            if (flag)
            {
                XtraMessageBox.Show("发生了错误","出错提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                goto ExitRead;
            }

            //实际总共需要读取的数据数
            CfgPara1.dwRealReadLen = 2 * 16 * CfgPara1.nReadLength;
            //初始化
            CfgPara1.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara1.dwReadDataSize = 0;
            CfgPara1.bAIStatus = 0;
            CfgPara1.dwFrameLen = UserDef.LMT_FRMCNT;
            NET2991.NET2991_AI_ClearBuffer(CfgPara1.hDevice);
            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara1.hDevice)==0)
            {
                XtraMessageBox.Show("启动采样任务失败");
                goto ExitRead;
            }
            //状态获取
            while (true)
            {
                NET2991.NET2991_AI_SendSoftTrig(CfgPara1.hDevice);
                Thread.Sleep(1);
                //得到状态
                if (NET2991.NET2991_AI_GetStatus(CfgPara1.hDevice,ref Sts)==0)
                {
                    XtraMessageBox.Show("获取状态失败");
                    goto ExitRead;
                }
                if (Sts.bTaskDone == 1)
                {
                    XtraMessageBox.Show("得到状态");
                    CfgPara1.bAIStatus = 1;
                    break;
                }
                Thread.Sleep(80);
            }
            Start_MyInfiniteNoSaveFile();
            return;
            
            ExitRead:
            if (CfgPara1.hDevice!=(IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                CfgPara1.hDevice = (IntPtr)(-1);
            }

                
        }
        
        async void Start_MyInfiniteNoSaveFile()
        {
            await Task.Run(() =>
            {
                UInt16[] nAIArray = new ushort[2048];
                UInt32 dwReadChan = 0;
                UInt32 dwReadSampsPerChan = 0;
                UInt32 dwSampsPerChanRead = 0;
                UInt32 dwAvailSampsPerChan = 0;
                UInt32 dwTotalDataSize = 0;
                double fTimeOut = -1.0;
                Int32 nIndex = 0;
                //根据用户设置的读取长度设置每通道数据的读取长度
                UInt32 nChanSize = (uint)(CfgPara1.nReadLength * 2);
                //读取数据
                dwTotalDataSize = 0;
                FileStream fs = new FileStream(@"C:\2.dat", FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                while (true)
                {
                    dwReadSampsPerChan = 0;
                    if (NET2991.NET2991_AI_ReadBinary(CfgPara1.hDevice, ref dwReadChan, nAIArray, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut) == 0)
                    {
                        continue;
                    }
                    dwTotalDataSize += (dwSampsPerChanRead * 2);
                    for (int j = 0; j < dwSampsPerChanRead; j++)
                    {
                        bw.Write(Convert.ToSingle((nAIArray[j] & 0xFFFF) * 20000.00 / 65536 - 10000.00));
                    }
                    if (dwTotalDataSize >= CfgPara1.dwRealReadLen)
                    {
                        NET2991.NET2991_AI_SetReadComplete(CfgPara1.hDevice);
                        XtraMessageBox.Show("数据读取已经完成,点击退出");
                        if (CfgPara1.hDevice != (IntPtr)(-1))
                        {
                            NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                            NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                            CfgPara1.hDevice = (IntPtr)(-1);
                        }
                        break;
                    }
                }
                bw.Close();
                fs.Close();
                if (CfgPara1.hDevice != (IntPtr)(-1))
                {
                    NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                    NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                    CfgPara1.hDevice = (IntPtr)(-1);
                }

            });
        }
        void InitOneCardSettings()
        {
            CfgPara1.AIParam.szDevName = new sbyte[32];
            CfgPara1.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            NET2991.NET2991_DEV_Init(4096);
            for (int i = 0; i < 32; i++)
            {
                CfgPara1.AIParam.szDevName[i] = 0;
            }
            CfgPara1.AIParam.nDeviceIP = (UInt32)(System.Net.IPAddress.NetworkToHostOrder(inet_addr("192.168.0.150")));
            CfgPara1.AIParam.nDevicePort = 9000;
            CfgPara1.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {
                if (i == 16)
                {
                    CfgPara1.AIParam.CHParam[i].bChannelEn = 0;
                }
                else
                {
                    CfgPara1.AIParam.CHParam[i].bChannelEn = 1;
                    CfgPara1.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                    CfgPara1.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
                }
            }
            CfgPara1.AIParam.fSampleRate = 1000000;
            CfgPara1.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_FINITE;
            CfgPara1.AIParam.nSampsPerChan = 1024;
            CfgPara1.AIParam.nClockSource = NET2991.NET2991_AI_CLOCKSRC_LOCAL;
            CfgPara1.AIParam.nReserved0 = 0;

            CfgPara1.AIParam.nTriggerSource = NET2991.NET2991_AI_TRIGSRC_ANALOG;
            CfgPara1.AIParam.nTriggerDir = NET2991.NET2991_AI_TRIGDIR_FALLING;
            CfgPara1.AIParam.fTriggerLevel = 0; //触发电平
            CfgPara1.AIParam.nDelaySamps = 0;
            CfgPara1.AIParam.nReTriggerCount = 1;

            CfgPara1.AIParam.bMasterEn = 1;
            CfgPara1.AIParam.nReserved1 = 0;
            CfgPara1.AIParam.nReserved2 = 0;

            CfgPara1.nReadOffset = 0;
            CfgPara1.nReadLength = 1024;
            CfgPara1.hDevice = (IntPtr)(-1);

        }

        //读取文件并显示
        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable dt = new DataTable();
            DataColumn[] dcArray = new DataColumn[1025];
            for (int i = 0; i < dcArray.Length; i++)
            {
                dcArray[i] = new DataColumn();
                if (i==0)
                {
                    dcArray[i].ColumnName = "通道号";
                }
                else
                {
                    dcArray[i].ColumnName = "第" + i + "个采样点";
                }
            }
            dt.Columns.AddRange(dcArray);
            FileStream fs = new FileStream(@"C:\2.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            for (int i = 0; i < 16; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "第" + i + "通道";
                for (int j = 1; j < 1025; j++)
                {
                    dr[j] = br.ReadSingle();
                }
                dt.Rows.Add(dr);
            }
            br.Close();
            fs.Close();
            this.vGridControl1.DataSource = dt;
            
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            string str = Environment.CurrentDirectory;
            string str1 = Application.StartupPath;
            string str2 = AppDomain.CurrentDomain.BaseDirectory;

            string name = Guid.NewGuid().ToString();
            string dbFilePath = new AppSettingsReader().GetValue("FileDirectory", typeof(string)) as string;
            string filename = dbFilePath + name + ".dat";
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
            fs.Close();
            XtraMessageBox.Show(filename);
        }
    }
}