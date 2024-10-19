using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.DTOs;
using PizzaCompany.Filters.ActionFilters;
using PizzaCompany.Interface;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleChangeRequestsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<RoleChangeRequestDTO> _roleChangeRequestDTOvalidator;
        private readonly IValidator<UpdateRoleRequestDTO> _updateRoleRequestDTOvalidator;
        public RoleChangeRequestsController(IUnitOfWork uow,
                            IValidator<RoleChangeRequestDTO> roleChangeRequestDTOvalidator,
                            IValidator<UpdateRoleRequestDTO> updateRoleRequestDTOvalidator) 
        {
            _uow = uow;
            _roleChangeRequestDTOvalidator = roleChangeRequestDTOvalidator;
            _updateRoleRequestDTOvalidator = updateRoleRequestDTOvalidator;
        }
        //Get request on basis of status
        [HttpGet("Status/{status}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Get([FromRoute] String status)
        {
            status = status.ToUpper();
            if (status != "PENDING" && status != "ACCEPTED" && status != "REJECTED")
            {
                return BadRequest("WRONG STATUS NAME");
            }
            var requests = await _uow.RoleChangeRequestRepository.Get(status);
            if (!requests.Any())
            {
                return Ok($"NO {status} REQUEST");
            }

            return Ok(requests);
        }
        //Get a particular request by id
        [HttpGet("Id/{id}")]
        [Authorize(Roles = "admin")]
        [ServiceFilter(typeof(RoleChangeRequest_ValidateIdFilterAttribute))]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _uow.RoleChangeRequestRepository.GetById(id);
            return Ok(new
            {
                Id = request.Id,
                UserId = request.UserId,
                UserEmail = request.User.UserEmail,
                RequestedRole = request.RequestedRole,
            });
        }
        //Send Request
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRoleChangeRequest([FromBody] RoleChangeRequestDTO requestDto)
        {
            ValidationResult result = await _roleChangeRequestDTOvalidator.ValidateAsync(requestDto);
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result);
            }
            //admin cannot be requested
            if (requestDto?.NewRole?.ToLower() == "admin")
            {
                return Unauthorized("NOT ALLOWED");
            }
            //VALIDATE IF ROLE EXISTS
            var newRole = await _uow.RoleRepository.Get(requestDto.NewRole);
            if (newRole == null)
            {
                return NotFound("ROLE DOES NOT EXIST");
            }

            var userId = int.Parse(_uow.RoleChangeRequestRepository.GetUserIdFromToken());
            //VALIDATE IF USER ALREADY HAS THE ROLE
            if (newRole.UserRoles.Any(ur => ur.UserId == userId))
            {
                return BadRequest("User already has this role");
            }
            //VALIDATE ALREADY PENDING REQUEST FOR SAME ROLE BY SAME USER
            var existingRequest = await _uow.RoleChangeRequestRepository.GetMyPendingRequest(newRole,userId);
            if (existingRequest != null)
            {
                return BadRequest("Request already exists");
            }
            //Create request
            var request = new RoleChangeRequest()
            {
                UserId = userId,
                RoleId = newRole.Id,
                RequestedRole = requestDto.NewRole.ToLower()
            };

            await _uow.RoleChangeRequestRepository.Create(request);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), //GetRequestById must exist and take Id as route value
                    new { Id = request.Id },
                    new
                    {
                        Id = request.Id,
                        UserId = request.UserId,
                        RequestedRole = request.RequestedRole,
                        Status = request.Status,
                    });
        }
        //Patch - updating only a part of resource
        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        [ServiceFilter(typeof(RoleChangeRequest_ValidateIdFilterAttribute))]
        public async Task<IActionResult> UpdateRequest(int id, UpdateRoleRequestDTO update)
        {
            ValidationResult result = await _updateRoleRequestDTOvalidator.ValidateAsync(update);
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result);
            }
            var newStatus = update.Status.ToUpper();
            if (newStatus != "ACCEPTED" && newStatus != "REJECTED")
            {
                return BadRequest("WRONG STATUS");
            }

            var request = await _uow.RoleChangeRequestRepository.GetById(id);
            //VALIDATE IF REQUEST IS PENDING OR NOT
            if (request.Status != "PENDING")
            {
                return BadRequest($"The request is already {request.Status}");
            }
            await _uow.RoleChangeRequestRepository.Update(request, newStatus);
            await _uow.SaveChangesAsync();
                
            return NoContent();
        }
        //CANCEL/DELETE A REQUEST
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteRequest(RoleChangeRequestDTO requestDto)
        {
            ValidationResult result = await _roleChangeRequestDTOvalidator.ValidateAsync(requestDto);
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(result);
            }

            //VALIDATE IF ROLE EXISTS
            var newRole = await _uow.RoleRepository.Get(requestDto.NewRole);
            if (newRole == null)
            {
                return NotFound("ROLE DOES NOT EXIST");
            }

            var userId = int.Parse(_uow.RoleChangeRequestRepository.GetUserIdFromToken());
            //VALIDATE IF PENDING REQUEST EXISTS
            var request = await _uow.RoleChangeRequestRepository.GetMyPendingRequest(newRole, userId);
            if (request == null)
            {
                return BadRequest("Request for this role does not exist or has been accepted/rejected");
            }

            await _uow.RoleChangeRequestRepository.Delete(request);
            await _uow.SaveChangesAsync();

            return Ok($"REQUEST for role - {requestDto.NewRole.ToLower()} was deleted");
        }
    }
}
