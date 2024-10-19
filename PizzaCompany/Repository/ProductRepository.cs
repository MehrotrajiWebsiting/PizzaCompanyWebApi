using Microsoft.EntityFrameworkCore;
using PizzaCompany.Data;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly PizzaCompanyDbContext _context;
        public ProductRepository(PizzaCompanyDbContext context)
        {
            _context = context;
        }
        // Get Product by ProductName
        public async Task<Product> GetByName(String name)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper()
                                    && x.IsActive == true);
        }
        // Get All Products
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.Where(x => x.IsActive == true).ToListAsync();
        }
        // Create new Product
        public async Task<Product> Create(Product product)
        {
            await _context.AddAsync(product);
            return product;
        }
        // Update Price of Product
        public void Update(Product existingProduct,Product product)
        {
            existingProduct.Price = product.Price;
        }
        // Delete a Product
        public void Delete(Product product)
        {
            product.IsActive = false;
        }
        // Get Users which have ordered a particular product
        public async Task<IEnumerable<object>> GetUserWithProduct(int Id)
        {
            return await(from user in _context.Users
                         join
                           orders in _context.Orders on
                           user.Id equals orders.UserId
                         join
                           product in _context.Products on
                           orders.ProductId equals product.Id
                         where product.Id == Id && product.IsActive == true
                         select new
                         {
                             user.UserEmail,
                             product.Name,
                             orders.Quantity
                         }).ToListAsync();
        }
        // Get All Users with their ordered Products sorted by ProductName
        public async Task<IEnumerable<object>> GetAllUserProduct()
        {
            return await _context.Users
                                 .Join(_context.Orders,
                                 user => user.Id,
                                 order => order.UserId,
                                 (user, order) => new { user, order }
                                 ).Join(_context.Products,
                                     userOrder => userOrder.order.Product.Id,
                                     product => product.Id,
                                     (userOrder, product) => new { userOrder, product }
                                 )
                                .Where(result => result.product.IsActive == true)
                                .OrderBy(result => result.product.Name)
                                .Select(result => new { result.userOrder.user.UserEmail, result.product.Name, result.userOrder.order.Quantity })
                                .ToListAsync();
        }
    }
}
