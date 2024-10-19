using Microsoft.AspNetCore.Mvc;
using PizzaCompany.DTOs;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Interface
{
    public interface IUserRepository
    {
        Task<User> Get(int id);
        Task Register(User user);
        Task<User> GetByEmail(String email);
        String GenerateToken(User user);
        bool Login(User user,LoginDTO loginDTO); 
        Task<IEnumerable<String>> GetRoles();
    }
}
