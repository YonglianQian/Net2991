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
using System.Runtime.InteropServices;
using Frm.Common;
using DevExpress.XtraEditors;
using System.Threading;
using DevExpress.XtraSplashScreen;
using System.Threading.Tasks;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

namespace Frm.Module
{
    public partial class TestManagement : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public TestManagement()
        {
            InitializeComponent();
        }

        [DllImport("Ws2_32.dll")]
        public static extern int inet_addr(string ipaddr);
        public static UserDef.CFGPARA CfgPara1;
        public static NET2991.NET2991_AI_STATUS Sts;
        public static UserDef.CFGPARA CfgPara2;
        public static NET2991.NET2991_AI_STATUS Sts1;

        /// <summary>
        /// 采集32路通道信号，调用matlab算法处理数据并保存为二进制文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            //板卡参数配置
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
            if (CfgPara1.hDevice == (IntPtr)(-1) || CfgPara2.hDevice == (IntPtr)(-1))
            {
                XtraMessageBox.Show("创建设备失败");
                goto ExitRead;
            }
            // 判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara1.hDevice) == 0 || NET2991.NET2991_DEV_IsLink(CfgPara2.hDevice) == 0)
            {
                XtraMessageBox.Show("设备没有连接!");
                goto ExitRead;
            }

