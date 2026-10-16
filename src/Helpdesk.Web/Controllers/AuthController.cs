using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

/// <summary>
/// Logowanie i wylogowanie (uwierzytelnianie ciasteczkiem).
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Loguje użytkownika i ustawia ciasteczko sesji.
    /// </summary>
    /// <response code="200">Zalogowano; zwraca dane użytkownika.</response>
    /// <response code="400">Brak nazwy użytkownika lub hasła.</response>
    /// <response code="401">Nieprawidłowa nazwa użytkownika lub hasło.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthenticatedUser>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Wylogowuje bieżącego użytkownika.
    /// </summary>
    /// <response code="204">Wylogowano.</response>
    /// <response code="401">Użytkownik nie jest zalogowany.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutUserAsync();
        return NoContent();
    }
}
