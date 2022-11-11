using LibrayBackEnd.Models;

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
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Book>> UniqueBookList(IEnumerable<Book> books)
        {
            return books.GroupBy(x => x.Title).Select(x => x.First()).ToList();

        }
    }
}
