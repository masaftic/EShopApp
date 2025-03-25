using System.Security.Claims;
using EShopApp.Application.Users.Commands.Register;
using EShopApp.Application.Users.Queries.Details;
using EShopApp.Application.Users.Queries.Login;
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
        return result.Match(
            authenticationResult => Ok(new
            {
                Message = "User registered successfully.",
                Token = authenticationResult.Token,
                ExpiresIn = authenticationResult.ExpiresIn,
            }),
            errors => HandleErrors(errors)
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginQuery request)
    {
        var result = await _mediator.Send(request);
        
        return result.Match(
            authenticationResult => Ok(new
            {
                Token = authenticationResult.Token,
                ExpiresIn = authenticationResult.ExpiresIn,
            }),
            errors => HandleErrors(errors)
        );
    }

    [HttpGet("details")]
    public async Task<IActionResult> Details()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            _logger.LogCritical("Authorize attribute failed.");
            return Unauthorized();
        }

        var result = await _mediator.Send(new UserDetailsQuery(int.Parse(userId)));
        if (result.IsError && result.FirstError == DomainErrors.User.InvalidCredentials)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: result.FirstError.Description);
        }

        return result.Match(
            authenticationResult => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }
}