using FastEndpoints;
using FluentValidation;

namespace Medicares.Application.Features.Auth.GetStarted;

public class GetStartedValidator : Validator<GetStartedRequest>
{
    public GetStartedValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required");
        RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Company Name is required");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
