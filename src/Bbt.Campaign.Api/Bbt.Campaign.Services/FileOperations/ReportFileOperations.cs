using Bbt.Campaign.Public.Dtos.Report;
using ClosedXML.Excel;

namespace Bbt.Campaign.Services.FileOperations
{
    public class ReportFileOperations 
    {
        public static byte[] GetCampaignReportListExcel(List<CampaignReportListDto> campaignList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Kampanya Listesi");

                FileOperations.HeaderSetsListe(worksheet, "A", "Kampanya Adı", 50);
                FileOperations.HeaderSetsListe(worksheet, "B", "Kampanya Kodu", 20);
                FileOperations.HeaderSetsListe(worksheet, "C", "Başlama Tarihi", 20);
                FileOperations.HeaderSetsListe(worksheet, "D", "Bitiş Tarihi", 20);
                FileOperations.HeaderSetsListe(worksheet, "E", "Aktif", 20);
                FileOperations.HeaderSetsListe(worksheet, "F", "Birleştirilebilir", 20);
                FileOperations.HeaderSetsListe(worksheet, "G", "Program Tipi", 20);
                FileOperations.HeaderSetsListe(worksheet, "H", "Sözleşme", 20);
                FileOperations.HeaderSetsListe(worksheet, "I", "Sözleşme ID", 20);
                FileOperations.HeaderSetsListe(worksheet, "J", "Sektör", 20);
                FileOperations.HeaderSetsListe(worksheet, "K", "Sıralama", 20);
                FileOperations.HeaderSetsListe(worksheet, "L", "Görüntüleme", 20);
                FileOperations.HeaderSetsListe(worksheet, "M", "Dahil Olma Şekli", 20);
                FileOperations.HeaderSetsListe(worksheet, "N", "Katılım Sağlanma Adedi", 20);
                FileOperations.HeaderSetsListe(worksheet, "O", "Kazanım Tipi", 20);
                FileOperations.HeaderSetsListe(worksheet, "P", "Kazanım Tutarı", 20);
                FileOperations.HeaderSetsListe(worksheet, "Q", "Kazanım Oranı", 20);
                FileOperations.HeaderSetsListe(worksheet, "R", "Çatı Limit", 20);

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
                    worksheet.Cell(currentRow, column).Value = campaign.ViewOptionName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = campaign.JoinTypeName;
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
                
                
                FileOperations.HeaderSetsListe(worksheet, "A", "Kampanya Kodu", 20);
                FileOperations.HeaderSetsListe(worksheet, "B", "Kampanya Adı", 30);
                FileOperations.HeaderSetsListe(worksheet, "C", "Aktif", 20);
                FileOperations.HeaderSetsListe(worksheet, "D", "Birleştirilebilir", 20);
                FileOperations.HeaderSetsListe(worksheet, "E", "Kampanyaya Katıldığı Tarih", 20);
                FileOperations.HeaderSetsListe(worksheet, "F", "Müşteri No", 20);
                FileOperations.HeaderSetsListe(worksheet, "G", "TCKN", 20);
                FileOperations.HeaderSetsListe(worksheet, "H", "Kazanıma Hak Kazandığı Tarih", 20);
                FileOperations.HeaderSetsListe(worksheet, "I", "Kazanım Tutarı", 20);
                FileOperations.HeaderSetsListe(worksheet, "J", "Kazanım Oranı", 20);
                FileOperations.HeaderSetsListe(worksheet, "K", "Müşteri Tipi", 20);
                FileOperations.HeaderSetsListe(worksheet, "L", "Şube", 20);
                FileOperations.HeaderSetsListe(worksheet, "M", "İş Kolu", 20);
                FileOperations.HeaderSetsListe(worksheet, "N", "Kazanım Tipi", 20);
                FileOperations.HeaderSetsListe(worksheet, "O", "Kazanımdan Yararlanılan Tarih", 20);

                int currentRow = 1;
                int column = 1;
                foreach (var customerCampaign in customerCampaignList)
                {
                    currentRow++;
                    column = 1;

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
                    worksheet.Cell(currentRow, column).Value = customerCampaign.JoinDateStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerIdentifier; 
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerCode;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.EarningReachDateStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.AchievementAmountStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.AchievementRateStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerTypeName;
                    worksheet.Column($"{column}").Width = 20;
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
                    worksheet.Cell(currentRow, column).Value = customerCampaign.AchievementTypeName;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.AchievementDateStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    result = stream.ToArray();

                    //File.WriteAllBytes(@"C:\Files2\xxx.xlsx", result);
                }
            }
            return result;
        }

