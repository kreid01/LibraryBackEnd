namespace LibrayBackEnd.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public DateTime Created { get; set; }
    }
}
