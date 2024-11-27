using System.Security.Claims;
using EShopApp.Application.Users.Commands.Register;
using EShopApp.Application.Users.Queries.Details;
using EShopApp.Application.Users.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class UserController : ApiController
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        var result = await _mediator.Send(request);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                Message = "User successfully registered!",
                Token = result.Value.Token
            });
        }

        return BadRequest(new
        {
            Message = "Registration failed",
            Errors = result.Errors
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginQuery request)
    {
        var result = await _mediator.Send(request);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                Token = result.Value.Token
            });
        }

        return BadRequest(new
        {
            Message = "Login failed",
            Errors = result.Errors
        });
    }

    [HttpGet]
    [Route("Details")]
    public async Task<IActionResult> Details()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            _logger.LogCritical("Authorize attribute failed.");
            return Unauthorized();
        }

        var result = await _mediator.Send(new UserDetailsQuery(new Guid(userId)));
        return Ok(result.Value);
    }
}
