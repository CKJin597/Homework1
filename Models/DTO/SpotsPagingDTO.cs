namespace MSIT155.Models.DTO
{
    public class SpotsPagingDTO
    {
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<SpotImagesSpot>? SpotsResult { get; set; }
        public string? categoriesName {  get; set; }
        public List<string>? categoriesValue { get; set; }

    }
}
