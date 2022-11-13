using Dapper;
using LibrayBackEnd.Models;
using LibrayBackEnd.Models.Books;
using LibrayBackEnd.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;

namespace LibrayBackEnd.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("orders")]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
          
                
            return await connection.QueryAsync<Order>("select * from orders");
        }

        [HttpPost]
        [Route("orders")]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            order.Created = DateTime.UtcNow;

            await connection.ExecuteAsync("insert into orders (userId, bookIds, addressId, created, isBorrowing) " +
                "values (@UserId, @BookIds, @AddressId, @Created, @IsBorrowing)", order);

            return Ok(order);
        }

        [HttpGet]
        [Route("orders/{userId}")]
        public async Task<IEnumerable<Order>> GetUserOrder(int userId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return await connection.QueryAsync<Order>("select * from orders where userId = @userId", new { userId = userId });
         
        }

        [HttpDelete]
        [Route("orders/{orderId}")]
        public async Task<ActionResult<Book>> DeleteOrder(int orderId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("delete from orders where id = @Id", new { Id = orderId });
            return Ok();
        }

    }
}
