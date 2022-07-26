using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignIdentity;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Dtos.Target;
using ClosedXML.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Bbt.Campaign.Services.FileOperations
{
    public class ListFileOperations 
    {

        public static byte[] GetCampaignListExcel2(List<CampaignListDto> campaignList)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells[1, 1].Value = "Id";
            workSheet.Cells[1, 2].Value = "Name";

            workSheet.Cells[2, 1].Value = 1;
            workSheet.Cells[2, 2].Value = "test";

            //string p_strPath = @"C:\Files\xxx.xlsx";
            //FileStream objFileStrm = File.Create(p_strPath);
            //objFileStrm.Close();



            byte[] bt = excel.GetAsByteArray();

            excel.Dispose();

            return bt;
        }


        public static  byte[] GetCampaignListExcel(List<CampaignListDto> campaignList)
        {
            
            byte[] result = null;
            try 
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Kampanya Listesi");

                    FileOperations.HeaderSetsListe(worksheet, "A", "Kampanya Adı", 50);
                    FileOperations.HeaderSetsListe(worksheet, "B", "Kampanya Kodu", 20);
                    FileOperations.HeaderSetsListe(worksheet, "C", "Sözleşme ID", 20);
                    FileOperations.HeaderSetsListe(worksheet, "D", "Başlama Tarihi", 20);
                    FileOperations.HeaderSetsListe(worksheet, "E", "Bitiş Tarihi", 20);
                    FileOperations.HeaderSetsListe(worksheet, "F", "Program Tipi", 20);
                    FileOperations.HeaderSetsListe(worksheet, "G", "Aktif", 20);
                    FileOperations.HeaderSetsListe(worksheet, "H", "Birleştirilebilir", 20);

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
            }

            
            catch(Exception ex) 
            {
                throw new Exception(ex.ToString());
            }
            
            return result;
        }

        public static byte[] GetTargetListExcel(List<TargetListDto> targetList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook()) 
            {
                var worksheet = workbook.Worksheets.Add($"Hedef Listesi");

                FileOperations.HeaderSetsListe(worksheet, "A", "Hedef Tanımı İsmi", 50);
                FileOperations.HeaderSetsListe(worksheet, "B", "Hedef ID", 20);
                FileOperations.HeaderSetsListe(worksheet, "C", "Hedef Gösterim Tipi", 20);
                FileOperations.HeaderSetsListe(worksheet, "D", "Akış", 20);
                FileOperations.HeaderSetsListe(worksheet, "E", "Sorgu", 20);
                FileOperations.HeaderSetsListe(worksheet, "F", "Aktif", 20);

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

                    //File.WriteAllBytes(@"C:\Files2\Target.xlsx", result);
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

                FileOperations.HeaderSetsListe(worksheet, "A", "Çatı Limiti Adı", 50);
                FileOperations.HeaderSetsListe(worksheet, "B", "Kampanya Adı", 50);
                FileOperations.HeaderSetsListe(worksheet, "C", "Kazanım Sıklığı", 20);
                FileOperations.HeaderSetsListe(worksheet, "D", "Para Birimi", 20);
                FileOperations.HeaderSetsListe(worksheet, "E", "Çatı Max Tutar", 20);
                FileOperations.HeaderSetsListe(worksheet, "F", "Çatı Max Yararlanma", 20);
                FileOperations.HeaderSetsListe(worksheet, "G", "Çatı Oranı", 20);
                FileOperations.HeaderSetsListe(worksheet, "H", "Tutar", 20);
                FileOperations.HeaderSetsListe(worksheet, "I", "Oran", 20);
                FileOperations.HeaderSetsListe(worksheet, "J", "Aktif", 20);

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
                    worksheet.Cell(currentRow, column).Value = topLimit.CampaignNames;
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

        public static byte[] GetCampaignIdentityListExcel(List<CampaignIdentityListDto> campaignIdentityList) 
        {
            byte[] result = null;
            try 
            {
                using (var workbook = new XLWorkbook()) 
                {
                    var worksheet = workbook.Worksheets.Add($"Kampanya TCKN Listesi");

                    FileOperations.HeaderSetsListe(worksheet, "A", "Kampanya/Program", 50);
                    FileOperations.HeaderSetsListe(worksheet, "B", "Alt Kırılım", 30);
                    FileOperations.HeaderSetsListe(worksheet, "C", "TCKN", 20);

                    int currentRow = 1;
                    int column = 1;
                    foreach (var campaignIdentity in campaignIdentityList) 
                    {
                        currentRow++;
                        column = 1;

                        worksheet.Cell(currentRow, column).Value = campaignIdentity.CampaignName;
                        worksheet.Column($"{column}").Width = 50;
                        worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                        column++;
                        worksheet.Cell(currentRow, column).Value = campaignIdentity.IdentitySubTypeName;
                        worksheet.Column($"{column}").Width = 30;
                        worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                        column++;
                        worksheet.Cell(currentRow, column).Value = campaignIdentity.Identities;
                        worksheet.Column($"{column}").Width = 20;
                        worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);

                        result = stream.ToArray();

                        //File.WriteAllBytes(@"C:\Files\TCKN-Listesi.xlsx", result);
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return result;
        }
    }
}
