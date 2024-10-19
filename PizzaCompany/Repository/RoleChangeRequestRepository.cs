using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Data;
using PizzaCompany.DTOs;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;
using System.Security.Claims;

namespace PizzaCompany.Repository
{
    public class RoleChangeRequestRepository : IRoleChangeRequestRepository
    {
        private readonly PizzaCompanyDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public RoleChangeRequestRepository(PizzaCompanyDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor; 
        }
        // Get all request with given status
        public async Task<IEnumerable<object>> Get(string status)
        {
            return await _context.RoleChangeRequests
                                .Include(rcr => rcr.User)
                                .Where(rcr => rcr.Status == status)
                                .Select(rcr => new
                                {
                                    Id = rcr.Id,
                                    UserId = rcr.UserId,
                                    UserEmail = rcr.User.UserEmail,
                                    RequestedRole = rcr.RequestedRole,
                                })
                                .ToListAsync();
        }
        // Get request by id
        public async Task<RoleChangeRequest> GetById(int id)
        {
            return await _context.RoleChangeRequests
                                    .Include(rcr => rcr.User)
                                    .Where(rcr => rcr.Id == id).FirstOrDefaultAsync();
        }
        //Get LoggedIn UserId from token
        public string GetUserIdFromToken()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity.Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        // Get pending Request from the user for given role
        public async Task<RoleChangeRequest> GetMyPendingRequest(Role newRole,int userId)
        {
            return await _context.RoleChangeRequests
                        .FirstOrDefaultAsync(rcr => rcr.RoleId == newRole.Id &&
                                                    rcr.UserId == userId &&
                                                    rcr.Status == "PENDING");
        }
        // Create Request
        public async Task Create(RoleChangeRequest request)
        {
            await _context.RoleChangeRequests.AddAsync(request);
        }
        // Update Request
        public async Task Update(RoleChangeRequest request,String newStatus)
        {
            if (newStatus == "ACCEPTED")
            {
                var userRole = new UserRole()
                {
                    RoleId = request.RoleId,
                    UserId = request.UserId
                };
                await _context.AddAsync(userRole);
            }
            request.Status = newStatus;
        }
        // Delete Request
        public async Task Delete(RoleChangeRequest request)
        {
            _context.RoleChangeRequests.Remove(request);
        }
    }
}
