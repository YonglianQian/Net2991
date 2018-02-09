using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

namespace TestMatlab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MWArray[] Result;
            MWNumericArray deltaAmp, Max_DeltaAmp, Vpp;

            Delta.Class1 dc = new Delta.Class1();
            Result = dc.Delta_amp(3, 32, 1024000, "C:\\201802073.dat");
            deltaAmp = (MWNumericArray)Result[0];
            Max_DeltaAmp = (MWNumericArray)Result[1];
            Vpp = (MWNumericArray)Result[2];
            double[] DeltaAmpArr = (double[])(deltaAmp.ToVector(MWArrayComponent.Real));
            double Max_DeltaAmp1 = Max_DeltaAmp.ToScalarDouble();
            double[] VppArr = (double[])(Vpp.ToVector(MWArrayComponent.Real));

            foreach (var item in DeltaAmpArr)
            {
                listBox1.Items.Add(item);
            }
            listBox2.Items.Add(Max_DeltaAmp1);
            foreach (var item in VppArr)
            {
                listBox3.Items.Add(item);
            }
        }
    }
}
