using FastEndpoints;
using FluentValidation;
using static Medicares.Domain.Shared.Constant.common.CommonConsts;

namespace Medicares.Application.Features.Auth.ResendTwoFACode
{
    public class Resend2FACodeValidator : Validator<Resend2FACodeRequest>
    {
        public Resend2FACodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(RequiredMessage)
                .EmailAddress().WithMessage("Invalid Email Address");
        }
    }
}
