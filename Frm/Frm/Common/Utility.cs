using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Frm.Common
{
    public class Utility
    {
        public static bool DataGridViewShowToExcel(DataGridView dgv,bool isShowExcel)
        {
            if (dgv.Rows.Count==0)
            {
                return false;
            }
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Application.Workbooks.Add(true);
            excel.Visible = isShowExcel;
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                excel.Cells[1, i + 1] = dgv.Columns[i].HeaderText;

            }
            for (int i = 0; i < dgv.RowCount; i++)
            {
                for (int j = 0;j < dgv.ColumnCount; j++)
                {
                    if (dgv[j,i].ValueType==typeof(string))
                    {
                        excel.Cells[i + 2, j + 1] = "'" + dgv[j, i].Value.ToString();

                    }
                    else
                    {
                        excel.Cells[i + 2, j + 1] = dgv[j, i].Value.ToString();
                    }
                }
            }
            return true;
        }
    }
}
