namespace HAR.Data.Models
{
    public class CartProduct
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public int ProductQuantity { get; set; }
        public int RentMonths { get; set; }
    }
}
