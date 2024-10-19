using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public RolesController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        //Get all Roles
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            var roles = await _uow.RoleRepository.Get();
            return Ok(roles.Select(r => new
            {
                Id = r.Id,
                Name = r.RoleName,
            }));
        }
        //Get Users associated with a current Role Id
        [HttpGet("User/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (id < 0)
            {
                return BadRequest("Id must be greater than 0");
            }
            var role = await _uow.RoleRepository.Get(id);
            if (role == null)
            {
                return BadRequest("Role does not exist");
            }
            var user = await _uow.RoleRepository.GetUser(id);
            return Ok(new
            {
                Role = role.RoleName,
                Users = user.Select(u => new
                {
                    u.Id,
                    u.UserEmail,
                })
            });
        }
        //Create Roles
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateRole([FromBody] String Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return BadRequest("Enter correct RoleName");
            }
            var existingRole = await _uow.RoleRepository.Get(Name);
            if (existingRole != null)
            {
                return BadRequest("Role Already Exists");
            }
            var role = new Role()
            {
                RoleName = Name.ToLower()
            };
            var newRole = await _uow.RoleRepository.Create(role);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get),
                new { Id = newRole.Id },
                new { Id = newRole.Id, Name = newRole.RoleName });
        }
        //Delete Role
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRole([FromBody] String Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return BadRequest("Enter correct RoleName");
            }
            //admin and user cannot be deleted
            if (Name.ToLower() == "admin" || Name.ToLower() == "user")
            {
                return BadRequest($"Cannot Delete {Name}");
            }
            var role = await _uow.RoleRepository.Get(Name);
            if (role == null)
            {
                return BadRequest("Role does not exist");
            }

            _uow.RoleRepository.Delete(role);
            await _uow.SaveChangesAsync();

            return Ok("Removed" + new { Id = role.Id, Name = role.RoleName });
        }
    }
}
