using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum ActionOptionsEnum
    {
        [Description("Ödeme Cashback")]
        PaymentCashback = 1,
        [Description("Fatura Cashback")]
        InvoiceCashback = 2,
    }
}
