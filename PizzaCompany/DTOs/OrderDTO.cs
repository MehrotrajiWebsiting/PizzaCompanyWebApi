using PizzaCompany.Validations;

namespace PizzaCompany.DTOs
{
    public class OrderDTO
    {
        public string ProductName { get; set; } = null!;

        [Product_QuantityValidation]
        public int Quantity { get; set; } = 1;
    }
}
