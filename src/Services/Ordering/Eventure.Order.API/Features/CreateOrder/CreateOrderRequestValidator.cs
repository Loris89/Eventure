using Eventure.Order.API.Features.CreateOrder.Models;
using FluentValidation;

namespace Eventure.Order.API.Features.CreateOrder;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemValidator());
    }
}

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.EventName).NotEmpty();
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
