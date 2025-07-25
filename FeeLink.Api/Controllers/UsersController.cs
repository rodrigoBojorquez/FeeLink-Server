using FeeLink.Infrastructure.Services.Assets;
using FeeLink.Api.Common.Controllers;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Users.Commands.Create;
using FeeLink.Application.UseCases.Users.Commands.Delete;
using FeeLink.Application.UseCases.Users.Commands.Update;
using FeeLink.Application.UseCases.Users.Commands.UpdateProfile;
using FeeLink.Application.UseCases.Users.Common;
using FeeLink.Application.UseCases.Users.Queries.GetData;
using FeeLink.Application.UseCases.Users.Queries.ListLinkedWithToy;
using FeeLink.Application.UseCases.Users.Queries.ListUsers;
using FeeLink.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class UsersController(
    IMediator mediator,
    IAuthService userService,
    IUserRepository repository,
    ImageService imageService)
    : ApiController
{
    public record ListUsersRequest(
        int? Page = 1,
        int? PageSize = 10,
        string? Search = null,
        Guid? RoleId = null);

    public record UserCreateRequest(
        string Name,
        string FirstLastName,
        string SecondLastName,
        string Email,
        string Password,
        Guid? EntityId,
        Guid RoleId);

    public record UserUpdateProfileRequest(
        string Name,
        string FirstLastName,
        string? SecondLastName);

    public record UserUpdateRequest(
        string Name,
        string FirstLastName,
        string SecondLastName,
        string Email,
        string Password,
        Guid? EntityId,
        Guid RoleId);

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListUsersRequest request)
    {
        var query = new ListUsersQuery(request.Page ?? 1, request.PageSize ?? 10, request.Search,
            request.RoleId);
        var result = await mediator.Send(query);

        return result.Match(Ok, Problem);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        if (!userService.HasSuperAccess())
        {
            if (userService.GetUserId() != id)
                return Problem(Errors.Authentication.NotAuthorized);
        }

        var user = await repository.GetByIdAsync(id);

        return user is null ? Problem(Errors.User.NotFound) : Ok(user.ToResult());
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserCreateRequest request)
    {
        var command = new CreateUserCommand(request.Name, request.FirstLastName, request.SecondLastName,
            request.Email, request.Password, request.EntityId, request.RoleId);
        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, UserUpdateRequest request)
    {
        var command = new UpdateUserCommand(id, request.Name, request.FirstLastName, request.SecondLastName,
            request.Email, request.Password, request.EntityId, request.RoleId);
        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPut("{id:guid}/profile")]
    public async Task<IActionResult> UpdateProfileData(Guid id, UserUpdateProfileRequest request)
    {
        var userId = userService.GetUserId();

        if (userId != id)
            return Problem(Errors.Authentication.NotAuthorized);

        var command = new UpdateProfileCommand(id, request.Name, request.FirstLastName, request.SecondLastName);

        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPatch("profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image, [FromForm] Guid userId)
    {
        var user = await repository.GetByIdAsync(userId);

        if (user is null)
            return Problem(Errors.User.NotFound);

        if (user.Id != userService.GetUserId())
            return Problem(Errors.Authentication.NotAuthorized);

        if (!string.IsNullOrEmpty(user.Picture))
        {
            imageService.Delete(user.Picture);
        }

        var result = await imageService.UploadAsync(
            image.FileName,
            image.OpenReadStream(),
            userId.ToString());

        if (result.IsError)
            return Problem(result.Errors);

        var filePath = result.Value;

        user.Picture = filePath;

        await repository.UpdateAsync(user);

        return Ok(filePath);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpGet("linked/{toyId:guid}")]
    public async Task<IActionResult> GetLinkedUsers(Guid toyId)
    {
        var query = new ListLinkedUsersWithToyQuery(toyId);
        var result = await mediator.Send(query);

        return result.Match(Ok, Problem);
    }

    [HttpGet("{tutorId:guid}/data")]
    public async Task<IActionResult> GetUserData(Guid tutorId)
    {
        var query = new GetUserDataQuery(tutorId);
        var result = await mediator.Send(query);
        return result.Match(Ok, Problem);
    }
}