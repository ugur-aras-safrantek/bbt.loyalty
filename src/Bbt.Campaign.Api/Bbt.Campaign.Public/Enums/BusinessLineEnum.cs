using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum BusinessLineEnum
    {
        [Description("Bireysel (B)")]
        Individual_B = 1,
        [Description("Ticari (T)")]
        Commercial_T = 2,
        [Description("Dijital (X)")]
        Digital_X = 3,
        [Description("Ticari 1 (I)")]
        Commercial1_I = 4,
        [Description("Ticari 2 (P)")]
        Commercial2_P = 5,
        [Description("Ticari 3 (M)")]
        Commercial3_M = 6,
        [Description("Kurumsal (K)")]
        Corporate_K = 7,
        [Description("Kurumsal 1 (A)")]
        Corporate1_A = 8,

    }
}
