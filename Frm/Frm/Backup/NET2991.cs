using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Sys
{
    class NET2991
    {
        public const Int32 STATUS_RUNNING = 1;           // 正在采集
        public const Int32 STATUS_STOPED = 2;              // 采集开始或采集完成

        /////////////////////////////////////////////////////////////////////////
        public const Int32 NET2991_MAX_CHANNELS = 17;                   // 本卡最多支持17路模拟量和数字量输入
        public const Int32 NET2991_AI_MAX_CHANNELS = 16;             // 本卡最多支持16路模拟量单端输入通道
        public const Int32 NET2991_DI_MAX_PORTS = 1;                       // 本卡最多支持1个数字量端口
        public const Int32 NET2991_DIO_PORT0_MAX_LINES = 16;      // 数字量端口0支持16条线
        public const Int32 NET2991_MAX_DEVICENAME_LEN = 32;    // 设备名字长度

        //#################### AI硬件参数USB2851_AI_PARAM定义 #####################
        // 通道参数结构体
        public struct NET2991_CH_PARAM
        {
            public UInt32 bChannelEn;             // BYTE[3:0] 通道使能
            public UInt32 nSampleRange;         // BYTE[7:4] 采样范围(Sample Range)档位选择，详见下面常量定义
            public UInt32 nRefGround;             // BYTE[11:8] 地参考方式：	0:单端输入; 1:差分输入
            public UInt32 nReserved0;             // BYTE[15:12] 保留(暂未定义)
            public UInt32 nReserved1;             // BYTE[19:16] 保留(暂未定义)
            public UInt32 nReserved2;             // BYTE[23:20] 保留(暂未定义)
        }

        // AI硬件参数结构体NET2991_CH_PARAM中NET2991_CH_PARAM结构体数组中的nSampleRange采样范围所使用的选项
        public const UInt32 NET2991_AI_SAMPRANGE_N10_P10V = 0;               // ±10V
        public const UInt32 NET2991_AI_SAMPRANGE_N5_P5V = 1;                   // ±5V
        public const UInt32 NET2991_AI_SAMPRANGE_N2D5_P2D5V = 2;          // ±2.5V
        public const UInt32 NET2991_AI_SAMPRANGE_N1D25_P1D25V = 3;       // ±1.25V

        // AI硬件参数结构体NET2991_CH_PARAM中NET2991_CH_PARAM结构体数组中的nRefGround参考地模式所使用的选项
        public const UInt32 NET2991_AI_REFGND_RSE = 0;              // 接地参考单端(Referenced Single Endpoint)
        public const UInt32 NET2991_AI_REFGND_DIFF = 1;             // 差分(Differential)

        public struct NET2991_AI_PARAM
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public SByte[] szDevName;                                               // [31:0]  名称不超过15个汉字，即30个字节
            public UInt32 nDeviceIP;                                                   // [35:32] 设备IP地址
            public UInt16 nDevicePort;                                                // [37:36] 设备端口号
            public UInt16 nLocalPort;                                                  // [39:38] 本机端口号

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public NET2991_CH_PARAM[] CHParam;                       // [447:40] 通道参数配置, 0-15对应AI0-AI15, 16对应DI

            public double fSampleRate;                                                // [455:448] 采样速率[0.00931sps, 1000000sps]
            public UInt32 nSampleMode;                                             // [459:456] 采样模式  2:有限采样, 3:连续采样 
            public UInt32 nSampsPerChan;                                          // [463:460] 设备每个通道采样点数(也是每通道待读取点数)(单位:字)
            public UInt32 nClockSource;                                              // [467:464] 时钟源 0:内时钟; 1:外部参考10M时钟; 2:主卡时钟; 3:外部转换时钟; 
            public UInt32 nReserved0;                                                 // [471:468] 保留(暂未定义)

            public UInt32 nTriggerSource;                                            // [475:472] 触发源选择 0:软件触发; 1:外部数据量触发; 2:外部模拟量触发; 3:同步触发 
            public UInt32 nTriggerDir;                                                  // [479:476] 触发方向 0:下降沿触发; 1:上升沿触发; 2/3:上升下降均触发(变化)
            public Single fTriggerLevel;                                                // [483:480] 触发电平(V)
            public Int32 nDelaySamps;                                                 // [487:484] 触发延迟点数(单位:字)
            public UInt32 nReTriggerCount;                                         // [491:488] 重复触发次数 仅在有限采样的后触发及硬件延时触发模式下

            public UInt32 bMasterEn;                                                  // [495:492] 主设备使能 0:从设备; 1:主设备

            public UInt32 nReserved1;                                                 // [499:496] 保留(暂未定义)
            public UInt32 nReserved2;                                                 // [503:500] 保留(暂未定义)
        }

        // AI硬件数据传输的方向
        public const Int32 NET2991_AI_TRANDIR_CLIENT = 0;             // 数据传输方向为客户端
        public const Int32 NET2991_AI_TRANDIR_SERVER = 1;            // 数据传输方向为服务器

        // AI硬件参数结构体NET2991_AI_PARAM中的nSampleMode采样模式所使用的选项
        public const Int32 NET2991_AI_SAMPMODE_FINITE = 2;                              // 有限采样
        public const Int32 NET2991_AI_SAMPMODE_CONTINUOUS = 3;                 // 连续采样

        // AI硬件参数结构体NET2991_AI_PARAM中的nTriggerSource触发源信号所使用的选项
        public const Int32 NET2991_AI_TRIGSRC_NONE = 0;                         // 无触发(等价于软件触发)
        public const Int32 NET2991_AI_TRIGSRC_DIGITAL = 1;                     // 外部数字量触发
        public const Int32 NET2991_AI_TRIGSRC_ANALOG = 2;                    // 外部模拟量触发
        public const Int32 NET2991_AI_TRIGSRC_SYNC = 3;                          // 同步触发

        // AI硬件参数结构体NET2991_AI_PARAM中的nTriggerDir触发方向所使用的选项
        public const Int32 NET2991_AI_TRIGDIR_FALLING = 0;                       // 下降沿触发
        public const Int32 NET2991_AI_TRIGDIR_RISING = 1;                          // 上升沿触发
        public const Int32 NET2991_AI_TRIGDIR_CHANGE = 2;                       // 变化(上升下降均触发)

        // AI硬件参数结构体NET2991_AI_PARAM中的nClockSource时钟源所使用的选项
        public const Int32 NET2991_AI_CLOCKSRC_LOCAL = 0;                      // 本地时钟(通常为本地晶振时钟OSCCLK),也叫内部时钟
        public const Int32 NET2991_AI_CLOCKSRC_CLKIN_10M = 1;              // 外部参考10M时钟定时触发
        public const Int32 NET2991_AI_CLOCKSRC_MAIN_BOARD = 2;         // 主卡时钟
        public const Int32 NET2991_AI_CLOCKSRC_CLKIN = 3;                       // 外部采样时钟

        // 用于AI采样的硬件工作状态
        public struct NET2991_AI_STATUS
        {
            public UInt32 bTaskDone;                                                          // [3:0]   AI采样任务是否结束，1:表示已结束; 0:表示未结束
            public UInt32 bTriggered;                                                           // [7:4]   AI是否被触发，1:表示已被触发; 0:表示未被触发(默认)
            public UInt32 bFreq10M;                                                           // [11:8]  外部输入信号是否10M
            public UInt32 nClockInFreq;                                                      // [15:12] 外部时钟信号频率, 单位Hz

            public UInt32 nSampTaskState;                                                  // [19:16] 采样任务状态, =1:正常, 其它值表示有异常情况
            public UInt32 nAvailSampsPerChan;                                          // [23:20] 每通道有效点数，只有它大于当前指定读数长度时才能调用AI_ReadAnalog()立即读取指定长度的采样数据
            public UInt32 nMaxAvailSampsPerChan;                                    // [27:24] 自AI_StartTask()后每通道出现过的最大有效点数，状态值范围[0, nBufSampsPerChan],它是为监测采集软件性能而提供，如果此值越趋近于1，则表示意味着性能越高，越不易出现溢出丢点的可能
            public UInt32 nBufSampsPerChan;                                             // [31:28] 每通道缓冲区大小(采样点数)
            public Int64 nSampsPerChanAcquired;                                       // [39:32] 每通道已采样点数(自启动AI_StartTask()之后所采样的点数)，这个只是给用户的统计数据

            public UInt32 nHardOverflowCnt;                                              // [43:40] 硬件溢出计数(在不溢出情况下恒等于0)
            public UInt32 nSoftOverflowCnt;                                               // [47:44] 软件溢出计数(在不溢出情况下恒等于0)
            public UInt32 nInitTaskCnt;                                                       // [51:48] 初始化采样任务的次数(即调用AI_InitTask()的次数)
            public UInt32 nReleaseTaskCnt;                                                // [55:52] 释放采样任务的次数(即调用AI_ReleaseTask()的次数)
            public UInt32 nStartTaskCnt;                                                     // [59:56] 启动采样任务的次数(即调用AI_StartTask()的次数)
            public UInt32 nStopTaskCnt;                                                      // [63:60] 停止采样任务的次数(即调用AI_StopTask()的次数)
            public UInt32 nTransRate;                                                         // [67:64] 传输速率, 即每秒传输点数(sps)，作为USB及应用软件传输性能的监测信息

            public UInt32 nReserved0;                                                         // [71:68] 保留字段(暂未定义)
            public UInt32 nReserved1;                                                         // [75:72] 保留字段(暂未定义)
            public UInt32 nReserved2;                                                         // [79:76] 保留字段(暂未定义)
            public UInt32 nReserved3;                                                         // [83:80] 保留字段(暂未定义)
            public UInt32 nReserved4;                                                         // [87:84] 保留字段(暂未定义)
        }

        // 设备网络参数信息结构体
        public struct DEVICE_NET_INFO
        {
            public UInt32 nDeviceIP;                       // [3:0]   IP地址  192.168.0.1
            public UInt16 nDevicePort;                    // [5:4]   端口号 最大65535
            public UInt16 nReserved0;                     // [7:6]   保留字段(暂未定义)
            public UInt64 nMAC;                            // [15:8]  网卡物理地址,用户一般不可更改
            public UInt32 bOneline;                         // [19:16] 在线状态，1:在线; 0:离线(下线)
            public UInt32 nSubnetMask;                  // [23:20] 子网掩码, "255.255.255.0"
            public UInt32 nGateway;                       // [27:24] 网关, "192.168.0.1"
            public UInt32 nReserved1;                     // [31:28] 保留字段(暂未定义)
        }

        // 得到dll内部设备采样状态
        public struct DEVICE_SAMP_STS
        {
            public UInt32 nDevIP;                               // [0:3]设备IP
            public Int32 nSampSts;                               // [4:7]设备状态

            public Int32 nChan;                                    // [8:11]当前通道号，有限使用
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public SByte[] szFileName;                        // [12:267]文件名
            public UInt32 nReserved0;                          // [268:271]保留未用
            public UInt64 ullFileSize;                             // [272:279]文件大小
        }

        // 设备数据读取完成信息结构体
        public struct DEVICE_SAMP_CMP
        {
            public UInt32 nDevIP;                   // [0:3]设备IP
            public UInt32 bSampCmp;             // [4:7]设备数据读取完成标志 0：未完成; 1：完成
        }

        // 采样状态定义 内部定义,已占用0--100,用户需要定义其它状态请从101开始
        public const Int32 NET2991_AI_SAMPSTS_NONE = 0;                                       // 无采样状态
        public const Int32 NET2991_AI_SAMPSTS_REDUCESPEED_FAIL = 1;              // 降速失败， 有限时返回
        public const Int32 NET2991_AI_FILE_READCH = 2;                                           // 保存文件时正在读取的通道号,有限时返回
        public const Int32 NET2991_AI_SAMPSTS_SAVEFILE = 3;                                 // 正在保存文件
        public const Int32 NET2991_AI_SAMPSTS_SAVEFILE_COMPLETE = 4;           // 保存文件完成
        public const Int32 NET2991_AI_SAMPSTS_START_PROCESSFFILE = 5;           // 开始文件处理
        public const Int32 NET2991_AI_SAMPSTS_FINISH_PROCESSFFILE = 6;          // 完成文件处理

        // ################################ DEV设备对象管理函数 ###################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_Init(UInt32 nReadBufSize);        // 对设备采集做一些初始工作

        [DllImport("NET2991.DLL")]
        public static extern IntPtr NET2991_DEV_Create(                                                  // 创建设备对象句柄(hDevice), 成功返回实际句柄,失败则返回INVALID_HANDLE_VALUE(-1)
                                                                                    UInt32 nDeviceIP,                      // 设备IP
                                                                                    UInt16 nDevicePort,                   // 设备端口号
                                                                                    UInt16 nLocalPort,                      // 本地端口号
                                                                                    UInt16 nDataTranDir,                  // 数据传输方向 0:客户端方向,1:服务器方向
                                                                                    Double fSendTimeout,                  // 发送超时(单位:秒)
                                                                                    Double fRecvTimeout,                 // 接收超时(单位:秒)
                                                                                    Double fFiniteWaitTimeOut);        // 有限模式下数据等待超时

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_Release(IntPtr hDevice);                       // 释放设备对象

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_IsLink(IntPtr hDevice);                       // 判断设备是否真正连接

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_SetSendSpeed(Single fSendSpeed);                       // 设置设备发送数据的速度, 设备发送数据的速度,1---127,1最快，127最慢

        // ################################ DEV设备网络参数管理函数 ################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_SetNetInfo(                                                                           // 设置网络信息
                                                                                        IntPtr hDevice,                                                      // 设备对象句柄,它由DEV_Create()函数创建
                                                                                        ref DEVICE_NET_INFO pNetInfo);                     // 网络信息结构体

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_GetNetInfo(                                                                           // 得到网络信息
                                                                                        IntPtr hDevice,                                                      // 设备对象句柄,它由DEV_Create()函数创建
                                                                                        ref DEVICE_NET_INFO pNetInfo);                     // 网络信息结构体

        // ################################ AI模拟量输入实现函数 ###################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_InitTask(                                                                           // 得到网络信息
                                                                                 IntPtr hDevice,                                                      // 设备对象句柄,它由DEV_Create()函数创建
                                                                                 ref NET2991_AI_PARAM pAIParam,                  // AI工作参数, 它仅在此函数中决定硬件初始状态和各工作模式
                                                                                 IntPtr pSampEvent);                                             // 返回采样事件对象句柄,当设备中出现可读数据段时会触发此事件，参数=NULL,表示不需要此事件句柄

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_ClearBuffer(IntPtr hDevice);                                             // 清除AI硬件缓存

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_StartTask(IntPtr hDevice);                                                 // 启动采集任务

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_SendSoftTrig(IntPtr hDevice);                                            // 发送软件触发事件(Send Software Trigger),软件触发也叫强制触发

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_GetStatus(                                                                           // 取得AI各种状态
                                                                                    IntPtr hDevice,                                                    // 设备对象句柄,它由DEV_Create()函数创建
                                                                                    ref NET2991_AI_STATUS pAIStatus);                // AI状态结构体

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_GetClockStatus(                                                                   // 得到时钟状态,外部10M时钟时有效
                                                                                    IntPtr hDevice,                                                    // 设备对象句柄,它由DEV_Create()函数创建
                                                                                    ref NET2991_AI_STATUS pAIStatus);                // AI状态结构体

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_SetReadOffsetAndLength(                                                  // 设置读取数据的偏移位置和读取长度(有限时模式下使用)
                                                                                    IntPtr hDevice,                                                    // 设备对象句柄,它由DEV_Create()函数创建
                                                                                    Int32 nReadOffset,                                               // 采样数据的偏移位置，参考点是整个采样序列的0位置(单位:字)
                                                                                    Int32 nReadLength);                                            // 采样数据的长度(单位:字)

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_ReadBinary(                                                                       // 读取采样数据(二进制原码数据序列)
                                                                                    IntPtr hDevice,                                                    // 设备对象句柄,它由DEV_Create()函数创建
                                                                                    ref UInt32 pReadChannel,                                       // 返回的读取数据的通道号(物理通道号),取值范围[0, 16], 0-15对应AI0-AI15, 16对应DI,有限模式使用，连续模式==NULL
                                                                                    UInt16[] nBinArray,                                             // 模拟数据数组(二进制原码数组),用于返回采样的二进制原码数据，取值区间由各通道采样时的采样范围决定(单位:V)
                                                                                    UInt32 nReadSampsPerChan,                               // 每通道请求读取的点数(单位：点)
                                                                                    ref UInt32 pSampsPerChanRead,                         // 返回每通道实际读取的点数(单位：点), =NULL,表示无须返回
                                                                                    ref UInt32 pAvailSampsPerChan,                         // 任务中还存在的可读点数, =NULL,表示无须返回
                                                                                    Double fTimeout);                                               // 超时时间，单位：秒, -1:无超时

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_SetReadComplete(IntPtr hDevice);                                        // 某设备采样完成

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_StopTask(IntPtr hDevice);                                                     // 某设备采样完成

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_IsSaveFile(                                                                           // 是否保存文件
                                                                                    IntPtr hDevice,                                                     // 设备对象句柄,它由DEV_Create()函数创建
                                                                                    UInt32 bSaveFile);                                                // 0：不保存文件(图形数字显示)，1：保存文件

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_SetReReadData(                                                                  // 是否保存文件
                                                                                    IntPtr hDevice,                                                     // 设备对象句柄,它由DEV_Create()函数创建
                                                                                    ref NET2991_AI_PARAM pAIParam,                 // AI工作参数, 它仅在此函数中决定硬件初始状态和各工作模式
                                                                                    ref UInt32 pLastSampsPerChan,                           // 历史待采样点数
                                                                                    UInt32 bReRead);                                                // 设置是否重新读取

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_GetDevSampleStatus(ref DEVICE_SAMP_STS pDevSampSts);      // 得到dll里设备采样的状态
    }
}
