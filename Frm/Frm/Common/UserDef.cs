using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Frm.Common
{
    public class UserDef
    {
        // 通道信息
        public struct CHANNEL_INFO
        {
            public Byte nChan;                        // 通道号
            public UInt32 dwChanTotal;           // 需要的数据字节数
            public UInt32 dwChanRecved;       // 已经收到的数据数
            public IntPtr pNext;
        }

        // 定义属性参数数组
        public struct CFGPARA
        {
            public NET2991.NET2991_AI_PARAM AIParam;            // AI参数
            public CHANNEL_INFO pChanInfo;                                // 通道信息链表
            public CHANNEL_INFO pChanHead;

            public Int32 nReadOffset;                                                  // 读取偏移
            public Int32 nReadLength;                                                 // 读取长度
            public Int32 bAllSameParam;                                             // 其它板卡与此相同,用此可以进行其它板卡参数同类化

            public Int32 bSel;                                                               // 选择采集此板卡数据
            public Int32 bOnLine;                                                        // 在线
            public Int32 bCalibration;                                                    // 是否选择校准
            public Int32 bDisplay;                                                         // 是否数字/图形显示,在连续模式下使用
            public Int32 bAIStatus;                                                       // 是否得到状态,有限模式下有效
            public Int32 nGetStsFailCnt;                                               // 得到状态失败次数
            public Int32 bAIStsTimeOut;                                              // 得到AI状态超时是否超时
            public Int32 dwAIStsTimeOut;                                            // AI状态超时时间
            public Int32 nSampStatus;                                                   // 采集完成
            public Int32 dwChanCnt;                                                    // 实际采集的通道数
            public Int32 dwReadDataSize;                                             // 现在读取的数据数
            public Int32 dwRealReadLen;                                              // 实际读取的数据长度
            public Int32 dwFrameLen;                                                   // 读取每包数据的长度
            public IntPtr hFile;                                                               // 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
            public Byte[] svFileName;
            public IntPtr hDevice;                                                          // 设备句柄
        }

        // 读取状态
        public const Int32 CMD_UNCPT = 0;                            // 读取未完成
        public const Int32 CMD_CPT = 4;                                 // 读取完成

        // 有限下数据的开始位置
        public const Int32 DATA_START = 8;
        public const Int32 LMT_FRMCNT = 1288;
    }
}
