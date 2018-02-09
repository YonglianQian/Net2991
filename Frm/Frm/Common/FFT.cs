using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frm.Common
{
    public class FFT
    {
        /// <summary>
        /// 一维频率抽取基2 FFT
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="countN"></param>
        /// <returns></returns>
        public static Complex[] fft_frequence(Complex[] sourceData, int countN)
        {
            int r = Convert.ToInt32(Math.Log(countN, 2));
            Complex[] interVar1 = new Complex[countN];
            Complex[] interVar2 = new Complex[countN];
            interVar1 = (Complex[])sourceData.Clone();

            //w代表旋转因子
            Complex[] w = new Complex[countN / 2];
            for (int i = 0; i < countN / 2; i++)
            {
                double angle = -i * Math.PI * 2 / countN;
                w[i] = new Complex(Math.Cos(angle), Math.Sin(angle));
            }

            //蝶形运算
            for (int i = 0; i < r; i++)
            {
                int interval = 1 << i;

                int halfN = 1 << (r - i);
                for (int j = 0; j < interval; j++)
                {
                    int gap = j * halfN;
                    for (int k = 0; k < halfN / 2; k++)
                    {
                        interVar2[k + gap] = interVar1[k + gap] + interVar1[k + gap + halfN / 2];
                        interVar2[k + halfN / 2 + gap] = (interVar1[k + gap] - interVar1[k + gap + halfN / 2]) * w[k * interval];
                    }
                }
                //结果拷贝到输入端，为下次迭代做好准备
                interVar1 = (Complex[])interVar2.Clone();

            }
            //将输出码位倒置
            for (uint j = 0; j < countN; j++)
            {
                uint rev = 0;
                uint num = j;
                for (int i = 0; i < r; i++)
                {
                    rev <<= 1;
                    rev |= num & 1;
                    num >>= 1;
                }
                interVar2[rev] = interVar1[j];
            }
            return interVar2;
        }

        //基2快速傅里叶逆变换
        public static Complex[] ifft_frequency(Complex[] sourceData, int countN)
        {
            for (int i = 0; i < countN; i++)
            {
                sourceData[i] = sourceData[i].Conjugate();
            }
            Complex[] interVar = new Complex[countN];
            interVar = fft_frequence(sourceData, countN);
            for (int i = 0; i < countN; i++)
            {
                interVar[i] = new Complex(interVar[i].Real / countN, -interVar[i].Imaginary / countN);
            }
            return interVar;
        }
    }
}
