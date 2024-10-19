namespace PizzaCompany.DTOs
{
    public class OrderStoredProcedureDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Quantity { get; set; } = 1;

        public string ProductName { get; set; } = null!;
        public decimal? ProductPrice { get; set; } = null!;

        public decimal TotalPrice
        {
            get
            {
                return (decimal)ProductPrice * (decimal)Quantity;
            }
        }
    }
}
