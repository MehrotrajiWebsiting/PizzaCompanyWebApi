using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Data;
using System.Data;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage;
using PizzaCompany.DTOs;

namespace PizzaCompany.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PizzaCompanyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public OrderRepository(PizzaCompanyDbContext context, IConfiguration configuration,
                IHttpContextAccessor contextAccessor)
        {
            this._context = context;
            this._configuration = configuration;
            this._contextAccessor = contextAccessor;
        }
        //Get Count of all orders
        public async Task<int> GetCount()
        {
            return await _context.GetCount();
        }
        //Get All Orders with count
        public async Task<(List<OrderStoredProcedureDTO>, int)> GetAllOrders()
        {
            return await _context.GetAllOrders();
        }
        //Get LoggedIn UserId from token
        private string GetUserIdFromToken()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            var userClaims = identity.Claims; //stores the claims array

            return userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        //Get my orders
        public async Task<IEnumerable<Order>> Get()
        {
            //Get userId
            int userId = int.Parse(GetUserIdFromToken());
            //Get order for userId
            return await _context.Orders.Include(o=>o.Product)
                                        .Where(o => o.UserId == userId)
                                        .ToListAsync();
        }

        // Create a new order
        public async Task<Order> CreateOrder(Order MyOrder)
        {
            int userId = int.Parse(GetUserIdFromToken());
            MyOrder.UserId = userId;

            await _context.AddAsync(MyOrder);

            return MyOrder;
        }
    }
}
