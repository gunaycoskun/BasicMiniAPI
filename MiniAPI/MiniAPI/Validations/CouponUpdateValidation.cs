using FluentValidation;
using MiniAPI.Models.DTO;

namespace MiniAPI.Validations
{
    public class CouponUpdateValidation:AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("Id is required");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(c => c.Percent).NotEmpty().WithMessage("Percent is required");
            RuleFor(c=> c.Percent).InclusiveBetween(1, 100).WithMessage("Percent must be between 1 and 100");
            RuleFor(c => c.IsActive).NotEmpty().WithMessage("IsActive is required");
        }
    }
}
