using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaCompany.Data;
using PizzaCompany.DTOs;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PizzaCompany.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PizzaCompanyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPasswordHasher _passwordHasher;
        public UserRepository(PizzaCompanyDbContext context, IConfiguration configuration, 
                IHttpContextAccessor contextAccessor, IPasswordHasher passwordHasher)
        {
            this._context = context;
            this._configuration = configuration;
            this._contextAccessor = contextAccessor;
            _passwordHasher = passwordHasher;
        }
        public async Task<User?> Get(int id)
        {
            return await _context.Users.Include(u => u.Orders)
                                .ThenInclude(o => o.Product)
                             .Include(u => u.UserRoles)
                                .ThenInclude(ur => ur.Role)
                             .Where(u => u.Id == id)
                             .FirstOrDefaultAsync();
        }
        public async Task Register(User user)
        {
            //Add to role = user Automatically to every new user
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.ToLower() == "user");

            // Hash Password
            user.Password = _passwordHasher.Hash(user.Password);

            var userRole = new UserRole()
            {
                User = user,
                Role = role
            };

            //Add BOTH user and userRole explicitly
            await _context.AddAsync(user);
            await _context.AddAsync(userRole);
        }

        public async Task<User?> GetByEmail(String email)
        {
            return await _context.Users.Include(u => u.Orders)
                                .ThenInclude(o => o.Product)
                             .Include(u => u.UserRoles)
                                .ThenInclude(ur => ur.Role)
                             .Where(u => u.UserEmail == email)
                             .FirstOrDefaultAsync();
        }
        public String GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            //Add roles
            foreach (var role in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSection["ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public bool Login(User user,LoginDTO loginDTO)
        {
            return _passwordHasher.Verify(user.Password,loginDTO.Password);
        }

        public async Task<IEnumerable<String>> GetRoles()
        {
            int userId = int.Parse(GetUserId());
            return await _context.UsersRoles
                                .Include(ur => ur.Role)
                                .Where(ur => ur.UserId == userId)
                                .Select(ur => ur.Role.RoleName).ToListAsync();
        }
        private string GetUserId()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity.Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
