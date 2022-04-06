namespace Bbt.Campaign.Public.Models.Paging
{
    public class PagingRequest
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
    }
}
