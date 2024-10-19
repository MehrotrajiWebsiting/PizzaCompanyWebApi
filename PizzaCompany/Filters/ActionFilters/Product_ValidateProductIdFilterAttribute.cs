using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaCompany.Data;

namespace PizzaCompany.Filters.ActionFilters
{
    public class Product_ValidateProductIdFilterAttribute : ActionFilterAttribute
    {
        PizzaCompanyDbContext _projectContext;
        public Product_ValidateProductIdFilterAttribute(PizzaCompanyDbContext projectContext)
        {
            _projectContext = projectContext;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            int? Id = context.ActionArguments["Id"] as int?;

            if (!Id.HasValue)
            {
                context.ModelState.AddModelError("Id", "Product Id is null");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
            else
            {
                var existingProduct = _projectContext.Products.FirstOrDefault(p => p.Id == Id.Value);
                if (existingProduct == null)
                {
                    context.ModelState.AddModelError("Product", "Product does not exist");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                }
            }
        }
    }
}
