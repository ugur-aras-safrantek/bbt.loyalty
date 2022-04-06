using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum JoinTypeEnum
    {
        [Description("Tüm Müşteriler")]
        AllCustomers = 1,
        [Description("Müşteri Özelinde")]
        Customer = 2,        
        [Description("İş Kolu Özelinde")]
        BusinessLine = 3,
        [Description("Şube Özelinde")]
        Branch = 4,        
        [Description("Müşteri Tipi Özelinde")]
        CustomerType = 5,
    }
}
