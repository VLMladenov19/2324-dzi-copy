namespace HAR.Data.Models
{
    public class RentProduct
    {
        public Guid RentId { get; set; }
        public Rent Rent { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int ProductQuantity { get; set; }
        public int RentMonths { get; set; }
    }
}
