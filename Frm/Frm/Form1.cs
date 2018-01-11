using Frm.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Frm.Common.NET2991;

namespace Frm
{
    public partial class Form1 : Form
    {
        //添加引用
        public const int WAIT_OBJECT_0 = 0;
        [DllImport("Kernel32.dll")]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMillisenconds);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateEvent(string lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("Ws2_32.dll")]
        public static extern int inet_addr(string ipaddr);






        public Form1()
        {
            InitializeComponent();
            //任意通道模拟波形输出
            this.chart1.Series[0].Points.Clear();
            this.chart1.Series[0].Points.AddXY(0, 0);
            this.chart1.Series[0].Color = Color.Red;
            //x轴
            this.chart1.ChartAreas[0].AxisX.Maximum = 100;
            this.chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            this.chart1.ChartAreas[0].AxisX.LineWidth = 2;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Silver;

            //y轴
            this.chart1.ChartAreas[0].AxisY.Maximum = 10;
            this.chart1.ChartAreas[0].AxisY.Minimum = -10;
            this.chart1.ChartAreas[0].AxisY.Interval = 1;
            this.chart1.ChartAreas[0].AxisY.Crossing = 0;
            this.chart1.ChartAreas[0].AxisY.LineWidth = 2;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Silver;



            //chart2 初始化,一致性Chart表

            Series s1 = new Series();
            s1.Name = "幅度";
            Series s2 = new Series();
            s2.Name = "相位";
            Random r = new Random();
            for (int i = 1; i < 17; i++)
            {
                s1.Points.AddXY(i, r.Next(20, 30));
                s2.Points.AddXY(i, r.Next(10, 20));
            }
            chart2.Series.Add(s1);
            chart2.Series.Add(s2);
            this.chart2.Series[0].ChartType = SeriesChartType.Column;
            this.chart2.Series[1].ChartType = SeriesChartType.Column;
            this.chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Silver;
            this.chart2.ChartAreas[0].AxisX.Interval = 1;
            this.chart2.ChartAreas[0].AxisX.Maximum = 17;
            this.chart2.ChartAreas[0].AxisX.Minimum = 0;
            this.chart2.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
            this.chart2.ChartAreas[0].AxisX.LabelStyle.Format = "{0}路";
            this.chart2.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 1;
            this.chart2.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;

            //chart3初始化
            this.chart3.Series[0].Points.Clear();
            this.chart3.Series[0].Color = Color.Red;
            //x轴
            this.chart3.ChartAreas[0].AxisX.Maximum = 100;
            this.chart3.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            this.chart3.ChartAreas[0].AxisX.LineWidth = 2;
            this.chart3.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Silver;

            //y轴
            this.chart3.ChartAreas[0].AxisY.Maximum = 10;
            this.chart3.ChartAreas[0].AxisY.Minimum = -10;
            this.chart3.ChartAreas[0].AxisY.Interval = 1;
            this.chart3.ChartAreas[0].AxisY.Crossing = 0;
            this.chart3.ChartAreas[0].AxisY.LineWidth = 2;
            this.chart3.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Silver;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            this.tabControl1.SelectedTab = this.tabPage5;
            //跨线程访问UI控件
            Control.CheckForIllegalCrossThreadCalls = false;
            //去掉行头下三角符号
            dataGridView2.RowHeadersDefaultCellStyle.Padding = new Padding(dataGridView2.RowHeadersWidth);
        }

