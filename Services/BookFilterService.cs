using LibrayBackEnd.Models.Books;

namespace LibrayBackEnd.Services
{
    public class BookFilterService : IBookFilterServices

    {
        public async Task<IEnumerable<Book>> FilterBooks(FilterQuery filters, IEnumerable<Book> books)
        {
            if (filters.GenreFilter != null)
            {
                books = await FilterBooksOnGenre(filters.GenreFilter, books);
            }

            if (filters.SearchQuery != null)
            {
                books = await FilterBooksOnSearch(filters.SearchQuery, books);
            }
            if (filters.SortFilter != null)
            {
                books = await FilterBooksOnSort(filters.SortFilter, books);

            }

            return books;

        }

        public async Task<IEnumerable<Book>> FilterBooksOnGenre(string genreQuery, IEnumerable<Book> books)
        {
            var booksWithGenre = new List<Book>();

            foreach (var book in books)
            {
                if (book.Genre == genreQuery)
                {
                    booksWithGenre.Add(book);
                }
            }

            return booksWithGenre;
        }

        public async Task<IEnumerable<Book>> FilterBooksOnSearch(string searchQuery, IEnumerable<Book> books)
        {
            return books.Where(_ => _.Title.ToLower().Contains(searchQuery.ToLower())
                || _.Author.ToLower().Contains(searchQuery.ToLower())).ToList();
        }

        public async Task<IEnumerable<Book>> FilterBooksOnSort(string sortQuery, IEnumerable<Book> books)
        {
            if (sortQuery == "Price+")
            {
                books = books.OrderBy(r => r.Price).ToList();
            }

            if (sortQuery == "Price-")
            {
                books = books.OrderByDescending(r => r.Price).ToList();
            }

            if (sortQuery == "Release+")
            {
                books = books.OrderBy(r => r.Published).ToList();
            }

            if (sortQuery == "Release-")
            {
                books = books.OrderByDescending(r => r.Published).ToList();
            }
            return books;
   
        }

        public async Task<IEnumerable<Book>> UniqueBookList(IEnumerable<Book> books)
        {
            return books.GroupBy(x => x.Title).Select(x => x.First()).ToList();

        }

        public async Task<int> BookNameCount(string value, IEnumerable<BookResponseDto> books)
        {

            return books.Where(r => r.Title == value && r.IsAvailable == true).Count();
        }

    }
}
