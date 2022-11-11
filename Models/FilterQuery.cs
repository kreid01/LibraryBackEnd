namespace LibrayBackEnd.Models
{
    public class FilterQuery
    {
        public string? SearchQuery { get; set; }

        public string? GenreFilter { get; set; }

        public string? SortFilter { get; set; }
    }
}
