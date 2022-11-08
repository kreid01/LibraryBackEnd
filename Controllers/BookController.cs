using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LibrayBackEnd.Controllers
{
    public class BookController : ControllerBase
    {
        private readonly IConfiguration _config;

        public BookController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("books")]
        public async Task<ActionResult<List<Book>>> GetAllBooks()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            IEnumerable<Book> books = await SelectAllBooks(connection);

            return Ok(books);
        }

      

        [HttpGet]
        [Route("books/{id}")]
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

            await connection.ExecuteAsync("insert into books (title, stockNumber, author, quality, pages, genre, summary, published, price) " +
                "values (@Title, @StockNumber, @Author, @Quality, @Pages, @Genre, @Summary, @Published, @Price)", book);

            return Ok(await SelectAllBooks(connection));
        }

        [HttpPut]
        [Route("books")]
        public async Task<ActionResult<Book>> UpdateBook(Book book)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("update books set title = @Title, stockNumber = @StockNumber, author = @Author, quality = @Quality, pages = @Pages, genre = @Genre, summary =@Summary, " +
                "published = @Published, price = @Price where id = @Id", book);

            return Ok(await SelectAllBooks(connection));
        }

        [HttpDelete]
        [Route("books/{id}")]
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


    }
}
