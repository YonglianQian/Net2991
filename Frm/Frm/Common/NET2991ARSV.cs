using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Frm.Common
{
    public class NET2991ARSV
    {
        // ������ͷ�ļ�, ����Ϊ�û��ͳ����ṩ��һ������ġ���ֵ�ģ�����ķ���
        // �����������£����ǽ���������ʹ��NET2991.h�еĽӿں�����(RSV=Reserve)
        // ��ͷ�ļ��е����ݣ����ǲ��ṩ����ļ���֧��
        // ����FILE_Create()�Ĳ���nOptMode���õ��ļ�������ʽ(֧��"��"ָ��ʵ�ֶ��ַ�ʽ���в���)
        public const Int32 NET2991_FILE_OPTMODE_CREATE_NEW = 1;                    // �����ļ�,����ļ�����������
        public const Int32 NET2991_FILE_OPTMODE_CREATE_ALWAYS = 2;             // �����ļ��Ƿ���ڣ�����Ҫ������(�����ܸ�дǰһ���ļ�)
        public const Int32 NET2991_FILE_OPTMODE_OPEN_EXISTING = 3;                // �򿪱����Ѿ����ڵ��ļ�
        public const Int32 NET2991_FILE_OPTMODE_OPEN_ALWAYS = 4;                 // ���ļ��������ļ����ڣ��򴴽���

        // ����FILE_SetOffset()�Ĳ���nBaseMode���õ��ļ�ָ���ƶ��ο�����
        public const Int32 NET2991_FILE_BASEMODE_BEGIN = 0;                           // ���ļ������Ϊ�ο�������ƫ��
        public const Int32 NET2991_FILE_BASEMODE_CURRENT = 1;                     // ���ļ��ĵ�ǰλ����Ϊ�ο������������ƫ��(nOffsetBytes<0ʱ����ƫ�ƣ�>0ʱ����ƫ��)
        public const Int32 NET2991_FILE_BASEMODE_END = 2;                               // ���ļ���β����Ϊ�ο�������ƫ��

        // �ļ�״̬�ṹ��
        public struct FILESTATUS
        {
            public UInt64 nFileSize;                  // �ļ���С
            public UInt32 nFileStatus;               // �ļ���ȡ״̬
        }

        // ################################ AIУ׼���� ################################
        // AI����У׼, �ɹ�ʱ����TRUE,���򷵻�FALSE,�ɵ���GetLastError()��������ԭ��
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_AI_SelfCal(IntPtr hDevice);        // �豸������,����DEV_Create()��������

        // ################################ �õ��豸�汾��Ϣ���� ################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_DEV_GetVersion(                                        // ����豸�汾��Ϣ, �ɹ�ʱ����TRUE,���򷵻�FALSE,�ɵ���GetLastError()��������ԭ��
                                                                                        IntPtr hDevice,                  // �豸������,����DEV_Create()��������
                                                                                        ref UInt32 pDllVer,            // ���صĶ�̬��(.dll)�汾��
                                                                                        ref UInt32 pDriverVer,       // ���ص�����(.sys)�汾��, ��û�еײ��ר�õ�.sys�ļ������Է���socket�İ汾
                                                                                        ref UInt32 pFirmwareVer);// ���صĹ̼��汾��

        // ################################ �ļ����� ##################################
        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_FILE_Create(                                      // �����ļ�·�����ļ���
                                                                                IntPtr hDevice,                  // �豸������,����DEV_Create()��������
                                                                                String szFilePath,               // �ļ�·��
                                                                                String szFileName);           // �ļ���

        [DllImport("NET2991.DLL")]
        public static extern Int32 NET2991_FILE_Close(                                      // �����ļ�����ʱ����󣬹ر��ļ�
                                                                                IntPtr hDevice);                // �豸������,����DEV_Create()��������
    }
}
