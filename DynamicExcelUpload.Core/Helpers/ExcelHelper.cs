using OfficeOpenXml;
using System.Data;
using System.Linq;

namespace DynamicExcelUpload.Core.Helpers
{
    public class ExcelHelper
    {
        public static int GetTotalColumnCountByAnyNonNullData(ExcelWorksheet sheet)
        {
            var column = sheet.Dimension.End.Column;
            while (column >= 1)
            {
                var range = sheet.Cells[1, column, 1, sheet.Dimension.End.Column];
                if (range.Any(c => !string.IsNullOrEmpty(c.Text)))
                {
                    break;
                }
                column--;
            }
            return column;
        }

        public static int GetTotalRowCountByAnyNonNullData(ExcelWorksheet sheet)
        {
            var row = sheet.Dimension.End.Row;
            while (row >= 1)
            {
                var range = sheet.Cells[row, 1, row, sheet.Dimension.End.Column];
                if (range.Any(c => !string.IsNullOrEmpty(c.Text)))
                {
                    break;
                }
                row--;
            }
            return row;
        }

        public static DataTable GetDataTableFromExcel(ExcelPackage package, bool hasHeader = true)
        {
            var ws = package.Workbook.Worksheets.First();
            DataTable tbl = new DataTable();
            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
            {
                var type = firstRowCell.Value.GetType();
                tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column), type);
            }
            var startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                DataRow row = tbl.Rows.Add();
                foreach (var cell in wsRow)
                {
                    row[cell.Start.Column - 1] = cell.Text;
                }
            }
            return tbl;
        }
    }
}