            // 设置读取偏移和读取长度
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara1.hDevice, CfgPara1.nReadOffset, CfgPara1.nReadLength);
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara2.hDevice, CfgPara2.nReadOffset, CfgPara2.nReadLength);

            // 设置是否重读
            UInt32 nLastSamps = 0;
            if (NET2991.NET2991_AI_SetReReadData(CfgPara1.hDevice, ref CfgPara1.AIParam, ref nLastSamps, 0) == 0 || NET2991.NET2991_AI_SetReReadData(CfgPara2.hDevice, ref CfgPara2.AIParam, ref nLastSamps, 0) == 0)
            {
                XtraMessageBox.Show("设置重读失败!");
                goto ExitRead;
            }

            // 是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara1.hDevice, 0) == 0 || NET2991.NET2991_AI_IsSaveFile(CfgPara2.hDevice, 0) == 0)
            {
                XtraMessageBox.Show("设置文件保存失败!");
                goto ExitRead;
            }

            // 初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara1.hDevice, ref CfgPara1.AIParam, (IntPtr)(-1)) == 0 || NET2991.NET2991_AI_InitTask(CfgPara2.hDevice, ref CfgPara2.AIParam, (IntPtr)(-1)) == 0)
            {
                XtraMessageBox.Show("初始化设备失败!");
                goto ExitRead;
            }

            if (NET2991.NET2991_AI_GetClockStatus(CfgPara1.hDevice, ref Sts) == 0 || NET2991.NET2991_AI_GetClockStatus(CfgPara2.hDevice, ref Sts1) == 0)
            {
                XtraMessageBox.Show("获取时钟状态失败!");
                goto ExitRead;
            }


            //配置任务执行
            CfgPara1.dwRealReadLen = 2 * 16 * CfgPara1.nReadLength;
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
            if (NET2991.NET2991_AI_StartTask(CfgPara1.hDevice) == 0 || NET2991.NET2991_AI_StartTask(CfgPara2.hDevice) == 0)
            {
                XtraMessageBox.Show("启动任务失败");
                goto ExitRead;
            }

            while (true)
            {
                NET2991.NET2991_AI_SendSoftTrig(CfgPara1.hDevice);
                NET2991.NET2991_AI_SendSoftTrig(CfgPara2.hDevice);
                Thread.Sleep(1);
                if (NET2991.NET2991_AI_GetStatus(CfgPara1.hDevice, ref Sts) == 0 || NET2991.NET2991_AI_GetStatus(CfgPara2.hDevice, ref Sts1) == 0)
                {
                    XtraMessageBox.Show("得到状态失败");
                    goto ExitRead;
                }
                if (Sts.bTaskDone == 1 && Sts1.bTaskDone == 1)
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
            if (CfgPara1.hDevice != (IntPtr)(-1) || CfgPara2.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara1.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara1.hDevice);
                CfgPara1.hDevice = (IntPtr)(-1);
                NET2991.NET2991_AI_StopTask(CfgPara2.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara2.hDevice);
                CfgPara2.hDevice = (IntPtr)(-1);
            }
        }
        /// <summary>
        /// 配置两块板卡参数
        /// </summary>
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
            CfgPara1.AIParam.nTriggerDir = NET2991.NET2991_AI_TRIGDIR_RISING;
            CfgPara1.AIParam.fTriggerLevel = 0.2F; //触发电平
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
                if (i == 16)
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
                if (i == 16)
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
            CfgPara2.AIParam.nTriggerDir = NET2991.NET2991_AI_TRIGDIR_RISING;
            CfgPara2.AIParam.fTriggerLevel = 0.2F; //触发电平
            CfgPara2.AIParam.nDelaySamps = 0;
            CfgPara2.AIParam.nReTriggerCount = 1;

            CfgPara2.AIParam.bMasterEn = 0;
            CfgPara2.AIParam.nReserved1 = 0;
            CfgPara2.AIParam.nReserved2 = 0;

            CfgPara2.nReadOffset = 0;
            CfgPara2.nReadLength = 1024;
            CfgPara2.hDevice = (IntPtr)(-1);
        }
       
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
                double fTimeOut = -1.0;
                UInt32 nChanSize = (uint)(CfgPara1.nReadLength * 2);//每通道数据读取长度


                string name = Guid.NewGuid().ToString();
                string filename = @"C:\" + name + ".dat";
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                float SampleData = 0F;
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
                        SampleData = Convert.ToSingle((nAIArray1[j] & 0xFFFF) * (20000.00 / 65536) - 10000.00);
                        bw.Write(SampleData);
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
                        SampleData = Convert.ToSingle((nAIArray2[i] & 0xFFFF) * (20000.00 / 65536) - 10000.00);
                        bw.Write(SampleData);
                    }

                    if (dwTotalDataSize >= CfgPara2.dwRealReadLen)
                    {
                        NET2991.NET2991_AI_SetReadComplete(CfgPara2.hDevice);
                        if (CfgPara2.hDevice != (IntPtr)(-1) || CfgPara1.hDevice != (IntPtr)(-1))
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



                //读取数据以供matlab调用
                //float[,] matlabData = new float[32, 1024000];
                //FileStream fstream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                //BinaryReader br = new BinaryReader(fstream);
                //for (int i = 0; i < 32; i++)
                //{
                //    for (int j = 0; j < 1024000; j++)
                //    {
                //        matlabData[i,j]= br.ReadSingle();
                //    }
                //}

                //Delta_amp.Class1 dc = new Delta_amp.Class1();
                //MWNumericArray array = new MWNumericArray(matlabData);
                //MWNumericArray deltaAmp, Max_DeltaAmp, Vpp;
                //MWArray[] Ampresult;
                //Ampresult = dc.Delta_amp(3, 32, 1024000, array);
                //deltaAmp = (MWNumericArray)Ampresult[0];
                //Max_DeltaAmp = (MWNumericArray)Ampresult[1];
                //Vpp = (MWNumericArray)Ampresult[2];
                //double[] deltaAmpArr = (double[])(deltaAmp.ToVector(MWArrayComponent.Real));
                //double Max_deltaAmp1 = Max_DeltaAmp.ToScalarDouble();
                //double[] VppArr = (double[])(Vpp.ToVector(MWArrayComponent.Real));
                //foreach (var item in deltaAmpArr)
                //{
                //    listBoxControl1.Items.Add(item);
                //}
                //listBoxControl2.Items.Add(Max_deltaAmp1);
                //foreach (var item in VppArr)
                //{
                //    listBoxControl3.Items.Add(item);
                //}

                //MWNumericArray deltaxw, Max_Deltaxw;
                //MWArray[] XwResult;
                //XwResult = dc.Delta_xw(2, 32, 1024, array, 4500, 1000000);
                //deltaxw = (MWNumericArray)XwResult[0];
                //Max_Deltaxw = (MWNumericArray)XwResult[1];
                //double[] deltaxwarr = (double[])(deltaxw.ToVector(MWArrayComponent.Real));
                //double Max_deltaxw1 = Max_Deltaxw.ToScalarDouble();

               

















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
    }
}