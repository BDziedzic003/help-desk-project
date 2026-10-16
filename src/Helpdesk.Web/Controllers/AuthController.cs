using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticatedUser>> Login(LoginRequest request)
    {
        var result = await authService.AuthenticateAsync(request.Username, request.Password);
        if (!result.Succeeded)
        {
            return Unauthorized(new { error = result.Error });
        }

        await HttpContext.SignInUserAsync(result.User!);

        return Ok(result.User);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutUserAsync();
        return NoContent();
    }
}
