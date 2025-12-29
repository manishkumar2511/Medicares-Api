using FastEndpoints;
using FluentValidation;
using static Medicares.Domain.Shared.Constant.common.CommonConsts;

namespace Medicares.Application.Features.Auth.Verify2FACode
{
    internal class Verify2FACodeValidator : Validator<Verify2FACodeRequest>
    {
        public Verify2FACodeValidator()
        {
            RuleFor(b => b.Email)
         .NotEmpty().WithMessage(RequiredMessage)
         .MaximumLength(100).WithMessage(MaxLengthMessage); // Added arbitrary 100 for now or need EmailLength const


            RuleFor(b => b.Code)
               .NotEmpty().WithMessage(RequiredMessage)
              .MaximumLength(6).WithMessage(MaxLengthMessage);
        }
    }
}
