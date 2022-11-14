using Dapper;
using LibrayBackEnd.Models.Books;
using LibrayBackEnd.Models.Users;
using LibrayBackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LibrayBackEnd.Controllers
{
    public class BookController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IBookFilterServices _filterService;

        public BookController(IConfiguration config, IBookFilterServices bookFilter)
        {
            _config = config;
            _filterService = bookFilter;

        }

        [HttpGet]
        [Route("/books")]
        public async Task<ActionResult<List<BookResponseDto>>> GetAllBooks([FromQuery] PagingParameters pagingParameters, FilterQuery filters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            IEnumerable<Book> books = await SelectAllBooks(connection);

            var uniqueBooks = await _filterService.UniqueBookList(books);

            var filteredBooks = await _filterService.FilterBooks(filters, uniqueBooks); 

            var pagedBooks = filteredBooks.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();

            return Ok(pagedBooks);
        }

        [HttpGet]
        [Route("/books/all")]
        public async Task<ActionResult<List<Book>>> GetAll([FromQuery] PagingParameters pagingParameters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            IEnumerable<Book> books = await SelectAllBooks(connection);

            return Ok(books.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList());
        }



        [HttpGet]
        [Route("/books/{bookId}")]
        public async Task<ActionResult<Book>> GetBook(int bookId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var book = await connection.QueryFirstAsync<Book>("select * from books where id = @Id",
                new { Id = bookId});

            return Ok(book);
        }

        [HttpGet]
        [Route("/books/conditions/{bookId}")]
        public async Task<ActionResult<List<Book>>> GetOtherConditions(int bookId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var book = await connection.QueryFirstAsync<Book>("select * from books where id = @Id",
                new { Id = bookId });

            var books = await connection.QueryAsync<Book>("select * from books where title = @Title", book);

            return Ok(books);
        }

        [HttpPut]
        [Route("/book/ordered/{bookId}")]
        public async Task<ActionResult<Book>> UpdateOrderedBook(int bookId, int userId)
        {
            using var connection =  new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var book = await connection.QueryFirstAsync<Book>("select * from books where id = @Id", new { id = bookId });

            book.IsAvailable = true;

            var bookToUpdate = await connection.QueryFirstAsync<Book>("select * from books where title = @Title and quality = @Quality and isAvailable = @IsAvailable", book);

            bookToUpdate.CurrentOwnerId = 1;
            bookToUpdate.IsAvailable= false;

            await connection.ExecuteAsync("update books set title = @Title, stockNumber = @StockNumber, author = @Author, quality = @Quality, pages = @Pages, genre = @Genre, summary = @Summary, Cover = @Cover, isAvailable = @IsAvailable," +
                "published = @Published, price = @Price, currentOwnerId = @CurrentOwnerId where id = @Id", bookToUpdate);

            return Ok(bookToUpdate);

        }
        [HttpPut]
        [Route("/book/returned/{bookId}")]
        public async Task<ActionResult<Book>> UpdateReturnedBook(int bookId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var bookToUpdate = await connection.QueryFirstAsync<Book>("select * from books where id = @id", new { id = bookId });

            bookToUpdate.CurrentOwnerId = 0;
            bookToUpdate.IsAvailable = true;

            await connection.ExecuteAsync("update books set title = @Title, stockNumber = @StockNumber, author = @Author, quality = @Quality, pages = @Pages, genre = @Genre, summary = @Summary, Cover = @Cover, isAvailable = @IsAvailable, " +
                "published = @Published, price = @Price where id = @Id", bookToUpdate);

            return Ok(bookToUpdate);

        }


        [HttpGet]
        [Route("/books/quantity/{bookId}")]
        public async Task<ActionResult<int>> GetBookQuantity(int bookId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var book = await connection.QueryFirstAsync<Book>("select * from books where id = @Id",
                new { Id = bookId });

            var books = await connection.QueryAsync<Book>("select * from books where title = @Title and quality = @Quality and isAvailable = @IsAvailable", book);

            var quantity = books.Count();

            return Ok(quantity);
        }

        [HttpGet]
        [Route("/books/multiple")]
        public async Task<ActionResult<Book>> GetBooks(List<int> bookIds)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var books = new List<Book>();

            foreach (var id in bookIds)
            {
                books.Add(await connection.QueryFirstOrDefaultAsync <Book>("select * from books where id = @Id",
                  new { Id = id }));
            }

            return Ok(books);
        }


        [HttpPost]
        [Route("/books")]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var quality = book.Quality;

            var count = await BookQualityCount(book) + 1;

            book.StockNumber = $"{book.Quality}{count}";

            await connection.ExecuteAsync("insert into books (title, stockNumber, author, quality, pages, genre, summary, published, price, isAvailable, cover) " +
                "values (@Title, @StockNumber, @Author, @Quality, @Pages, @Genre, @Summary, @Published, @Price, @isAvailable, @Cover)", book);

            return Ok(await SelectAllBooks(connection));
        }

        [HttpPut]
        [Route("/books")]
        public async Task<ActionResult<Book>> UpdateBook(Book book)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("update books set title = @Title, stockNumber = @StockNumber, author = @Author, quality = @Quality, pages = @Pages, genre = @Genre, summary = @Summary, Cover = @Cover, isAvailable = @IsAvailable, " +
                "published = @Published, price = @Price where id = @Id", book);

            return Ok(await SelectAllBooks(connection));
        }

        [HttpDelete]
        [Route("/books/{bookId}")]
        public async Task<ActionResult<Book>> DeleteBook(int bookId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("delete from books where id = @Id", new { Id = bookId });
            return Ok(await SelectAllBooks(connection));
        }

      


        private static async Task<IEnumerable<Book>> SelectAllBooks(SqlConnection connection)
        {
            return await connection.QueryAsync<Book>("select * from books");
        }

        public async Task<int> BookQualityCount(Book book)
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var count = connection.Query<Book>("select * from books where quality = @Quality", book).Count();

            return count;

        }
    }
}
