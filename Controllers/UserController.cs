using AutoMapper;
using Dapper;
using LibrayBackEnd.Models;
using LibrayBackEnd.Models.Books;
using LibrayBackEnd.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LibrayBackEnd.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("users")]
        public async Task<IEnumerable<User>> GetUsers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return await connection.QueryAsync<User>("select * from users");
        }



        [HttpPost]
        [Route("/register")]

        public async Task<ActionResult<User>> Register(UserRequestDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            request.PasswordHash = passwordHash;
            request.PasswordSalt = passwordSalt;
            request.IsAdmin = false;

            await connection.ExecuteAsync("insert into users (firstName, lastName, email, passwordHash, passwordSalt, isAdmin) " +
                    "values (@FirstName, @LastName, @Email, @passwordHash, @passwordSalt, @IsAdmin)", request);



            return Ok();

        }


        [HttpPut]
        [Route("/user")]
        public async Task<ActionResult<User>> UpdateUser(UserAdminDto user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var userToUpdate = await connection.QueryFirstAsync<User>("select * from users where id = @id", new { id = user.Id});

            userToUpdate.IsAdmin = true;

           await connection.ExecuteAsync("update users set firstName = @FirstName, lastName = @LastName, email = @Email, passwordHash = @PasswordHash, passwordSalt = @PasswordSalt, " +
                "isAdmin = @IsAdmin where id = @Id", userToUpdate);

            return Ok();
        }

        [HttpDelete]
        [Route("users/{userId}")]
        public async Task<ActionResult<Book>> DeleteBook(int userId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("delete from users where id = @Id", new { Id = userId });
            return Ok();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var user = await connection.QueryFirstAsync<User>("select * from users where email = @Email",
                new { Email = request.Email });

            if (user.Email != request.Email)
            {
                return BadRequest("User not found");
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);

            var response = new AuthResponseDto();

            response.Token = token;
            response.UserId = user.Id;
            response.IsAdmin = user.IsAdmin;
            return Ok(response);
        }

        [HttpGet]
        [Route("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(int userId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var user = await connection.QueryFirstAsync<User>("select * from users where id = @Id",
                new { Id = userId });

            return Ok(_mapper.Map<UserResponseDto>(user));
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, user.FirstName,
                ClaimTypes.Name, user.LastName),
                new Claim("userId", $"{user.Id}")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
   
    }
}
