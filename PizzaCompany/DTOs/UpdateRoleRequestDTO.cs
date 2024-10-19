using FluentValidation;

namespace PizzaCompany.DTOs
{
    public class UpdateRoleRequestDTO
    {
        public string Status { get; set; } = null!;
    }

    public class UpdateRoleRequestDTOValidator : AbstractValidator<UpdateRoleRequestDTO>
    {
        public UpdateRoleRequestDTOValidator()
        {
            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status cannot be null")
                .NotEmpty().WithMessage("Enter value");
        }
    }
}
