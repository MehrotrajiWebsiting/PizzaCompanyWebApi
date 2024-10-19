using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaCompany.Data;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Filters.ActionFilters
{
    public class User_ValidateExistingUserFilterAttribute : ActionFilterAttribute
    {
        private readonly PizzaCompanyDbContext _context;
        public User_ValidateExistingUserFilterAttribute(PizzaCompanyDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var user = context.ActionArguments["user"] as User;


            var exisitingUser = _context.Users.Where(u => u.UserEmail == user.UserEmail).FirstOrDefault();

            if (exisitingUser != null)
            {
                context.ModelState.AddModelError("User", "User already exists");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }
    }
}
