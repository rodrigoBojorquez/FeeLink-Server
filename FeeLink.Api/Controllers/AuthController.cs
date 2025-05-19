using System.Security.Claims;
using ErrorOr;
using FeeLink.Api.Common.Controllers;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.UseCases.Authentication.Commands.ChangePassword;
using FeeLink.Application.UseCases.Authentication.Commands.PasswordReset;
using FeeLink.Application.UseCases.Authentication.Commands.Register;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Application.UseCases.Authentication.Queries.Login;
using FeeLink.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

[AllowAnonymous]
public class AuthController(IMediator mediator, IAuthService authService, ITokenService tokenService)
    : ApiController
{
    public record LoginRequest(
        string Email,
        string Password
    );

    public record RegisterRequest(
        string Name,
        string LastName,
        string Email,
        string Password
    );

    public record PasswordResetRequest(string Token, string Password, string ConfirmPassword);

    public record GoogleLoginRequest(string Credential);

    public record ChangePasswordRequest(string OldPassword, string NewPassword, string ConfirmPassword);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Name, request.LastName, request.Email,
            request.Password);
        var result = await mediator.Send(command);

        return result.Match(authResult =>
        {
            authService.SetRefreshToken(authResult.RefreshToken);
            return Ok(authResult);
        }, Problem);
    }

    // [HttpPost("account-recovery")]
    // public async Task<IActionResult> AccountRecovery(string email)
    // {
    //     var command = new AccountRecoveryCommand(email);
    //     ErrorOr<Unit> result = await mediator.Send(command);
    //
    //     return result.Match(_ => Ok(), Problem);
    // }

    [HttpPost("password-reset")]
    public async Task<IActionResult> PasswordReset(PasswordResetRequest request)
    {
        var command = new PasswordResetCommand(request.Token, request.Password, request.ConfirmPassword);
        ErrorOr<AuthResult> authResult = await mediator.Send(command);

        return authResult.Match(result =>
        {
            authService.SetRefreshToken(result.RefreshToken);
            return Ok(result);
        }, Problem);
    }

    // [HttpPost("fast-register")]
    // public async Task<IActionResult> FastRegister(string email)
    // {
    //     var command = new FastRegisterCommand(email);
    //     ErrorOr<AuthResult> authResult = await mediator.Send(command);
    //
    //     return authResult.Match(result =>
    //     {
    //         authService.SetRefreshToken(result.RefreshToken);
    //         return Ok(result);
    //     }, Problem);
    // }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Problem(Errors.Authentication.MissingRefreshToken);

        var command = new ChangePasswordCommand(request.OldPassword, request.NewPassword, request.ConfirmPassword,
            refreshToken);

        ErrorOr<AuthResult> result = await mediator.Send(command);

        return result.Match(r =>
        {
            authService.SetRefreshToken(r.RefreshToken);
            return Ok(r);
        }, Problem);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        ErrorOr<AuthResult> authResult = await mediator.Send(query);

        return authResult.Match(
            response =>
            {
                authService.SetRefreshToken(response.RefreshToken);
                return Ok(response);
            },
            Problem);
    }

    [HttpPost("refresh-token/{refreshToken}")]
    public async Task<IActionResult> Token( string refreshToken)
    {
        if (refreshToken is null || string.IsNullOrEmpty(refreshToken))
            return Problem(Errors.Authentication.MissingRefreshToken);

        if (!await tokenService.ValidateRefreshTokenAsync(refreshToken))
            return Problem(Errors.Authentication.InvalidRefreshToken);

        var result = await tokenService.RefreshToken(refreshToken);

        return result.Match(authResult =>
        {
            authService.SetRefreshToken(authResult.RefreshToken);
            return Ok(authResult);
        }, Problem);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Problem(Errors.Authentication.MissingRefreshToken);

        await tokenService.DeleteRefreshTokenAsync(refreshToken);
        return Ok();
    }

    // [HttpPost("show-access")]
    // public async Task<IActionResult> ShowAccess()
    // {
    //     var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    //     if (userId is null) return Problem(Errors.Authentication.NotAuthorized);
    //
    //     var result = await authService.ShowAccessLevel(Guid.Parse(userId));
    //
    //     return result.Match(Ok, Problem);
    // }


    // [HttpGet]
    // [Route("token")]
    // public async Task<IActionResult> Token()
    // {
    //     if (!Request.Headers.ContainsKey("Authorization"))
    //     {
    //         return Problem(Errors.Authentication.NotAuthenticated);
    //     }
    //     
    //     var authHeader = Request.Headers.Authorization.ToString();
    //     var token = authHeader.Substring("Bearer ".Length).Trim();
    //     var query = new TokenQuery(token);
    //     ErrorOr<AuthResult> authResult = await _mediator.Send(query);
    //     
    //     return authResult.Match(
    //         result => Ok(_mapper.Map<AuthResult>(result)),
    //         errors => Problem(errors));
    // }
}