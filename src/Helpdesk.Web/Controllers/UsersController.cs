using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

/// <summary>
/// Zarządzanie użytkownikami (tylko administrator).
/// </summary>
[ApiController]
[Authorize(Policy = AuthPolicies.AdminOnly)]
[Route("api/users")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class UsersController(IUserService userService) : ControllerBase
{
    private AuthenticatedUser CurrentUser => User.ToAuthenticatedUser();

    /// <summary>
    /// Zwraca wszystkich użytkowników wraz z liczbą ich zgłoszeń.
    /// </summary>
    /// <response code="200">Lista użytkowników.</response>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
    {
        var result = await userService.GetAllAsync(CurrentUser);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Zwraca pojedynczego użytkownika.
    /// </summary>
    /// <response code="200">Użytkownik.</response>
    /// <response code="404">Użytkownik nie istnieje.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await userService.GetByIdAsync(CurrentUser, id);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Tworzy użytkownika.
    /// </summary>
    /// <response code="201">Utworzony użytkownik.</response>
    /// <response code="400">Niepoprawne dane (np. za krótkie hasło).</response>
    /// <response code="409">Nazwa użytkownika jest już zajęta.</response>
    [HttpPost]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request)
    {
        var result = await userService.CreateAsync(CurrentUser, request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Edytuje nazwę, rolę i (opcjonalnie) hasło użytkownika.
    /// </summary>
    /// <response code="200">Zaktualizowany użytkownik.</response>
    /// <response code="400">Niepoprawne dane.</response>
    /// <response code="404">Użytkownik nie istnieje.</response>
    /// <response code="409">Nazwa jest zajęta albo próba zdegradowania ostatniego administratora.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Update(int id, UpdateUserRequest request)
    {
        var result = await userService.UpdateAsync(CurrentUser, id, request);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Usuwa użytkownika razem z jego zgłoszeniami.
    /// </summary>
    /// <response code="204">Usunięto.</response>
    /// <response code="404">Użytkownik nie istnieje.</response>
    /// <response code="409">Próba usunięcia własnego konta lub ostatniego administratora.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await userService.DeleteAsync(CurrentUser, id);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result.Error);
    }
}
