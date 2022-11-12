using Dapper;
using LibrayBackEnd.Models;
using LibrayBackEnd.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LibrayBackEnd.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;
        public static User user = new User();


        public OrderController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
          
                
            return await connection.QueryAsync<Order>("select * from orders");
        }
    }
}
