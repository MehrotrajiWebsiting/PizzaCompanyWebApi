using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaCompany.DTOs;
using PizzaCompany.Filters.ActionFilters;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public UsersController(IUnitOfWork uow) {
            _uow = uow;
        }

        // Get User By Id (only by admin)
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // Get User
            var user = await _uow.UserRepository.Get(id);
            if(user == null)
            {
                return NotFound("User does not exist");
            }
            var userDto = new
            {
                Email = user.UserEmail,
                Roles = user.UserRoles.Select(x => x.Role.RoleName).ToList(),
                Orders = user.Orders.Select(o => new
                {
                    Id = o.Id,
                    Product = o.Product.Name,
                    Quantity = o.Quantity,
                    TotalPrice = o.Product.Price * o.Quantity,
                })
            };
            return Ok(userDto);
        }

        // Register User
        [HttpPost("Register")]
        [User_ValidateUserInputFilter]
        [ServiceFilter(typeof(User_ValidateExistingUserFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            await _uow.UserRepository.Register(user);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser),
                new { id = user.Id },
                user);
        }

        // Login User
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginDTO loginDTO)
        {
            //Get user
            var user = await _uow.UserRepository.GetByEmail(loginDTO.Email);
            if(user == null)
            {
                return Unauthorized("Wrong Username");
            }
            if (!_uow.UserRepository.Login(user,loginDTO))
            {
                return Unauthorized("Wrong Password");
            }

            var tokenString = _uow.UserRepository.GenerateToken(user);
            return Ok(new { Token = tokenString });
        }

        // Get Your Roles
        [HttpGet("Roles")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _uow.UserRepository.GetRoles());
        }
    }
}
