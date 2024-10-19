using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.DTOs;
using PizzaCompany.Interface;
using PizzaCompany.Mapper;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public OrdersController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        //Get count of all orders
        [HttpGet("count")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetCount()
        {
            return Ok(await _uow.OrderRepository.GetCount());
        }
        //Get all orders
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var (orders, count) = await _uow.OrderRepository.GetAllOrders();
            return Ok(new { Orders = orders, TotalCount = count });
        }
        //Get my orders
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            var orders = await _uow.OrderRepository.Get();
            if(!orders.Any())
            {
                return Ok("No order");
            }
            return Ok(orders.Select( o => new
            {
                Id = o.Id,
                Product = o.Product.Name,
                Quantity = o.Quantity,
                TotalPrice = o.Product.Price * o.Quantity,
            }));
        }
        // Create a new order
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] OrderDTO MyOrder)
        {
            var product = await _uow.ProductRepository.GetByName(MyOrder.ProductName);
            if (product == null)
            {
                return BadRequest("Product does not exist");
            }

            var order = await _uow.OrderRepository
                    .CreateOrder(OrderMapper.ConvertToOrder(MyOrder, product));

            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get),
                new { id = order.Id },
                OrderMapper.ConvertToUserOrdersDTO(order));
        }
    }
}
