using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Frm.Common
{
    public class UserDef
    {
        // ͨ����Ϣ
        public struct CHANNEL_INFO
        {
            public Byte nChan;                        // ͨ����
            public UInt32 dwChanTotal;           // ��Ҫ�������ֽ���
            public UInt32 dwChanRecved;       // �Ѿ��յ���������
            public IntPtr pNext;
        }

        // �������Բ�������
        public struct CFGPARA
        {
            public NET2991.NET2991_AI_PARAM AIParam;            // AI����
            public CHANNEL_INFO pChanInfo;                                // ͨ����Ϣ����
            public CHANNEL_INFO pChanHead;

            public Int32 nReadOffset;                                                  // ��ȡƫ��
            public Int32 nReadLength;                                                 // ��ȡ����
            public Int32 bAllSameParam;                                             // �����忨�����ͬ,�ô˿��Խ��������忨����ͬ�໯

            public Int32 bSel;                                                               // ѡ��ɼ��˰忨����
            public Int32 bOnLine;                                                        // ����
            public Int32 bCalibration;                                                    // �Ƿ�ѡ��У׼
            public Int32 bDisplay;                                                         // �Ƿ�����/ͼ����ʾ,������ģʽ��ʹ��
            public Int32 bAIStatus;                                                       // �Ƿ�õ�״̬,����ģʽ����Ч
            public Int32 nGetStsFailCnt;                                               // �õ�״̬ʧ�ܴ���
            public Int32 bAIStsTimeOut;                                              // �õ�AI״̬��ʱ�Ƿ�ʱ
            public Int32 dwAIStsTimeOut;                                            // AI״̬��ʱʱ��
            public Int32 nSampStatus;                                                   // �ɼ����
            public Int32 dwChanCnt;                                                    // ʵ�ʲɼ���ͨ����
            public Int32 dwReadDataSize;                                             // ���ڶ�ȡ��������
            public Int32 dwRealReadLen;                                              // ʵ�ʶ�ȡ�����ݳ���
            public Int32 dwFrameLen;                                                   // ��ȡÿ�����ݵĳ���
            public IntPtr hFile;                                                               // 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
            public Byte[] svFileName;
            public IntPtr hDevice;                                                          // �豸���
        }

        // ��ȡ״̬
        public const Int32 CMD_UNCPT = 0;                            // ��ȡδ���
        public const Int32 CMD_CPT = 4;                                 // ��ȡ���

        // ���������ݵĿ�ʼλ��
        public const Int32 DATA_START = 8;
        public const Int32 LMT_FRMCNT = 1288;
    }
}
