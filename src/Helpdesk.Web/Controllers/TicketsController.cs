using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

/// <summary>
/// Zgłoszenia. Zwykły użytkownik widzi i zmienia tylko własne zgłoszenia, administrator wszystkie.
/// </summary>
[ApiController]
[Authorize]
[Route("api/tickets")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    private AuthenticatedUser CurrentUser => User.ToAuthenticatedUser();

    /// <summary>
    /// Zwraca listę zgłoszeń z opcjonalnym filtrowaniem i sortowaniem.
    /// </summary>
    /// <param name="query">Kryteria filtrowania (status, priorytet, fragment tytułu) i sortowania.</param>
    /// <response code="200">Lista zgłoszeń widocznych dla bieżącego użytkownika.</response>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TicketDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetAll([FromQuery] TicketQuery query) =>
        Ok(await ticketService.GetAllAsync(CurrentUser, query));

    /// <summary>
    /// Zwraca pojedyncze zgłoszenie.
    /// </summary>
    /// <response code="200">Zgłoszenie.</response>
    /// <response code="404">Zgłoszenie nie istnieje lub należy do innego użytkownika.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<TicketDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetById(int id)
    {
        var result = await ticketService.GetByIdAsync(CurrentUser, id);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Tworzy zgłoszenie (status Open) przypisane do bieżącego użytkownika.
    /// </summary>
    /// <response code="201">Utworzone zgłoszenie.</response>
    /// <response code="400">Niepoprawne dane (np. pusty tytuł).</response>
    [HttpPost]
    [ProducesResponseType<TicketDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketDto>> Create(TicketRequest request)
    {
        var ticket = await ticketService.CreateAsync(CurrentUser, request);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    /// <summary>
    /// Edytuje tytuł, opis i priorytet zgłoszenia.
    /// </summary>
    /// <response code="200">Zaktualizowane zgłoszenie.</response>
    /// <response code="400">Niepoprawne dane.</response>
    /// <response code="404">Zgłoszenie nie istnieje lub należy do innego użytkownika.</response>
    /// <response code="409">Zgłoszenie jest zakończone, a edytuje je zwykły użytkownik.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType<TicketDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketDto>> Update(int id, TicketRequest request)
    {
        var result = await ticketService.UpdateAsync(CurrentUser, id, request);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Usuwa zgłoszenie.
    /// </summary>
    /// <response code="204">Usunięto.</response>
    /// <response code="404">Zgłoszenie nie istnieje lub należy do innego użytkownika.</response>
    /// <response code="409">Zgłoszenie jest zakończone, a usuwa je zwykły użytkownik.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await ticketService.DeleteAsync(CurrentUser, id);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result.Error);
    }

    /// <summary>
    /// Zmienia status zgłoszenia (tylko administrator).
    /// </summary>
    /// <response code="200">Zgłoszenie z nowym statusem.</response>
    /// <response code="403">Użytkownik nie jest administratorem.</response>
    /// <response code="404">Zgłoszenie nie istnieje.</response>
    [HttpPut("{id:int}/status")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType<TicketDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> ChangeStatus(int id, ChangeStatusRequest request)
    {
        var result = await ticketService.ChangeStatusAsync(CurrentUser, id, request);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }
}
