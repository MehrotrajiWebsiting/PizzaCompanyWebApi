using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Filters.ActionFilters;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ProductsController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        // READ PRODUCTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok((await _uow.ProductRepository.GetAll()).Select(p => new
            {
                p.Id,p.Name,p.Price
            }));
        }
        //Only admin can Create Products
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ServiceFilter(typeof(Product_ValidateExistingProductFilterAttribue))]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (Convert.ToDecimal(product.Price) == 0)
            {
                return BadRequest("Price cannot be 0");
            }
            var createdProduct = await _uow.ProductRepository.Create(product);

            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducts),
                new { Id = createdProduct.Id },
                createdProduct);
        }
        //Only admin can Update Products
        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var existingProduct = await _uow.ProductRepository.GetByName(product.Name);
            if (existingProduct == null)
            {
                return NotFound();
            }
            if (Convert.ToDecimal(product.Price) == 0)
            {
                return BadRequest("Price cannot be 0");
            }
            _uow.ProductRepository.Update(existingProduct, product);
            await _uow.SaveChangesAsync();

            return Ok(existingProduct);
        }
        //Only admin can SoftDelete Product
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct([FromBody] String ProductName)
        {
            if (string.IsNullOrEmpty(ProductName))
            {
                return BadRequest("Enter Product");
            }
            var existingProduct = await _uow.ProductRepository.GetByName(ProductName);
            if (existingProduct == null)
            {
                return NotFound();
            }
            // Soft
            _uow.ProductRepository.Delete(existingProduct);
            await _uow.SaveChangesAsync();

            return Ok(existingProduct);
        }
        //Get users which have ordered a particular product
        [HttpGet("User/{id}")]
        [Authorize(Roles = "admin")]
        [ServiceFilter(typeof(Product_ValidateProductIdFilterAttribute))]
        public async Task<IActionResult> GetUserWithProduct(int Id)
        {
            return Ok(await _uow.ProductRepository.GetUserWithProduct(Id));
        }   
        [HttpGet("User")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUserProduct()
        {
            return Ok(await _uow.ProductRepository.GetAllUserProduct());
        }
    }
}
