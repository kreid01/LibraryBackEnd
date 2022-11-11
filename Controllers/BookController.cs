using Dapper;
using LibrayBackEnd.Models;
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
        [Route("books")]
        public async Task<ActionResult<List<Book>>> GetAllBooks([FromQuery] PagingParameters pagingParameters, FilterQuery filters)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            IEnumerable<Book> books = await SelectAllBooks(connection);

            var uniqueBooks = await _filterService.UniqueBookList(books);

            var filteredBooks = await _filterService.FilterBooks(filters, uniqueBooks); 

            var pagedBooks = filteredBooks.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();

            return Ok(pagedBooks);
        }



        [HttpGet]
        [Route("books/{bookId}")]
        public async Task<ActionResult<Book>> GetBook(int bookId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var book = await connection.QueryFirstAsync<Book>("select * from books where id = @Id",
                new { Id = bookId});

            return Ok(book);
        }


        [HttpPost]
        [Route("books")]
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
        [Route("books")]
        public async Task<ActionResult<Book>> UpdateBook(Book book)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("update books set title = @Title, stockNumber = @StockNumber, author = @Author, quality = @Quality, pages = @Pages, genre = @Genre, summary = @Summary, Cover = @Cover, isAvailable = @IsAvailable, " +
                "published = @Published, price = @Price where id = @Id", book);

            return Ok(await SelectAllBooks(connection));
        }

        [HttpDelete]
        [Route("books/{bookId}")]
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
