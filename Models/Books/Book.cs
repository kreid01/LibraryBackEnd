using System.Data.SqlTypes;

namespace LibrayBackEnd.Models.Books
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string StockNumber { get; set; }

        public string Quality { get; set; }

        public string Author { get; set; }

        public int Pages { get; set; }

        public decimal Price { get; set; }

        public string Genre { get; set; }

        public string Summary { get; set; }

        public string Published { get; set; }

        public bool IsAvailable { get; set; }

        public string Cover { get; set; }
        
        public int CurrentOwnerId { get; set; }

    }

}
