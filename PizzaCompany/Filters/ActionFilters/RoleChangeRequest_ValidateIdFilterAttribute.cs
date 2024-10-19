using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaCompany.Data;

namespace PizzaCompany.Filters.ActionFilters
{
    public class RoleChangeRequest_ValidateIdFilterAttribute : ActionFilterAttribute
    {
        private readonly PizzaCompanyDbContext _projectContext;
        public RoleChangeRequest_ValidateIdFilterAttribute(PizzaCompanyDbContext projectContext)
        {
            _projectContext = projectContext;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var id = context.ActionArguments["id"] as int?;

            if(id == null || (int)id <= 0)
            {
                context.ModelState.AddModelError("Id","Id must be greater than 0");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
            else if(_projectContext.RoleChangeRequests.FirstOrDefault(rcr => rcr.Id == (int)id) == null)
            {
                context.ModelState.AddModelError("Id", "Request does not exist");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status404NotFound
                };
                context.Result = new NotFoundObjectResult(problemDetails);
            }
        }
    }
}
