using System.Security.Claims;
using EShopApp.Application.Users.Commands;
using EShopApp.Application.Users.Commands.Login;
using EShopApp.Application.Users.Commands.Register;
using EShopApp.Application.Users.Queries;
using EShopApp.Application.Users.Queries.Details;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiController
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World!");
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        var result = await _mediator.Send(request);
        return result.Match(Ok, HandleErrors);
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await _mediator.Send(request);

        if (result.IsError && result.FirstError == DomainErrors.User.InvalidCredentials)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: result.FirstError.Description);
        }

        return result.Match(Ok, HandleErrors);
    }


    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> LoginWithRefresh([FromBody] LoginWithRefreshTokenQuery request)
    {
        var result = await _mediator.Send(request);

        return result.Match(Ok, HandleErrors);
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            _logger.LogCritical("Authorize attribute failed.");
            return Unauthorized();
        }

        var result = await _mediator.Send(new UserDetailsQuery(int.Parse(userId)));
        return result.Match(Ok, HandleErrors);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _mediator.Send(new UserDetailsQuery(id));
        return result.Match(Ok, HandleErrors);
    }


    [Authorize]
    [HttpDelete("revoke-token")]
    public async Task<IActionResult> RevokeToken()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            _logger.LogCritical("Authorize attribute failed.");
            return Unauthorized();
        }

        var request = new RevokeTokenCommand(int.Parse(userId));
        var result = await _mediator.Send(request);
        return result.Match(success => NoContent(), HandleErrors);
    }
}