using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Data;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PizzaCompanyDbContext _context;
        public RoleRepository(PizzaCompanyDbContext context)
        {
            _context = context;
        }
        // Get all roles
        public async Task<IEnumerable<Role>> Get()
        {
            return await _context.Roles.Where(r => r.IsActive == true).ToListAsync();
        }
        // Get Role By Id
        public async Task<Role> Get(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && r.IsActive == true);
        }
        // Get Role By Name
        public async Task<Role> Get(String Name)
        {
            return await _context.Roles.Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.RoleName.ToLower() == Name.ToLower() 
                                            && r.IsActive == true);
        }
        // Get User having Role with id 
        public async Task<IEnumerable<User>> GetUser(int id)
        {
            return await _context.UsersRoles
                                 .Include(x => x.User)
                                 .Where(ur => ur.RoleId == id)
                                 .Select(ur => ur.User)
                                 .ToListAsync();
        }
        // Create role
        public async Task<Role> Create(Role role)
        {
            await _context.AddAsync(role);
            return role;
        }
        // SoftDelete role
        public void Delete(Role role)
        {
            role.IsActive = false;
        }
    }
}
