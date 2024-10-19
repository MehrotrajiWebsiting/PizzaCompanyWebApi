using Microsoft.AspNetCore.Mvc;
using PizzaCompany.DTOs;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Interface
{
    public interface IRoleChangeRequestRepository
    {
        Task<IEnumerable<object>> Get(String status);
        Task<RoleChangeRequest> GetById(int id);
        Task<RoleChangeRequest> GetMyPendingRequest(Role newRole, int userId);
        Task Create(RoleChangeRequest request);
        Task Update(RoleChangeRequest request,String newStatus);
        Task Delete(RoleChangeRequest request);
        string GetUserIdFromToken();
    }
}
