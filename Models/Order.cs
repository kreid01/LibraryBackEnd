
namespace LibrayBackEnd.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int AddressId { get; set; }

        public int? UserId { get; set; }

        public bool IsBorrowing { get; set; }

        public decimal TotalPrice { get; set; }

        public string BookIds { get; set; }

        public DateTime Created { get; set; }
    }
}
