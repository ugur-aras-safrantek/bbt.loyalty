﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target
{
    public class TargetParameterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int TargetViewTypeId { get; set; }



        //public decimal? UsedAmount { get; set; }
        public string? UsedAmountStr { get; set; }
        public string?  UsedAmountCurrencyCode { get; set; }
        
               
        public string? RemainAmountStr { get; set; }
        public string? RemainAmountCurrencyCode { get; set; }

       
        
        
        //public int? UsedNumberOfTransaction { get; set; }
        public int? Percent { get; set; }


        public string? TargetAmountStr { get; set; }
        public string? TargetAmountCurrencyCode { get; set; }


        public string? TargetDetailEn { get; set; }
        public string? TargetDetailTr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionTr { get; set; }
        public string? Description { get; set; }
    }
}
