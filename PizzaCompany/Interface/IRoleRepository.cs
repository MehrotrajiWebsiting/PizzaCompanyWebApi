using Microsoft.AspNetCore.Mvc;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Interface
{
    public interface IRoleRepository
    {
        // Get all roles
        Task<IEnumerable<Role>> Get();
        // Get Role by Id
        Task<Role> Get(int id);
        // Get Role by Name
        Task<Role> Get(String Name);
        //Get Users associated with Role Id
        Task<IEnumerable<User>> GetUser(int id);
        // Create Role
        Task<Role> Create(Role role);
        // Delete Role
        void Delete(Role role);
    }
}
