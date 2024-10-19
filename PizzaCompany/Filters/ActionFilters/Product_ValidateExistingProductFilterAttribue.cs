using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Data;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Filters.ActionFilters
{
    public class Product_ValidateExistingProductFilterAttribue : Attribute, IAsyncActionFilter
    {
        private readonly PizzaCompanyDbContext _context;
        public Product_ValidateExistingProductFilterAttribue(PizzaCompanyDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var product = context.ActionArguments["product"] as Product;

            if (product == null)
            {
                context.ModelState.AddModelError("Product", "No Input");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;  // Early return to prevent further execution
            }
            else
            {
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p=>p.Name.ToUpper() == product.Name.ToUpper());

                if (existingProduct != null)
                {
                    context.ModelState.AddModelError("Product", "Product Already Exists");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;  // Early return to prevent further execution
                }
            }
            // Continue with the action execution
            await next();
        }
    }
}
