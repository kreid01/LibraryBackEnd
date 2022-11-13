using LibrayBackEnd.Models.Books;
using LibrayBackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;

namespace LibrayBackEnd.Controllers
{
    public class AddressController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AddressController(IConfiguration config)
        {
            _config = config;
        }


        [HttpDelete]
        [Route("addresses/{addressId}")]
        public async Task<ActionResult<Address>> DeleteAddress(int addressId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("delete from addresses where id = @Id", new { Id = addressId });
            
            return Ok();
        }

        [HttpGet]
        [Route("addresses")]
        public async Task<IEnumerable<Address>> GetAddresses()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return await connection.QueryAsync<Address>("select * from addresses");
        }

        [HttpPost]
        [Route("address")]
        public async Task<ActionResult<Address>> CreateAddress(Address address)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

           var existingAddress  = await connection.QueryFirstAsync<Address>("select * from addresses where userId = @UserId and AddressLine1 = @AddressLine1", new { userId = address.UserId, addressLine1 = address.AddressLine1 });

            if (existingAddress == null)
            {

                await connection.ExecuteAsync("insert into addresses (userId, firstName, lastName, addressLine1, addressLine2, postcode, city) " +
                    "values (@UserId, @FirstName, @LastName, @AddressLine1, @AddressLine2, @Postcode, @City)", address);

            }
            Address addressResponse = await connection.QueryFirstAsync<Address>("select * from addresses where userId = @UserId and AddressLine1 = @AddressLine1", new { userId = address.UserId, addressLine1 = address.AddressLine1 });

            return Ok(addressResponse.Id);
        }
    }

}