        /// <summary>
        /// 基础信息——转到任意通道信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage4;
        }

        public static Queue<double> toolqueue = new Queue<double>(3600);
       
        /// <summary>
        /// 首页载入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            CfgPara.AIParam.szDevName = new sbyte[32];
            CfgPara.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            Int32 dwChanCnt = 0; //通道总数
            NET2991.NET2991_DEV_Init(4096);

            for (int i = 0; i < 32; i++)
            {
                CfgPara.AIParam.szDevName[i] = 0;
            }
            Int32 NetworkOrder = inet_addr("192.168.0.145");
            CfgPara.AIParam.nDeviceIP = (UInt32)(IPAddress.NetworkToHostOrder(NetworkOrder));
            CfgPara.AIParam.nDevicePort = 9000;
            CfgPara.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {

                CfgPara.AIParam.CHParam[i].bChannelEn = 1;
                CfgPara.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
            }

            CfgPara.AIParam.fSampleRate = 1000000;
            CfgPara.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_CONTINUOUS;
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

            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;//数据传输方向
            //创建设备
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP, CfgPara.AIParam.nDevicePort, CfgPara.AIParam.nLocalPort, nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                ShowErr("创建设备失败");
                goto Exit;
            }
            //判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice) == 0)
            {
                ShowErr("设备没有连接");
                goto Exit;

            }
            //初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1)) == 0)
            {
                ShowErr("初始化设备失败");
                goto Exit;

            }

            //清除缓存
            if (NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice) == 0)
            {
                ShowErr("清除缓存失败");
                goto Exit;

            }

            //设置是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice, 0) == 0)
            {
                ShowErr("设置文件保存失败");
                goto Exit;

            }

            //设置完毕最终执行代码
            dwChanCnt = 0;
            for (int i = 0; i < NET2991.NET2991_AI_MAX_CHANNELS; i++)
            {
                if (CfgPara.AIParam.CHParam[i].bChannelEn == 1)
                {
                    dwChanCnt++;
                }
            }
            //实际总共需要读取的数据数
            CfgPara.dwRealReadLen = 2 * dwChanCnt * CfgPara.nReadLength;
            //初始化
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;
            //读取数据的长度
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;

            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice) == 0)
            {
                ShowErr("启动采集任务失败");
                goto Exit;
            }

            Thread tDataRead = new Thread(ReadData1);
            tDataRead.Start();
            tDataRead.IsBackground = true;
            return;

            Exit:
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            MessageBox.Show("程序结束");
        }
        private void ReadData1()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("通道号", typeof(string));
            DataColumn dc1 = new DataColumn("波形有无", typeof(string));
            DataColumn dc2 = new DataColumn("信号频率", typeof(string));
            DataColumn dc3 = new DataColumn("信号脉宽", typeof(string));
            DataColumn dc4 = new DataColumn("信号VPP", typeof(string));
            dt.Columns.Add(dc);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            //清空默认列信息
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.DataSource = dt;

            UInt16[] nAIArray = new UInt16[2048];
            UInt32 dwReadChan = 0;
            UInt32 dwReadSampsPerChan = 0;
            UInt32 dwSampsPerChanRead = 0;
            UInt32 dwAvailSampsPerChan = 0;
            double fTimeOut = -1.0;
            Int32 nIndex = 0;
            double fVoltData = 0.00D;
            double fPerLsb = 0.00D;
            switch (CfgPara.AIParam.CHParam[nIndex].nSampleRange)
            {
                case NET2991.NET2991_AI_SAMPRANGE_N10_P10V:
                    fPerLsb = 20000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N5_P5V:
                    fPerLsb = 10000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N2D5_P2D5V:
                    fPerLsb = 5000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N1D25_P1D25V:
                    fPerLsb = 2500.00 / 65536;
                    break;
                default:
                    break;
            }
            //根据用户设置的读取长度设置每通道数据的读取长度
            UInt32 nChanSize = (uint)(CfgPara.nReadLength * 2);
            bool flag = false;
            for (int j = 0; j < 16; j++)
            {
                while (true)
                {
                    dwReadSampsPerChan = 0;
                    NET2991.NET2991_AI_ReadBinary(CfgPara.hDevice, ref dwReadChan, nAIArray, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut);
                    for (int i = 0; i < dwSampsPerChanRead / 17; i++)
                    {
                        fVoltData = nAIArray[i * 17 + j + 1] * fPerLsb - 10000.00;
                        toolqueue.Enqueue(fVoltData);
                        if (toolqueue.Count >= 3600)
                        {
                            if (toolqueue.Max() >= 200)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = "AI" + j + "通道";
                                dr[1] = "有";
                                dr[2] = "10KHZ";
                                dr[3] = (3600/10 ^ 6).ToString("0.00");
                                dr[4] = (toolqueue.Max() - toolqueue.Min()).ToString("0.00");
                                dt.Rows.Add(dr);
                                
                            }
                            else
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = "AI" + j + "通道";
                                dr[1] = "无";
                                dr[2] = "10KHZ";
                                dr[3] = "";
                                dr[4] = 0;
                                dt.Rows.Add(dr);
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        break;
                    }
                }
                toolqueue.Clear();
                flag = false;
            }
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
        }
        /// <summary>
        /// 基础性信息表格行首添加图标显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (this.dataGridView2.Rows[e.RowIndex].Cells[1].Value == DBNull.Value)
            {
                return;
            }
            string flag = Convert.ToString(dataGridView2.Rows[e.RowIndex].Cells[1].Value);
            Image img;
            string strToolTip;
            if (flag == "有")
            {
                img = Properties.Resources.绿灯16;
                strToolTip = "有信号值";
            }
            else
            {
                img = Properties.Resources.红灯16;
                strToolTip = "无信号值";
            }
            e.Graphics.DrawImage(img, e.RowBounds.Left + this.dataGridView2.RowHeadersWidth - 20, e.RowBounds.Top + 4, 16, 16);
            this.dataGridView2.Rows[e.RowIndex].HeaderCell.ToolTipText = strToolTip;

        }
        public static Queue<double> dataqueue1 = new Queue<double>(100);
        public static ulong ColumnCount = 0;
        /// <summary>
        /// 详细信息——输出具体某通道详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_Click(object sender, EventArgs e)
        {

            CfgPara.AIParam.szDevName = new sbyte[32];
            CfgPara.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            Int32 nIndex = 0;
            Int32 dwChanCnt = 0; //通道总数

            NET2991.NET2991_DEV_Init(4096);

            for (int i = 0; i < 32; i++)
            {
                CfgPara.AIParam.szDevName[i] = 0;
            }
            Int32 NetworkOrder = inet_addr("192.168.0.145");
            CfgPara.AIParam.nDeviceIP = (UInt32)(IPAddress.NetworkToHostOrder(NetworkOrder));
            CfgPara.AIParam.nDevicePort = 9000;
            CfgPara.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {

                CfgPara.AIParam.CHParam[i].bChannelEn = 1;
                CfgPara.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
            }

            CfgPara.AIParam.fSampleRate = 1000000;
            CfgPara.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_CONTINUOUS;
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


            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;//数据传输方向
            //创建设备
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP, CfgPara.AIParam.nDevicePort, CfgPara.AIParam.nLocalPort, nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                ShowErr("创建设备失败");
                goto Exit;
            }
            //判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice) == 0)
            {
                ShowErr("设备没有连接");
                goto Exit;

            }
            //初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1)) == 0)
            {
                ShowErr("初始化设备失败");
                goto Exit;

            }

            //清除缓存
            if (NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice) == 0)
            {
                ShowErr("清除缓存失败");
                goto Exit;

            }

            //设置是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice, 0) == 0)
            {
                ShowErr("设置文件保存失败");
                goto Exit;

            }

            dwChanCnt = 0;
            for (int i = 0; i < NET2991.NET2991_AI_MAX_CHANNELS; i++)
            {
                if (CfgPara.AIParam.CHParam[i].bChannelEn == 1)
                {
                    dwChanCnt++;
                }
            }
            //实际总共需要读取的数据数
            CfgPara.dwRealReadLen = 2 * dwChanCnt * CfgPara.nReadLength;

            //初始化
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;
            //读取数据的长度
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;



            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice) == 0)
            {
                MessageBox.Show("启动采集任务失败");
                goto Exit;
            }

            Thread tDataRead = new Thread(ReadData);
            tDataRead.Start();
            tDataRead.IsBackground = true;
            return;



            Exit:
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            MessageBox.Show("程序结束");

        }
        private void ReadData()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("通道号", typeof(String));
            DataColumn dc1 = new DataColumn("采样值", typeof(string));
            DataColumn dc2 = new DataColumn("Vpp", typeof(string));
            DataColumn dc3 = new DataColumn("波峰最大值", typeof(string));
            DataColumn dc4 = new DataColumn("波谷最小值", typeof(string));
            dt.Columns.Add(dc);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView3.DataSource = dt;



            UInt16[] nAIArray = new UInt16[2048];
            UInt32 dwReadChan = 0;
            UInt32 dwReadSampsPerChan = 0;
            UInt32 dwSampsPerChanRead = 0;
            UInt32 dwAvailSampsPerChan = 0;
            double fTimeOut = -1.0;
            Int32 nIndex = 0;
            double fVoltData = 0.00D;
            double fPerLsb = 0.00D;
            //Int16 tmp = 0;


            switch (CfgPara.AIParam.CHParam[nIndex].nSampleRange)
            {
                case NET2991.NET2991_AI_SAMPRANGE_N10_P10V:
                    fPerLsb = 20000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N5_P5V:
                    fPerLsb = 10000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N2D5_P2D5V:
                    fPerLsb = 5000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N1D25_P1D25V:
                    fPerLsb = 2500.00 / 65536;
                    break;
                default:
                    break;
            }
            //根据用户设置的读取长度设置每通道数据的读取长度
            UInt32 nChanSize = (uint)(CfgPara.nReadLength * 2);
            while (true)
            {
                dwReadSampsPerChan = 0;
                NET2991.NET2991_AI_ReadBinary(CfgPara.hDevice, ref dwReadChan, nAIArray, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut);
                for (int i = 0; i < dwSampsPerChanRead / 17; i++)
                {

                    //fVoltData = nAIArray[i * 17 + 1] * fPerLsb - 10000.00;

                    DataRow dr = dt.NewRow();
                    dr[0] = ColumnCount++;
                    dr[1] = (nAIArray[i * 17 + 1] * fPerLsb - 10000.00).ToString("0.00");
                    if (toolqueue.Count > 360)
                    {
                        toolqueue.Dequeue();
                    }
                    else
                    {
                        toolqueue.Enqueue(nAIArray[i * 17 + 1] * fPerLsb - 10000.00);
                    }


                    dr[2] = (toolqueue.Max() - toolqueue.Min()).ToString("0.00");
                    dr[3] = toolqueue.Max().ToString("0.00");
                    dr[4] = toolqueue.Min().ToString("0.00");
                    dt.Rows.Add(dr);

                }


                if (ColumnCount >= 500)
                {
                    break;
                }
                //Application.DoEvents();
                //Thread.Sleep(1000);
            }
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }

        }
        delegate void delePaintChart();
        private void PaintChart1()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new delePaintChart(PaintChart1));
            }
            else
            {
                this.chart1.Series[0].Points.Clear();
                for (int i = 0; i < dataqueue1.Count; i++)
                {
                    this.chart1.Series[0].Points.AddXY(i, dataqueue1.ElementAt(i));
                }
            }

        }
        public static UserDef.CFGPARA CfgPara;
        public static NET2991.NET2991_AI_STATUS Sts;
        public static NET2991.DEVICE_SAMP_STS SampSts;
        /// <summary>
        /// 配置——开始采样
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Test_Click(object sender, EventArgs e)
        {
            CfgPara.AIParam.szDevName = new sbyte[32];
            CfgPara.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            Int32 nIndex = 0;
            Int32 dwChanCnt = 0; //通道总数

            NET2991.NET2991_DEV_Init(4096);

            for (int i = 0; i < 32; i++)
            {
                CfgPara.AIParam.szDevName[i] = 0;
            }
            Int32 NetworkOrder = inet_addr("192.168.0.150");
            CfgPara.AIParam.nDeviceIP = (UInt32)(IPAddress.NetworkToHostOrder(NetworkOrder));
            CfgPara.AIParam.nDevicePort = 9000;
            CfgPara.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {

                CfgPara.AIParam.CHParam[i].bChannelEn = 1;
                CfgPara.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
            }

            CfgPara.AIParam.fSampleRate = 1000000;
            CfgPara.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_CONTINUOUS;
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


            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;//数据传输方向
            //创建设备
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP, CfgPara.AIParam.nDevicePort, CfgPara.AIParam.nLocalPort, nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                ShowErr("创建设备失败");
                goto Exit;
            }
            //判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice) == 0)
            {
                ShowErr("设备没有连接");
                goto Exit;

            }
            //初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1)) == 0)
            {
                ShowErr("初始化设备失败");
                goto Exit;

            }

            //清除缓存
            if (NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice) == 0)
            {
                ShowErr("清除缓存失败");
                goto Exit;

            }

            //设置是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice, 0) == 0)
            {
                ShowErr("设置文件保存失败");
                goto Exit;

            }
            //文件路径设置,如果设置保存文件，必须
            //if (NET2991ARSV.NET2991_FILE_Create(CfgPara.hDevice, "D:\\", "Net2991Data") == 0)
            //{
            //    ShowErr("创建保存文件失败");
            //}


            //检查完毕最终执行代码

            dwChanCnt = 0;
            for (int i = 0; i < NET2991.NET2991_AI_MAX_CHANNELS; i++)
            {
                if (CfgPara.AIParam.CHParam[i].bChannelEn == 1)
                {
                    dwChanCnt++;
                }
            }
            //实际总共需要读取的数据数
            CfgPara.dwRealReadLen = 2 * dwChanCnt * CfgPara.nReadLength;

            //初始化
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;
            //读取数据的长度
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;



            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice) == 0)
            {
                MessageBox.Show("启动采集任务失败");
                goto Exit;
            }

            Thread tDataRead = new Thread(ReadDataFun);
            tDataRead.Start();
            tDataRead.IsBackground = true;
            return;



            Exit:
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            MessageBox.Show("程序结束");
        }
        /// <summary>
        ///配置——开始采样线程任务
        /// </summary>
        private void ReadDataFun()
        {
            
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("序号", typeof(String));
            DataColumn dc1 = new DataColumn("AI0值", typeof(string));
            DataColumn dc2 = new DataColumn("Vpp", typeof(string));
            DataColumn dc3 = new DataColumn("波峰最大值", typeof(string));
            DataColumn dc4 = new DataColumn("波谷最小值", typeof(string));
            dt.Columns.Add(dc);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView3.DataSource = dt;



            UInt16[] nAIArray = new UInt16[2048];
            UInt32 dwReadChan = 0;
            UInt32 dwReadSampsPerChan = 0;
            UInt32 dwSampsPerChanRead = 0;
            UInt32 dwAvailSampsPerChan = 0;
            double fTimeOut = -1.0;
            Int32 nIndex = 0;
            double fVoltData = 0.00D;
            double fPerLsb = 0.00D;
            //Int16 tmp = 0;


            switch (CfgPara.AIParam.CHParam[nIndex].nSampleRange)
            {
                case NET2991.NET2991_AI_SAMPRANGE_N10_P10V:
                    fPerLsb = 20000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N5_P5V:
                    fPerLsb = 10000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N2D5_P2D5V:
                    fPerLsb = 5000.00 / 65536;
                    break;
                case NET2991.NET2991_AI_SAMPRANGE_N1D25_P1D25V:
                    fPerLsb = 2500.00 / 65536;
                    break;
                default:
                    break;
            }
            //根据用户设置的读取长度设置每通道数据的读取长度
            UInt32 nChanSize = (uint)(CfgPara.nReadLength * 2);
            try
            {
                while (true)
                {
                    dwReadSampsPerChan = 0;
                    NET2991.NET2991_AI_ReadBinary(CfgPara.hDevice, ref dwReadChan, nAIArray, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut);
                    for (int i = 0; i < dwSampsPerChanRead / 17; i++)
                    {

                        //fVoltData = nAIArray[i * 17 + 1] * fPerLsb - 10000.00;

                        DataRow dr = dt.NewRow();
                        dr[0] = ColumnCount++;
                        dr[1] = (nAIArray[i * 17 + 1] * fPerLsb - 10000.00).ToString("0.00");
                        if (toolqueue.Count > 360)
                        {
                            toolqueue.Dequeue();
                        }
                        else
                        {
                            toolqueue.Enqueue(nAIArray[i * 17 + 1] * fPerLsb - 10000.00);
                        }


                        dr[2] = (toolqueue.Max() - toolqueue.Min()).ToString("0.00");
                        dr[3] = toolqueue.Max().ToString("0.00");
                        dr[4] = toolqueue.Min().ToString("0.00");
                        dt.Rows.Add(dr);

                    }


                    if (ColumnCount >= 100000)
                    {
                        NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                        NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                        CfgPara.hDevice = (IntPtr)(-1);
                        break;
                    }
                    Application.DoEvents();

                }
            }
            catch (Exception)
            {
               
                    NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                    NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                    CfgPara.hDevice = (IntPtr)(-1);
                
            }
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }

        }
        
        public static Queue<double> dataqueue3 = new Queue<double>(360);
        delegate void delePaintChart3();
        private void PaintChart3()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new delePaintChart(PaintChart3));
            }
            else
            {
                this.chart3.Series[0].Points.Clear();
                for (int i = 0; i < dataqueue3.Count; i++)
                {
                    this.chart3.Series[0].Points.AddXY(i, dataqueue3.ElementAt(i));
                }
            }
        }
        

        //出错失败提示
        private void ShowErr(string msg)
        {
            MessageBox.Show(msg, "失败提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        /// <summary>
        /// 配置——输出为波形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            CfgPara.AIParam.szDevName = new sbyte[32];
            CfgPara.AIParam.CHParam = new NET2991.NET2991_CH_PARAM[17];
            Int32 nIndex = 0;
            Int32 dwChanCnt = 0; //通道总数

            NET2991.NET2991_DEV_Init(4096);

            for (int i = 0; i < 32; i++)
            {
                CfgPara.AIParam.szDevName[i] = 0;
            }
            Int32 NetworkOrder = inet_addr("192.168.0.145");
            CfgPara.AIParam.nDeviceIP = (UInt32)(IPAddress.NetworkToHostOrder(NetworkOrder));
            CfgPara.AIParam.nDevicePort = 9000;
            CfgPara.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {

                CfgPara.AIParam.CHParam[i].bChannelEn = 1;
                CfgPara.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_RSE;
            }

            CfgPara.AIParam.fSampleRate = 1000000;
            CfgPara.AIParam.nSampleMode = NET2991.NET2991_AI_SAMPMODE_CONTINUOUS;
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


            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;//数据传输方向
            //创建设备
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP, CfgPara.AIParam.nDevicePort, CfgPara.AIParam.nLocalPort, nDataTranDir, 0.2, 0.2, 2);
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                ShowErr("创建设备失败");
                goto Exit;
            }
            //判断是否连接
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice) == 0)
            {
                ShowErr("设备没有连接");
                goto Exit;

            }
            //初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1)) == 0)
            {
                ShowErr("初始化设备失败");
                goto Exit;

            }

            //清除缓存
            if (NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice) == 0)
            {
                ShowErr("清除缓存失败");
                goto Exit;

            }

            //设置是否保存文件
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice, 0) == 0)
            {
                ShowErr("设置文件保存失败");
                goto Exit;

            }
            //文件路径设置,如果设置保存文件，必须
            //if (NET2991ARSV.NET2991_FILE_Create(CfgPara.hDevice, "D:\\", "Net2991Data") == 0)
            //{
            //    ShowErr("创建保存文件失败");
            //}


            //检查完毕最终执行代码

            dwChanCnt = 0;
            for (int i = 0; i < NET2991.NET2991_AI_MAX_CHANNELS; i++)
            {
                if (CfgPara.AIParam.CHParam[i].bChannelEn == 1)
                {
                    dwChanCnt++;
                }
            }
            //实际总共需要读取的数据数
            CfgPara.dwRealReadLen = 2 * dwChanCnt * CfgPara.nReadLength;

            //初始化
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;
            //读取数据的长度
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;



            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice) == 0)
            {
                MessageBox.Show("启动采集任务失败");
                goto Exit;
            }

            Thread tDataRead = new Thread(ReadDataFun);
            tDataRead.Start();
            tDataRead.IsBackground = true;
            return;



            Exit:
            if (CfgPara.hDevice != (IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            MessageBox.Show("程序结束");

        }


        /// <summary>
        /// 有限采样8192点并保存文件在本地C区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            CfgPara.AIParam.szDevName = new sbyte[32];
            CfgPara.AIParam.CHParam = new NET2991_CH_PARAM[17];
            //通道总数
            Int32 dwchanCnt = 0;

            NET2991.NET2991_DEV_Init(4096);
            for (int i = 0; i < 32; i++)
            {
                CfgPara.AIParam.szDevName[i] = 0;
            }
            Int32 ipaddr = inet_addr("192.168.0.150");
            CfgPara.AIParam.nDeviceIP = (UInt32)(System.Net.IPAddress.NetworkToHostOrder(ipaddr));
            CfgPara.AIParam.nDevicePort = 9000;
            CfgPara.AIParam.nLocalPort = 8000;
            for (int i = 0; i < 17; i++)
            {
                CfgPara.AIParam.CHParam[i].bChannelEn = 1;
                CfgPara.AIParam.CHParam[i].nSampleRange = NET2991.NET2991_AI_SAMPRANGE_N10_P10V;
                CfgPara.AIParam.CHParam[i].nRefGround = NET2991.NET2991_AI_REFGND_DIFF;
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
            //读取长度
            CfgPara.nReadLength = 1000000;
            CfgPara.hDevice = (IntPtr)(-1);

            UInt32 bReRead = 0;
            UInt16 nDataTranDir = NET2991.NET2991_AI_TRANDIR_CLIENT;

            //创建设备
            CfgPara.hDevice = NET2991.NET2991_DEV_Create(CfgPara.AIParam.nDeviceIP,
                                                        CfgPara.AIParam.nDevicePort,
                                                        CfgPara.AIParam.nLocalPort,
                                                        nDataTranDir,
                                                        0.2, 0.2, 2);
            //判断是否创建设备
            if (CfgPara.hDevice == (IntPtr)(-1))
            {
                MessageBox.Show("创建设备失败");
                goto ExitRead;
            }
            if (NET2991.NET2991_DEV_IsLink(CfgPara.hDevice)==0)
            {
                MessageBox.Show("设备没有连接");
                goto ExitRead;
            }
            //设置读取偏移和读取长度
            NET2991.NET2991_AI_SetReadOffsetAndLength(CfgPara.hDevice, CfgPara.nReadOffset, CfgPara.nReadLength);
            //设置是否重读
            UInt32 nLastSamps = 0;
            if (NET2991.NET2991_AI_SetReReadData(CfgPara.hDevice,ref CfgPara.AIParam,ref nLastSamps,bReRead)==0)
            {
                MessageBox.Show("设置重读失败");
                goto ExitRead;
            }
            //设置保存文件选项
            if (NET2991.NET2991_AI_IsSaveFile(CfgPara.hDevice,0)==0)
            {
                MessageBox.Show("设置文件保存失败");
                goto ExitRead;
            }
            //初始化设备
            if (NET2991.NET2991_AI_InitTask(CfgPara.hDevice, ref CfgPara.AIParam, (IntPtr)(-1))==0)
            {
                MessageBox.Show("初始化设备失败");
                goto ExitRead;
            }
            //获取时钟状态
            if (NET2991.NET2991_AI_GetClockStatus(CfgPara.hDevice, ref Sts)==0)
            {
                MessageBox.Show("获取时钟状态失败!");
                goto ExitRead;
            }

            //计算通道总数
            for (int i = 0; i < NET2991.NET2991_MAX_CHANNELS; i++)
            {
                if (CfgPara.AIParam.CHParam[i].bChannelEn==1)
                {
                    dwchanCnt++;
                }
            }
            //计算总共需要读取的数据数
            CfgPara.dwRealReadLen = 2 * dwchanCnt * CfgPara.nReadLength;
            //初始化
            CfgPara.nSampStatus = UserDef.CMD_UNCPT;
            CfgPara.dwReadDataSize = 0;
            CfgPara.bAIStatus = 0;
            //读取数据的长度
            CfgPara.dwFrameLen = UserDef.LMT_FRMCNT;
            //清除缓存
            NET2991.NET2991_AI_ClearBuffer(CfgPara.hDevice);
            //启动采样
            if (NET2991.NET2991_AI_StartTask(CfgPara.hDevice)==0)
            {
                MessageBox.Show("启动采样任务失败");
                goto ExitRead;
            }
            //获取采样任务结束的状态
            while (true)
            {
                NET2991.NET2991_AI_SendSoftTrig(CfgPara.hDevice);
                Thread.Sleep(1);
                //获取状态
                if (NET2991.NET2991_AI_GetStatus(CfgPara.hDevice, ref Sts)==0)
                {
                    MessageBox.Show("得到硬件状态失败");
                    goto ExitRead;
                }
                //获取采样任务结束状态
                if (Sts.bTaskDone==1)
                {
                    MessageBox.Show("取得硬件状态,正在采样...");
                    CfgPara.bAIStatus = 1;
                    break;
                }
            }
            Thread tDataRead = new Thread(SaveFile);
            tDataRead.Start();
            return;

            ExitRead:
            if (CfgPara.hDevice!=(IntPtr)(-1))
            {
                NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                CfgPara.hDevice = (IntPtr)(-1);
            }
            MessageBox.Show("已退出采样任务");
        }
        //线程任务
        private void SaveFile()
        {
            UInt16[] nAIArray = new ushort[2048];
            UInt32 dwReadChan = 0;
            UInt32 dwReadSampsPerChan = 0;
            UInt32 dwSampsPerChanRead = 0;
            UInt32 dwAvailSampsPerChan = 0;
            double fTimeOut = -1.0;
            //总计
            UInt32 dwTotalDataSize = 0;
            Single fPerLsb = 20000.00F / 65536;
            Single fVoltData = 0;
            //每通道数据的读取长度
            UInt32 nChanSize = (uint)(CfgPara.nReadLength * 2);
            FileStream fs = new FileStream(@"C:\1.dat", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                while (true)
                {
                    dwReadSampsPerChan = 0;
                    if (NET2991.NET2991_AI_ReadBinary(CfgPara.hDevice, ref dwReadChan, nAIArray, dwReadSampsPerChan, ref dwSampsPerChanRead, ref dwAvailSampsPerChan, fTimeOut)==0)
                    {
                        continue;
                    }
                    dwTotalDataSize += (dwSampsPerChanRead * 2);
                    if (dwReadChan==0)
                    {
                        for (int i = 0; i < dwSampsPerChanRead; i++)
                        {
                            fVoltData = nAIArray[i] * fPerLsb - 10000.00F;
                            bw.Write(fVoltData);
                        }
                    }
                    if (dwTotalDataSize>=CfgPara.dwRealReadLen)
                    {
                        NET2991.NET2991_AI_SetReadComplete(CfgPara.hDevice);
                        if (CfgPara.hDevice!=(IntPtr)(-1))
                        {
                            NET2991.NET2991_AI_StopTask(CfgPara.hDevice);
                            NET2991.NET2991_DEV_Release(CfgPara.hDevice);
                            CfgPara.hDevice = (IntPtr)(-1);
                        }
                        break;
                    }
                }
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
            MessageBox.Show("采样成功，文件位于: " + @"C:\1.dat");
        }

        //生成8192点波形图
        public static Queue<Single> queue100 = new Queue<Single>(8192);
        private void button14_Click(object sender, EventArgs e)
        {
            this.chart4.Series[0].Points.Clear();
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += new DoWorkEventHandler(bg_DoWork);
                bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerComplete);
                bg.WorkerReportsProgress = true;
                bg.ProgressChanged += Bg_ProgressChanged;
                //bg.WorkerSupportsCancellation = true;
                bg.RunWorkerAsync();
            }

        }

        private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                this.chart4.Series[0].Points.AddY(Convert.ToSingle(e.UserState));
            }
            catch
            {

            }
            
        }

        void bg_DoWork(object sender,DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            FileStream fs = new FileStream(@"C:\1.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            try
            {
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                while (br.BaseStream.Position < br.BaseStream.Length/2)
                {
                    bw.ReportProgress(0, br.ReadSingle());
                    Application.DoEvents();
                }
            }
            finally
            {
                br.Close();
                fs.Close();
                bw.Dispose();
            }
           
        }
        void bg_RunWorkerComplete(object sender,RunWorkerCompletedEventArgs e)
        {
        }

       
        //生成幅频特性图
        private void button15_Click(object sender, EventArgs e)
        {
            
                    this.chart5.Series[0].Points.Clear();
                    List<Complex> list = new List<Complex>();
                    FileStream fs = new FileStream(@"C:\1.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        br.BaseStream.Seek(0, SeekOrigin.Begin);
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            list.Add(new Complex(br.ReadSingle(), 0));
                        }
                    }
                    finally
                    {
                        br.Close();
                        fs.Close();
                    }
                    Complex[] list2 = FFT.fft_frequence(list.ToArray(), 8192);
                    for (int i = 0; i < list2.Length; i++)
                    {
                        this.chart5.Series[0].Points.AddY(list2[i].GetModul());
                    }
                
               
           
        }

        private void button16_Click(object sender, EventArgs e)
        {
            MessageBox.Show("正在添加到图表...");
            this.chart6.Series[0].Points.Clear();
            List<Complex> list = new List<Complex>();
            FileStream fs = new FileStream(@"C:\1.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            try
            {
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    list.Add(new Complex(br.ReadSingle(), 0));
                }
            }
            finally
            {
                br.Close();
                fs.Close();
            }
            Complex[] list2 = FFT.fft_frequence(list.ToArray(), 8192);
            for (int i = 0; i < list2.Length; i++)
            {
                this.chart6.Series[0].Points.AddY(list2[i].GetAngle());
            }
        }
        /// <summary>
        /// 基础性信息导出至Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (Utility.DataGridViewShowToExcel(dataGridView2, true)==true)
            {
                MessageBox.Show("导出至Excel成功");
            }
            else
            {
                MessageBox.Show("导出至Excel失败");
            }
        }
    }
}
