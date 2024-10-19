using PizzaCompany.DTOs;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Mapper
{
    public static class OrderMapper
    {
        public static UserOrdersDTO ConvertToUserOrdersDTO(Order o)
        {
            return new UserOrdersDTO()
            {
                Id = o.Id,
                Product = o.Product,
                Quantity = o.Quantity,
            };
        }
        public static Order ConvertToOrder(OrderDTO orderDTO, Product product)
        {
            return new Order
            {
                Product = product,
                Quantity = orderDTO.Quantity,
            };
        }
    }
}
