using FluentValidation; 

namespace PizzaCompany.DTOs
{
    public class RoleChangeRequestDTO
    {
        public String? NewRole { get; set; }
    }
    //Fluent Validation
    public class RoleChangeRequestDTOValidator : AbstractValidator<RoleChangeRequestDTO>
    {
        public RoleChangeRequestDTOValidator() 
        {
            RuleFor(req => req.NewRole)
                .NotNull().WithMessage("Enter Correct Role Name")
                .NotEmpty().WithMessage("Empty Role Name");
        }
    }
}
