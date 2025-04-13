using EShopApp.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[AllowAnonymous]
[Route("api/webhooks")]
public class StripeWebhookController : ApiController
{
    private readonly IMediator _mediator;

    public StripeWebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrEmpty(signature))
        {
            return BadRequest("Stripe signature is missing");
        }

        await _mediator.Publish(new StripeWebhookReceivedEvent(json, signature));
        return NoContent();
    }
}