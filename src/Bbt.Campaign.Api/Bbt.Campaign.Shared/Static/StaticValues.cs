﻿namespace Bbt.Campaign.Shared.Static
{
    public static class StaticValues
    {
        public static string Campaign_Redis_ConStr { get; set; }
        public static int Campaign_Redis_Port { get; set; }
        public static string Campaign_Redis_Password { get; set; }
        public static string Campaign_Redis_Ttl { get; set; }
        public static string Campaign_MsSql_ConStr { get; set; }
        public static string CampaignListImageUrlDefault { get; set; }
        public static string CampaignDetailImageUrlDefault { get; set; }
        public static bool IsDevelopment { get; set; }

        public static string BranchServiceUrl { get; set; }

        public static string ChannelCodeServiceUrl { get; set; }

        public static string ContractServiceUrl { get; set; }
        public static int SessionTimeout { get; set; }


        public static string Audience { get; set; }
        public static string Issuer { get; set; }
        public static string SecurityKey { get; set; }

    }
}
