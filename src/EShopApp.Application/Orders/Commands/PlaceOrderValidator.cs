using System;
using FluentValidation;

namespace EShopApp.Application.Orders.Commands;

public class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("ShippingAddress is required");

        RuleFor(x => x.ShippingPostalCode)
            .NotEmpty()
            .WithMessage("ShippingPostalCode is required");
    }
}
