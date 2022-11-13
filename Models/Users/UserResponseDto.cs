namespace LibrayBackEnd.Models.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }

        public bool? IsAdmin { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
