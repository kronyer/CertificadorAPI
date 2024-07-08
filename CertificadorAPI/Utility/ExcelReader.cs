using OfficeOpenXml;

namespace Reading;
public class ExcelReader
{
    public static List<string> ReadUserNameFromExcel(string filePath, string columnName)
    {
        List<string> columnData = new List<string>();

        FileInfo file = new FileInfo(filePath);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


        using (ExcelPackage package = new ExcelPackage(file))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); //é a primeira e unica 

            int columnIndex = FindColumnIndex(worksheet, columnName);

            if (columnIndex != -1)
            {
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string cellValue = worksheet.Cells[row, columnIndex].Value?.ToString().Trim();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        columnData.Add(cellValue);
                    }
                }
            }


        }
        return columnData;

    }

    static int FindColumnIndex(ExcelWorksheet worksheet, string columnName)
    {
        int columnIndex = -1;
        int totalColumns = worksheet.Dimension.Columns;

        for (int col = 1; col <= totalColumns; col++) // Ajuste na condição aqui
        {
            string headerValue = worksheet.Cells[1, col].Value?.ToString().Trim();
            if (headerValue != null && headerValue.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                columnIndex = col;
                break;
            }
        }
        return columnIndex;
    }

}