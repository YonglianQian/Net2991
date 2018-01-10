using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Frm.Common
{
    public class NET2991ARSV
    {
        // 保留的头文件, 这是为用户和厂商提供的一种特殊的、增值的，额外的服务。
        // 绝大多数情况下，我们建议您优先使用NET2991.h中的接口函数。(RSV=Reserve)
        // 本头文件中的内容，我们不提供额外的技术支持
        // 函数FILE_Create()的参数nOptMode所用的文件操作方式(支持"或"指令实现多种方式并行操作)
        public const Int32 NET2991_FILE_OPTMODE_CREATE_NEW = 1;                    // 创建文件,如果文件存在则会出错
        public const Int32 NET2991_FILE_OPTMODE_CREATE_ALWAYS = 2;             // 不管文件是否存在，总是要被创建(即可能改写前一个文件)
        public const Int32 NET2991_FILE_OPTMODE_OPEN_EXISTING = 3;                // 打开必须已经存在的文件
        public const Int32 NET2991_FILE_OPTMODE_OPEN_ALWAYS = 4;                 // 打开文件，若该文件不在，则创建它

        // 函数FILE_SetOffset()的参数nBaseMode所用的文件指针移动参考基点
        public const Int32 NET2991_FILE_BASEMODE_BEGIN = 0;                           // 以文件起点作为参考点往右偏移
        public const Int32 NET2991_FILE_BASEMODE_CURRENT = 1;                     // 以文件的当前位置作为参考点往左或往右偏移(nOffsetBytes<0时往左偏移，>0时往右偏移)
        public const Int32 NET2991_FILE_BASEMODE_END = 2;                               // 以文件的尾部作为参考点往左偏移

        // 文件状态结构体
        public struct FILESTATUS
        {
            public UInt64 nFileSize;                  // 文件大小
            public UInt32 nFileStatus;               // 文件读取状态
        }

        // ################################ AI校准函数 ################################
        // AI自我校准, 成功时返回TRUE,否则返回FALSE,可调用GetLastError()分析错误原因
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_SelfCal(IntPtr hDevice);        // 设备对象句柄,它由DEV_Create()函数创建

        // ################################ 得到设备版本信息函数 ################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_GetVersion(                                        // 获得设备版本信息, 成功时返回TRUE,否则返回FALSE,可调用GetLastError()分析错误原因
                                                                                        IntPtr hDevice,                  // 设备对象句柄,它由DEV_Create()函数创建
                                                                                        ref UInt32 pDllVer,            // 返回的动态库(.dll)版本号
                                                                                        ref UInt32 pDriverVer,       // 返回的驱动(.sys)版本号, 因没有底层的专用的.sys文件，可以返回socket的版本
                                                                                        ref UInt32 pFirmwareVer);// 返回的固件版本号

        // ################################ 文件函数 ##################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_FILE_Create(                                      // 设置文件路径和文件名
                                                                                IntPtr hDevice,                  // 设备对象句柄,它由DEV_Create()函数创建
                                                                                String szFilePath,               // 文件路径
                                                                                String szFileName);           // 文件名

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_FILE_Close(                                      // 用于文件保存时出错后，关闭文件
                                                                                IntPtr hDevice);                // 设备对象句柄,它由DEV_Create()函数创建
    }
}
