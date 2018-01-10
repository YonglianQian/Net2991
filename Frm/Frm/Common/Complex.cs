using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frm.Common
{
    public class Complex
    {
        #region 字段
        //复数实部
        private double real = 0.0;
        //复数虚部
        private double imaginary = 0.0;
        #endregion

        #region 属性
        public double Real { get
            {
                return real;
            }
            set { real = value; } }
        public double Imaginary
        {
            get
            {
                return imaginary;
            }
            set
            {
                imaginary = value;
            }
        }
        #endregion
        #region 构造函数
        public Complex() : this(0, 0)
        {

        }
        public Complex(double dbreal) : this(dbreal, 0)
        {

        }
        public Complex(double dbreal, double dbImage)
        {
            real = dbreal;
            imaginary = dbImage;
        }
        public Complex(Complex other)
        {
            real = other.real;
            imaginary = other.imaginary;
        }

        #endregion
        #region 运算符重载
        public static Complex operator +(Complex c1, Complex c2) {
            return c1.Add(c2);
        }
        public static Complex operator -(Complex c1, Complex c2)
        {
            return c1.Substract(c2);
        }
        public static Complex operator *(Complex c1, Complex c2)
        {
            return c1.Multiply(c2);
        }
        public static bool operator ==(Complex c1, Complex c2) 
            {
             return ((c1.real==c2.real)&&(c1.imaginary==c2.imaginary));              
}
        public static bool operator !=(Complex c1,Complex c2)
        {
            return ((c1.real != c2.real) || (c1.imaginary != c2.imaginary));
        }
        public override string ToString()
        {
            if (Real==0&&Imaginary==0)
            {
                return string.Format("{0}", 0);
            }
            if (Real==0&&(Imaginary!=0))
            {
                return  string.Format("{0}i", Imaginary);
            }
            if (Real!=0&&Imaginary==0)
            {
               return  string.Format("{0}", Real);
            }
            if (Real!=0&&Imaginary<0)
            {
                return string.Format("{0}-{1}i", Real, -Imaginary);
            }
            return string.Format("{0}+{1}i", Real, Imaginary);
        }
        #endregion
        #region 公共方法
        public Complex Add(Complex c)
        {
            double x = this.real + c.real;
            double y = this.imaginary + c.imaginary;
            return new Complex(x, y);
        }
        public Complex Substract(Complex c)
        {
            double x = this.real - c.real;
            double y = this.imaginary - c.imaginary;
            return new Complex(x, y);
        }
        public Complex Multiply(Complex c)
        {
            double x = this.real * c.real - this.imaginary * c.imaginary;
            double y = this.imaginary *c.real + this.real * c.imaginary;
            return new Complex(x, y);
        }
        //取模
        public double GetModul()
        {
            return Math.Sqrt(Math.Pow(real, 2) + Math.Pow(imaginary, 2));
        }
        //取相位角
        public double GetAngle()
        {
            return Math.Atan2(imaginary, real);
            
        }
        //获取共轭复数
        public Complex Conjugate()
        {
            return new Complex(real,-imaginary);
        }
        #endregion
    }
}
