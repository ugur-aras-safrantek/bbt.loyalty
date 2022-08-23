﻿using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Paging;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Bbt.Campaign.Core.Helper
{
    public static class Helpers
    {
        public static string dateTimeFormat = "dd-MM-yyyy hh:mm:ss";

        public static PagingDto Paging(int totalItems, int currentPage, int pageSize = 10)
        {
            if (totalItems <= 0)
                return new PagingDto();

            if (pageSize == 0)
                pageSize = 10;
            int maxPages = 10;
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            if (currentPage < 1)
            {
                currentPage = 1;
            }
            else if (currentPage > totalPages)
            {
                currentPage = totalPages;
            }
            int startPage, endPage;
            if (totalPages <= maxPages)
            {
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // total pages more than max so calculate start and end pages
                var maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / (decimal)2);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / (decimal)2) - 1;
                if (currentPage <= maxPagesBeforeCurrentPage)
                {
                    // current page near the start
                    startPage = 1;
                    endPage = maxPages;
                }
                else if (currentPage + maxPagesAfterCurrentPage >= totalPages)
                {
                    // current page near the end
                    startPage = totalPages - maxPages + 1;
                    endPage = totalPages;
                }
                else
                {
                    // current page somewhere in the middle
                    startPage = currentPage - maxPagesBeforeCurrentPage;
                    endPage = currentPage + maxPagesAfterCurrentPage;
                }
            }

            var pager = new PagingDto
            {
                TotalItems = totalItems,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                StartPage = startPage,
                EndPage = endPage,
                HasNextPage = !(currentPage == endPage),
                HasPreviousPage = !(currentPage == startPage),
            };

            return pager;
        }

        //public static string ConvertDotFormatDatetimeString(string dateStr) 
        //{
        //    //9-6-2022  06/09/2022    IFormatProvider culture = new CultureInfo("en-US");
        //    //"13-06-2019 00:00:00"

        //    string[] dateArray = dateStr.Split('-');
        //    //string day = dateArray[0].PadLeft(2, '0');
        //    //string month = dateArray[1].PadLeft(2, '0');
        //    string day = dateArray[0].PadLeft(2, '0');
        //    string month = dateArray[1].PadLeft(2, '0');
        //    string year = dateArray[2];

        //    dateStr = string.Concat(day, "-", month, "-", year, " 00:00:00");


        //    //return int.Parse(dateArray[1]).ToString() + "-" + int.Parse(dateArray[0]).ToString()
        //    //    + "-" + int.Parse(dateArray[2]).ToString();

        //    //return DateTime.ParseExact(dateStr, "dd-MM-yyyy", null).ToShortDateString();

        //    return dateStr;
        //}

        //13-06-2019 formatında gelen tarihi döndürür
        public static string ConvertBackEndDateTimeToStringForUI(DateTime inputDate) 
        {
            string day = inputDate.Day.ToString().PadLeft(2, '0');
            string month = inputDate.Month.ToString().PadLeft(2, '0');
            string year = inputDate.Year.ToString();
            string seperator = "-";
            string retVal = string.Concat(day, seperator, month, seperator, year);
            return retVal;
        }

        public static DateTime ConvertUIDateTimeStringForBackEnd(string inputDate) 
        {
            string[] dateArray = inputDate.Split('-');
            string day = dateArray[0].PadLeft(2, '0');
            string month = dateArray[1].PadLeft(2, '0');
            string year = dateArray[2];
            inputDate = string.Concat(day, "-", month, "-", year, " 00:00:00");
            return DateTime.ParseExact(inputDate, dateTimeFormat, CultureInfo.InvariantCulture);
        }

        public static string ConvertWithTimeForBackEnd(DateTime inputDate) 
        {
            string day = inputDate.Day.ToString().PadLeft(2, '0');
            string month = inputDate.Month.ToString().PadLeft(2, '0');
            string year = inputDate.Year.ToString();
            string hour = inputDate.Hour.ToString().PadLeft(2, '0');
            string minute = inputDate.Minute.ToString().PadLeft(2, '0');
            string second = inputDate.Second.ToString().PadLeft(2, '0');

            return string.Concat(day, "-", month, "-", year, " ", hour, ":", minute, ":", second);
        }


        public static DateTime ConvertDateTimeToShortDate(DateTime inputDate)
        {
            return new DateTime(inputDate.Year, inputDate.Month, inputDate.Day);
        }

        public static string TarihDonustur(this DateTime tarih)
        {
            if (tarih != null)
                return tarih.ToString("dd/MM/yyyy HH:mm");
            return "";
        }

        public static string GetDatabaseDateString(DateTime tarih) 
        {
            return tarih.ToString("yyyy-MM-dd");
        }

        public static string GetDatabaseDateTimeString(DateTime tarih)
        {
            return tarih.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string SaatsizTarihDonustur(this DateTime tarih)
        {
            if (tarih != null)
                return tarih.ToString("dd/MM/yyyy");
            return "";
        }

        public static string SaatsizTarihDonustur(this DateTime? tarih)
        {
            if (tarih != null)
                return tarih.Value.ToString("dd/MM/yyyy");
            return "";
        }

        public static string TarihDonustur(this DateTime? tarih)
        {
            if (tarih != null)
                return tarih.Value.ToString("dd/MM/yyyy HH:mm");
            return "";
        }
        public static DateTime? TarihDonustur(this string tarih)
        {
            if (!string.IsNullOrEmpty(tarih))
                return Convert.ToDateTime(tarih);
            return null;
        }

        #region Decimal Metodlar

        public static decimal? ConvertNullableDecimal(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            return Convert.ToDecimal(data);
        }

        //nullable price string with thousand seperator
        public static string? ConvertNullablePriceString(decimal? data)
        {
            if (data == null)
                data = 0;
            NumberFormatInfo nfi = CultureInfo.CreateSpecificCulture("tr-TR").NumberFormat;
            return data?.ToString("N2", nfi);
        }

        public static decimal ConvertDecimal(string data)
        {
            if (string.IsNullOrEmpty(data))
                return 0;
            return Convert.ToDecimal(data);
        }
        public static decimal ConvertStringToDecimal(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return 0;
            return Convert.ToDecimal(data);
        }

        public static string ConvertPriceToString(this decimal? data)
        {
            if (data == null)
                return "0,00";
            return data.Value.ToString().Replace('.', ',');
        }
        public static string ConvertPriceToString(this decimal data)
        {
            return data.ToString().Replace('.', ',');
        }

        public static string PriceString(this decimal? data)
        {

            return data.GetValueOrDefault(0).ToString("N");
        }
        public static string PriceString(this decimal data)
        {
            return data.ToString("N");
        }
        public static string StringOutDecimal(this decimal data)
        {

            return data.ToString("N0");
        }
        #endregion
        public static DateTime? ConvertToNullableDatetime(this string x)
        {
            if (!string.IsNullOrEmpty(x))
            {
                return Convert.ToDateTime(x);
            }

            return null;
        }
        public static int TarihFarkHesapla(DateTime bas, DateTime bitis)
        {
            TimeSpan Sonuc = bitis - bas;

            return Convert.ToInt32(Sonuc.TotalDays);

        }


        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string StringToDecimalString(string? input) 
        {
            if (string.IsNullOrEmpty(input))
                return null;

            return input?.Replace(".", "").Replace(",", ".");
        }

        public static string CreateCampaignCode() 
        { 
            return Guid.NewGuid().ToString().ToUpper().Replace("-", "").Substring(0, 10);
        }

        #region Enum
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static string GetEnumDescription<TEnum>(int value)
        {
            return GetEnumDescription((Enum)(object)((TEnum)(object)value));  // ugly, but works
        }


        public static bool ListByFilterValidation(object input)
        {
            if (input != null)
            {
                var properties = input.GetType().GetProperties();
                foreach (var item in properties)
                {
                    if (item.Name?.ToLower() == "PageNumber" || item.Name?.ToLower() == "PageSize" || item.Name?.ToLower() == "TotalCount")
                        continue;

                    var itemType = item.PropertyType;
                    var itemValue = item.GetValue(input);

                    if (itemValue != null)
                    {
                        if (itemType == typeof(int?))
                        {
                            if (((int?)itemValue).Value > 0)
                                return true;
                        }
                        else if (itemType == typeof(int))
                        {
                            if (((int)itemValue) > 0)
                                return true;
                        }
                        else if (itemType == typeof(string))
                        {
                            if (!string.IsNullOrWhiteSpace(itemValue as string))
                                return true;
                        }
                        else if (itemType == typeof(uint?))
                        {
                            if (((uint?)itemValue) > 0)
                                return true;
                        }
                        else if (itemType == typeof(uint))
                        {
                            if (((uint)itemValue) > 0)
                                return true;
                        }
                        else if (itemType == typeof(DateTime))
                        {
                            if (((DateTime)itemValue) > DateTime.MinValue)
                                return true;
                        }
                        else if (itemType == typeof(DateTime?))
                        {
                            if (((DateTime?)itemValue) > DateTime.MinValue)
                                return true;
                        }
                    }

                }

            }

            return false;
        }
        public static void ListByFilterCheckValidation(object input)
        {
            var allowedFields = new List<string>() { "pagenumber", "pagesize", "totalcount", "sortdir", "sortby" };
            if (input != null)
            {
                var properties = input.GetType().GetProperties();
                foreach (var item in properties)
                {
                    if (allowedFields.Contains(item.Name?.ToLower()))
                        continue;

                    var itemType = item.PropertyType;
                    var itemValue = item.GetValue(input);

                    if (itemValue != null)
                    {
                        if (itemType == typeof(int?))
                        {
                            if (((int?)itemValue).Value > 0)
                                return;
                        }
                        else if (itemType == typeof(int))
                        {
                            if (((int)itemValue) > 0)
                                return;
                        }
                        else if (itemType == typeof(string))
                        {
                            if (!string.IsNullOrWhiteSpace(itemValue as string))
                                return;
                        }
                        else if (itemType == typeof(uint?))
                        {
                            if (((uint?)itemValue) > 0)
                                return;
                        }
                        else if (itemType == typeof(uint))
                        {
                            if (((uint)itemValue) > 0)
                                return;
                        }
                        else if (itemType == typeof(DateTime))
                        {
                            if (((DateTime)itemValue) > DateTime.MinValue)
                                return;
                        }
                        else if (itemType == typeof(DateTime?))
                        {
                            if (((DateTime?)itemValue) > DateTime.MinValue)
                                return;
                        }
                        else if (itemType == typeof(bool?))
                        {
                            if (((bool?)itemValue).HasValue)
                                return;
                        }
                    }

                }

            }

            //throw new Exception("En az bir kriter girilmelidir.");
        }
        public static bool ibanKontrol(string ibanNo)
        {
            try
            {
                ibanNo = ibanNo.ToUpper();
                if (String.IsNullOrEmpty(ibanNo))
                    return false;
                else if (System.Text.RegularExpressions.Regex.IsMatch(ibanNo, "^[A-Z0-9]"))
                {
                    ibanNo = ibanNo.Replace(" ", String.Empty);
                    string bank =
                    ibanNo.Substring(4, ibanNo.Length - 4) + ibanNo.Substring(0, 4);
                    int asciiShift = 55;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (char c in bank)
                    {
                        int v;
                        if (Char.IsLetter(c)) v = c - asciiShift;
                        else v = int.Parse(c.ToString());
                        sb.Append(v);
                    }
                    string checkSumString = sb.ToString();
                    int checksum = int.Parse(checkSumString.Substring(0, 1));
                    for (int i = 1; i < checkSumString.Length; i++)
                    {
                        int v = int.Parse(checkSumString.Substring(i, 1));
                        checksum *= 10;
                        checksum += v;
                        checksum %= 97;
                    }
                    return checksum == 1;
                }

                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool TcAuthentication(string input)
        {

            string tcKimlikNo = input.Trim();
            bool returnvalue = false;
            if (tcKimlikNo.Length == 11)
            {
                Int64 ATCNO, BTCNO, TcNo;
                long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

                TcNo = Int64.Parse(tcKimlikNo);

                ATCNO = TcNo / 100;
                BTCNO = TcNo / 100;

                C1 = ATCNO % 10; ATCNO = ATCNO / 10;
                C2 = ATCNO % 10; ATCNO = ATCNO / 10;
                C3 = ATCNO % 10; ATCNO = ATCNO / 10;
                C4 = ATCNO % 10; ATCNO = ATCNO / 10;
                C5 = ATCNO % 10; ATCNO = ATCNO / 10;
                C6 = ATCNO % 10; ATCNO = ATCNO / 10;
                C7 = ATCNO % 10; ATCNO = ATCNO / 10;
                C8 = ATCNO % 10; ATCNO = ATCNO / 10;
                C9 = ATCNO % 10; ATCNO = ATCNO / 10;
                Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
                Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

                returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
            }
            return returnvalue;
        }

        public static Boolean FirmaVergiKontrol(string input)
        {
            if (input == null || input.Length != 10) return false;
            var vkn = input.ToArray();
            if (!vkn.All(n => char.IsNumber(n))) return false;

            var lastDigit = Convert.ToInt32(vkn[9].ToString());
            int total = 0;
            for (int i = 9; i >= 1; i--)
            {
                int digit = Convert.ToInt32(vkn[9 - i].ToString());
                var v1 = ((digit + i) % 10);
                int v11 = (int)(v1 * Math.Pow(2, i)) % 9;
                if (v1 != 0 && v11 == 0) v11 = 9;
                total += v11;
            }

            total = (total % 10 == 0) ? 0 : (10 - (total % 10));
            return (total == lastDigit);
        }

        public static List<SelectListItemDTO> EnumToList<T>() where T : System.Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList().Select(x =>
                      new SelectListItemDTO
                      {
                          Key = Convert.ToInt32(x),
                          Value = GetEnumDescription(x)
                      }).ToList();
        }
        public static List<SeedParameterDto> EnumToObjectList<T>() where T : System.Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList().Select(x =>
                      new SeedParameterDto
                      {
                          Id = Convert.ToInt32(x),
                          CreatedBy = "1",
                          CreatedOn = DateTime.Now,
                          IsDeleted = false,
                          Name = GetEnumDescription(x)
                      }).ToList();
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }

        //Identify if a string is a number 
        public static bool IsNumeric(string input) 
        { 
            return input.All(char.IsDigit);
        }

        public static bool IsTwoIntegerListEqual(List<int> sourceList, List<int> targetList) 
        { 
            bool retVal;

            if(sourceList == null)
                sourceList = new List<int>();
            if(targetList == null)
                targetList = new List<int>();

            if(sourceList.Count != targetList.Count)
                retVal = true;
            else 
                retVal = sourceList.Except(targetList).Any(); 

            return retVal;
        }

        public static bool IsTwoStringListEqual(List<string> sourceList, List<string> targetList)
        {
            bool retVal;

            if (sourceList == null)
                sourceList = new List<string>();
            if (targetList == null)
                targetList = new List<string>();

            if (sourceList.Count != targetList.Count)
                retVal = true;
            else
                retVal = sourceList.Except(targetList).Any();

            return retVal;
        }

        #endregion
    }
}
