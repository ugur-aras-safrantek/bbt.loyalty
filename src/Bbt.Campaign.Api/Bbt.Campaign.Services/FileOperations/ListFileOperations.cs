using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Dtos.Target;
using ClosedXML.Excel;


namespace Bbt.Campaign.Services.FileOperations
{
    public class ListFileOperations : FileOperations
    {
        public static  byte[] GetCampaignListExcel(List<CampaignListDto> campaignList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Kampanya Listesi");

                HeaderSetsListe(worksheet, "A", "Kampanya Adı", 50);
                HeaderSetsListe(worksheet, "B", "Kampanya Kodu", 20);
                HeaderSetsListe(worksheet, "C", "Sözleşme ID", 20);
                HeaderSetsListe(worksheet, "D", "Başlama Tarihi", 20);
                HeaderSetsListe(worksheet, "E", "Bitiş Tarihi", 20);
                HeaderSetsListe(worksheet, "F", "Program Tipi", 20);
                HeaderSetsListe(worksheet, "G", "Aktif", 20);
                HeaderSetsListe(worksheet, "H", "Birleştirilebilir", 20);

                int currentRow = 1;
                int column = 1;
                foreach (var campaign in campaignList) 
                {
                    currentRow++;
                    column = 1;

                    worksheet.Cell(currentRow, column).Value = campaign.Name;
                    worksheet.Column($"{column}").Width = 50;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.Code;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = ((campaign.ContractId ?? 0) == 0) ? "" : campaign.ContractId.ToString();
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.StartDate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.EndDate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.ProgramType;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.IsActive ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.IsBundle ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    result = stream.ToArray();

                    //File.WriteAllBytes(@"C:\Files\xxx.xlsx", result);
                }
            }
            
            return result;
        }

        public static byte[] GetTargetListExcel(List<TargetListDto> targetList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook()) 
            {
                var worksheet = workbook.Worksheets.Add($"Hedef Listesi");

                HeaderSetsListe(worksheet, "A", "Hedef Tanımı İsmi", 50);
                HeaderSetsListe(worksheet, "B", "Hedef ID", 20);
                HeaderSetsListe(worksheet, "C", "Hedef Gösterim Tipi", 20);
                HeaderSetsListe(worksheet, "D", "Akış", 20);
                HeaderSetsListe(worksheet, "E", "Sorgu", 20);
                HeaderSetsListe(worksheet, "F", "Aktif", 20);

                int currentRow = 1;
                int column = 1;
                foreach (var target in targetList) 
                {
                    currentRow++;
                    column = 1;

                    worksheet.Cell(currentRow, column).Value = target.Name;
                    worksheet.Column($"{column}").Width = 50;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = target.Id.ToString();
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = target.TargetViewType;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = target.Flow ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = target.Query ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = target.IsActive ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    result = stream.ToArray();

                    //File.WriteAllBytes(@"C:\Files\Target.xlsx", result);
                }
            }
            return result;
        }

        public static byte[] GetTopLimitListExcel(List<CampaignTopLimitListDto> topLimitList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook()) 
            {
                var worksheet = workbook.Worksheets.Add($"Çatı Limiti Listesi");

                HeaderSetsListe(worksheet, "A", "Çatı Limiti Adı", 50);
                HeaderSetsListe(worksheet, "B", "Kazanım Sıklığı", 20);
                HeaderSetsListe(worksheet, "C", "Para Birimi", 20);
                HeaderSetsListe(worksheet, "D", "Çatı Max Tutar", 20);
                HeaderSetsListe(worksheet, "E", "Çatı Max Yararlanma", 20);
                HeaderSetsListe(worksheet, "F", "Çatı Oranı", 20);
                HeaderSetsListe(worksheet, "G", "Tutar", 20);
                HeaderSetsListe(worksheet, "H", "Oran", 20);
                HeaderSetsListe(worksheet, "I", "Aktif", 20);

                int currentRow = 1;
                int column = 1;
                foreach (var topLimit in topLimitList)
                {
                    currentRow++;
                    column = 1;


                    worksheet.Cell(currentRow, column).Value = topLimit.Name;
                    worksheet.Column($"{column}").Width = 50;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.AchievementFrequency;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.Currency;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.MaxTopLimitAmount;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.MaxTopLimitUtilization;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.MaxTopLimitRate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.Amount ? "Evet" : "Hayır"; 
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.Rate ? "Evet" : "Hayır"; 
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = topLimit.IsActive ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    result = stream.ToArray();

                    //File.WriteAllBytes(@"C:\Files\Çatı Limit Listesi.xlsx", result);
                }
            }
            return result;
        }

        
    }
}
