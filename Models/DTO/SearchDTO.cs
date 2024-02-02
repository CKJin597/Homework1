namespace MSIT155.Models.DTO
{
    public class SearchDTO
    {
        public int? CategoryId {  get; set; }
        public string? Keyword { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortType { get; set; }
        public string? CategoryName { get; set; }


    }
}
