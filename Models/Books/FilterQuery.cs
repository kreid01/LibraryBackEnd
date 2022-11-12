namespace LibrayBackEnd.Models.Books
{
    public class FilterQuery
    {
        public string? SearchQuery { get; set; }

        public string? GenreFilter { get; set; }

        public string? SortFilter { get; set; }
    }
}
