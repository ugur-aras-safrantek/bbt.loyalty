using ClosedXML.Excel;


namespace Bbt.Campaign.Services.FileOperations
{
    public static class FileOperations
    {
        public static void HeaderSetsListe(IXLWorksheet worksheet, string column, string headerValue, int width)
        {
            worksheet.Range($"{column}1").Style.Font.Bold = true;
            worksheet.Range($"{column}1").Style.Fill.SetBackgroundColor(XLColor.FromArgb(234, 157, 147));
            worksheet.Column($"{column}").Width = width;
            worksheet.Range($"{column}1").Value = headerValue; 
            worksheet.Range($"{column}1").Style.Alignment.SetWrapText(true);
            worksheet.Range($"{column}1").Style.Alignment.Vertical = (XLAlignmentVerticalValues.Center);

            worksheet.Columns().AdjustToContents();
        }
    }
}
