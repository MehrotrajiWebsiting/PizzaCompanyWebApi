using PizzaCompany.DTOs;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Interface
{
    public interface IOrderRepository
    {
        Task<int> GetCount();
        Task<(List<OrderStoredProcedureDTO>, int)> GetAllOrders();
        Task<IEnumerable<Order>> Get();
        Task<Order> CreateOrder(Order MyOrder);
    }
}
