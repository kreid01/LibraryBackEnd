namespace LibrayBackEnd.Models
{
    public class Address
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string Postcode { get; set; }

        public string City { get; set; }
    }
}
