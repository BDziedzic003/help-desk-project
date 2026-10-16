using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicies.AdminOnly)]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private AuthenticatedUser CurrentUser => User.ToAuthenticatedUser();

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
    {
        var result = await userService.GetAllAsync(CurrentUser);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await userService.GetByIdAsync(CurrentUser, id);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request)
    {
        var result = await userService.CreateAsync(CurrentUser, request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : this.ToActionResult(result.Error);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, UpdateUserRequest request)
    {
        var result = await userService.UpdateAsync(CurrentUser, id, request);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await userService.DeleteAsync(CurrentUser, id);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result.Error);
    }
}
