using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.RemoteServicModel
{
    public class GetAccounts
    {
        public class Response
        {
            [JsonProperty("checking")]
            public List<Account> Checking { get; set; }

            [JsonProperty("saving")]
            public List<Account> Saving { get; set; }
        }

        public class Account
        {
            [JsonProperty("accountInternetName")]
            public string AccountInternetName { get; set; }
            [JsonProperty("accountType")]
            public string AccountType { get; set; }
            [JsonProperty("actionOnDueDate")]
            public string ActionOnDueDate { get; set; }
            [JsonProperty("branch")]
            public Branch Branch { get; set; }
            [JsonProperty("businessLine")]
            public string BusinessLine { get; set; }
            [JsonProperty("channel")]
            public Channel Channel { get; set; }
            [JsonProperty("currency")]
            public string Currency { get; set; }
            [JsonProperty("iban")]
            public string Iban { get; set; }
            [JsonProperty("isPartnerAccount")]
            public bool IsPartnerAccount { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("number")]
            public string Number { get; set; }
            [JsonProperty("partnershipType")]
            public string PartnershipType { get; set; }
            [JsonProperty("product")]
            public Product Product { get; set; }
            [JsonProperty("remainingDaysToDueDate")]
            public int RemainingDaysToDueDate { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("suffix")]
            public string Suffix { get; set; }
            [JsonProperty("vdlDay")]
            public int VdlDay { get; set; }
        }

        public class Channel
        {
            [JsonProperty("code")]
            public string Code { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
        }
        public class Product
        {
            [JsonProperty("productCode")]
            public string ProductCode { get; set; }
            [JsonProperty("productName")]
            public string ProductName { get; set; }
            [JsonProperty("subProduct")]
            public Subproduct SubProduct { get; set; }
        }

        public class Subproduct
        {
            [JsonProperty("productCode")]
            public string ProductCode { get; set; }
            [JsonProperty("productName")]
            public string ProductName { get; set; }
        }

        public class OperationProfile
        {
            [JsonProperty("allowClosing")]
            public bool AllowClosing { get; set; }

            [JsonProperty("allowMoneyTransfer")]
            public bool AllowMoneyTransfer { get; set; }

            [JsonProperty("allowToBeSourceForSavingAccount")]
            public bool AllowToBeSourceForSavingAccount { get; set; }

            [JsonProperty("allowToBeCardMasterAccount")]
            public bool AllowToBeCardMasterAccount { get; set; }

            [JsonProperty("allowToBeCardAccount")]
            public bool AllowToBeCardAccount { get; set; }
        }


        public class Branch
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }



    }
}
