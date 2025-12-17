using Eventure.Order.API.Features.GetAllOrdersForUser.Models;
using FluentValidation;

namespace Eventure.Order.API.Features.GetAllOrdersForUser;

public sealed class GetAllOrdersForUserRequestValidator : AbstractValidator<GetAllOrdersForUserRequest>
{
    public GetAllOrdersForUserRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}