        public static byte[] GetCustomerCampaignReportListExcel(List<CustomerCamapignReportListDto> customerCampaignList)
        {
            byte[] result = null;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Müşteri Listesi");


                FileOperations.HeaderSetsListe(worksheet, "A", "Kampanya Kodu", 20);
                FileOperations.HeaderSetsListe(worksheet, "B", "Kampanya Adı", 30);
                FileOperations.HeaderSetsListe(worksheet, "C", "Aktif", 20);
                FileOperations.HeaderSetsListe(worksheet, "D", "Birleştirilebilir", 20);
                FileOperations.HeaderSetsListe(worksheet, "E", "TCKN", 20);
                FileOperations.HeaderSetsListe(worksheet, "F", "Ayrıldı", 20);
                FileOperations.HeaderSetsListe(worksheet, "G", "Kampanya Başlangıç Tarihi", 20);
                FileOperations.HeaderSetsListe(worksheet, "H", "Kampanyaya Katıldığı Tarih", 20);
                FileOperations.HeaderSetsListe(worksheet, "I", "Kampanyadan Ayrıldığı Tarih", 20);

                int currentRow = 1;
                int column = 1;
                foreach (var customerCampaign in customerCampaignList)
                {
                    currentRow++;
                    column = 1;

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
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerId;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.IsExited ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CampaignStartDate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerJoinDate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = customerCampaign.CustomerExitDate;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    result = stream.ToArray();

                    //File.WriteAllBytes(@"C:\Files2\xxx.xlsx", result);
                }
            }
            return result;
        }

        public static byte[] GetTargetReportListExcel(List<TargetReportListDto> targetReportList) 
        {
            byte[] result = null;
            using (var workbook = new XLWorkbook()) 
            {
                var worksheet = workbook.Worksheets.Add($"Hedef Raporu");

                FileOperations.HeaderSetsListe(worksheet, "A", "Hedef Adı", 50);
                FileOperations.HeaderSetsListe(worksheet, "B", "Kampanya / Program", 50);
                FileOperations.HeaderSetsListe(worksheet, "C", "Programa Dahil Mi?", 20);
                FileOperations.HeaderSetsListe(worksheet, "D", "Müşteri No", 20);
                FileOperations.HeaderSetsListe(worksheet, "E", "Alt Kırılım", 30);
                FileOperations.HeaderSetsListe(worksheet, "F", "Harcama Hedefi", 20);
                FileOperations.HeaderSetsListe(worksheet, "G", "Hedef Tuttu Mu?", 20);
                FileOperations.HeaderSetsListe(worksheet, "H", "Kalan Harcama", 20);
                FileOperations.HeaderSetsListe(worksheet, "I", "Hedefin Gerçekleştiği Tarih", 30);

                int currentRow = 1;
                int column = 1;
                foreach (var item in targetReportList) 
                {
                    currentRow++;
                    column = 1;

                    worksheet.Cell(currentRow, column).Value = item.TargetName;
                    worksheet.Column($"{column}").Width = 50;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.CampaignName;
                    worksheet.Column($"{column}").Width = 50;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.IsJoin ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.CustomerCode;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.IdentitySubTypeName;
                    worksheet.Column($"{column}").Width = 30;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.TargetAmountStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.IsTargetSuccess ? "Evet" : "Hayır";
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.RemainAmountStr;
                    worksheet.Column($"{column}").Width = 20;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;

                    column++;
                    worksheet.Cell(currentRow, column).Value = item.TargetSuccessDateStr;
                    worksheet.Column($"{column}").Width = 30;
                    worksheet.Cell(currentRow, column).Style.Alignment.WrapText = true;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    result = stream.ToArray();

                    //File.WriteAllBytes(@"C:\Files\TargetReport.xlsx", result);
                }
            }
            return result;
        }
    }
}
