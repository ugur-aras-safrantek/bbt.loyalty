using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Customer
{
    public class GoalResultByCustomerIdAndMonthCount
    {
        public Total Total { get; set; }
        public List<Months> Months { get; set; }
    }
    public class Total 
    { 
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
    }

    public class Months 
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }

    }
}
