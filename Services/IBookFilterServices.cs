using LibrayBackEnd.Models.Books;

namespace LibrayBackEnd.Services
{
    public interface IBookFilterServices
    {
        Task<IEnumerable<Book>> FilterBooks(FilterQuery filters, IEnumerable<Book> books );

        Task<IEnumerable<Book>> FilterBooksOnSearch(string searchQuery, IEnumerable<Book> books);

        Task<IEnumerable<Book>> FilterBooksOnGenre(string genreQuery, IEnumerable<Book> books);

        Task<IEnumerable<Book>> FilterBooksOnSort(string sortQuery, IEnumerable<Book> books);

        Task<IEnumerable<Book>> UniqueBookList(IEnumerable<Book> books);

        Task<int> BookNameCount(string value, IEnumerable<BookResponseDto> books);

    }

}
