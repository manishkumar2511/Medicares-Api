using FastEndpoints;
using FluentValidation;

namespace Medicares.Application.Features.Subscription_Plans.Create
{
    public class CreateSubscriptionPlanValidator : Validator<CreateSubscriptionPlanRequest>
    {
        public CreateSubscriptionPlanValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Plan name is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
            RuleFor(x => x.DurationInDays).GreaterThan(0).WithMessage("Duration must be greater than 0");
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid Subscription Plan Type");
        }
    }
}
