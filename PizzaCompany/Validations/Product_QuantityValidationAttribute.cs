using PizzaCompany.DTOs;
using System.ComponentModel.DataAnnotations;

namespace PizzaCompany.Validations
{
    public class Product_QuantityValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var od = validationContext.ObjectInstance as OrderDTO;

            if (od==null || od.Quantity <= 0)
            {
                return new ValidationResult("Quantity must be above 1");
            }
            return ValidationResult.Success;
        }
    }
}
