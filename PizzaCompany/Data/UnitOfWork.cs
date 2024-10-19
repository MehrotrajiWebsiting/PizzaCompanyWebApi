using PizzaCompany.Interface;
using PizzaCompany.Repository;

namespace PizzaCompany.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PizzaCompanyDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public UnitOfWork(PizzaCompanyDbContext context,IHttpContextAccessor contextAccessor,IConfiguration configuration,IPasswordHasher passwordHasher)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public IUserRepository UserRepository => new UserRepository(_context,_configuration,_contextAccessor,_passwordHasher);

        public IOrderRepository OrderRepository => new OrderRepository(_context,_configuration,_contextAccessor);

        public IProductRepository ProductRepository => new ProductRepository(_context);

        public IRoleRepository RoleRepository => new RoleRepository(_context);

        public IRoleChangeRequestRepository RoleChangeRequestRepository => new RoleChangeRequestRepository(_context,_contextAccessor);

        public async Task<bool> SaveChangesAsync()
        {
            // When changes made it returns value > 0
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
