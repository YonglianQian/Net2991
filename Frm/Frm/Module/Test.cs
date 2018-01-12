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

namespace Frm.Module
{
    public partial class Test : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public Test()
        {
            InitializeComponent();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
        Random random = new Random();
        double value1 = 10.0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            SeriesPoint[] pointsToUpdate1 = new SeriesPoint[200];
            DateTime argu = DateTime.Now;
            for (int i = 0; i < 200; i++)
            {
                pointsToUpdate1[i] = new SeriesPoint(argu, value1);
                argu = argu.AddMilliseconds(1);
                value1 = value1 + (random.NextDouble() * 10.0 - 5.0);
            }
            DateTime minDate = argu.AddSeconds(-100);
            int pointsToRemoveCount = 0;
            foreach (SeriesPoint point in chart.Series[0].Points)
            {
                if (point.DateTimeArgument < minDate)
                {
                    pointsToRemoveCount++;
                }
            }
            if (pointsToRemoveCount < chart.Series[0].Points.Count)
            {
                pointsToRemoveCount--;
            }
            chart.Series[0].Points.AddRange(pointsToUpdate1);
            if (pointsToRemoveCount > 0)
            {
                chart.Series[0].Points.RemoveRange(0, pointsToRemoveCount);
            }
            SwiftPlotDiagram diagram = chart.Diagram as SwiftPlotDiagram;
            if (diagram != null && (diagram.AxisX.DateTimeScaleOptions.MeasureUnit == DateTimeMeasureUnit.Millisecond || diagram.AxisX.DateTimeScaleOptions.ScaleMode == ScaleMode.Continuous))
            {
                diagram.AxisX.WholeRange.SetMinMaxValues(minDate, argu);
            }
        }
    }
}