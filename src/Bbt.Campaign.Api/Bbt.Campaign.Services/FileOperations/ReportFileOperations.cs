using Bbt.Campaign.Public.Dtos.Report;
using ClosedXML.Excel;

namespace Bbt.Campaign.Services.FileOperations
{
    public class ReportFileOperations : FileOperations
    {
        public static byte[] GetCampaignReportListExcel(List<CampaignReportListDto> campaignList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Kampanya Listesi");

                HeaderSetsListe(worksheet, "A", "Kampanya Adı", 50);
                HeaderSetsListe(worksheet, "B", "Kampanya Kodu", 20);
                HeaderSetsListe(worksheet, "C", "Başlama Tarihi", 20);
                HeaderSetsListe(worksheet, "D", "Bitiş Tarihi", 20);
                HeaderSetsListe(worksheet, "E", "Aktif", 20);
                HeaderSetsListe(worksheet, "F", "Birleştirilebilir", 20);
                HeaderSetsListe(worksheet, "G", "Program Tipi", 20);
                HeaderSetsListe(worksheet, "H", "Sözleşme", 20);
                HeaderSetsListe(worksheet, "I", "Sözleşme ID", 20);
                HeaderSetsListe(worksheet, "J", "Sektör", 20);
                HeaderSetsListe(worksheet, "K", "Sıralama", 20);
                HeaderSetsListe(worksheet, "L", "Katılım Sağlanma Adedi", 20);
                HeaderSetsListe(worksheet, "M", "Kazanım Tipi", 20);
                HeaderSetsListe(worksheet, "N", "Kazanım Tutarı", 20);
                HeaderSetsListe(worksheet, "O", "Kazanım Oranı", 20);
                HeaderSetsListe(worksheet, "P", "Çatı Limit", 20);

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
                    worksheet.Cell(currentRow, column).Value = campaign.StartDate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.EndDate;
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

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.ProgramTypeName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.IsContract ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = ((campaign.ContractId ?? 0) == 0) ? "" : campaign.ContractId.ToString();
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.SectorName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.Order;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.CustomerCount;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.AchievementTypeName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.AchievementAmount;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.AchievementRate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.TopLimitName;
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
        public static byte[] GetCustomerReportListExcel(List<CustomerReportListDto> customerCampaignList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Müşteri Listesi");

                HeaderSetsListe(worksheet, "A", "Müşteri No", 10);
                HeaderSetsListe(worksheet, "B", "TCKN", 15);
                HeaderSetsListe(worksheet, "C", "Müşteri Tipi", 20);
                HeaderSetsListe(worksheet, "D", "Kazanım Tipi", 20);
                HeaderSetsListe(worksheet, "E", "Kampanya Başlama Dönemi", 25);
                HeaderSetsListe(worksheet, "F", "Şube", 20);
                HeaderSetsListe(worksheet, "G", "İş Kolu", 20);
                HeaderSetsListe(worksheet, "H", "Kazanımdan Yararlanılan Tarih", 30);
                HeaderSetsListe(worksheet, "I", "Kampanya Kodu", 20);
                HeaderSetsListe(worksheet, "J", "Kampanya Adı", 30);
                HeaderSetsListe(worksheet, "K", "Aktif", 20);
                HeaderSetsListe(worksheet, "L", "Birleştirilebilir", 20);
                HeaderSetsListe(worksheet, "M", "Kampanya Yürürlükte Mi?", 25);
                HeaderSetsListe(worksheet, "N", "Kampanyaya Katıldığı Tarih", 25);

                int currentRow = 1;
                int column = 1;
                foreach (var customerCampaign in customerCampaignList)
                {
                    currentRow++;
                    column = 1;

                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerCode;
                    worksheet.Column($"{column}").Width = 10;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerIdentifier;
                    worksheet.Column($"{column}").Width = 15;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerTypeName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.AchievementTypeName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CampaignStartDateStr;
                    worksheet.Column($"{column}").Width = 25;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.BranchName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.BusinessLineName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.AchievementDateStr;
                    worksheet.Column($"{column}").Width = 30;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CampaignCode;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CampaignName;
                    worksheet.Column($"{column}").Width = 30;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.IsActive ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.IsBundle ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.IsContinuingCampaign ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 25;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.JoinDateStr;
                    worksheet.Column($"{column}").Width = 25;
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
    }
}
