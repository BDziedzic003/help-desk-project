using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/tickets")]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    private AuthenticatedUser CurrentUser => User.ToAuthenticatedUser();

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetAll([FromQuery] TicketQuery query) =>
        Ok(await ticketService.GetAllAsync(CurrentUser, query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDto>> GetById(int id)
    {
        var result = await ticketService.GetByIdAsync(CurrentUser, id);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create(TicketRequest request)
    {
        var ticket = await ticketService.CreateAsync(CurrentUser, request);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketDto>> Update(int id, TicketRequest request)
    {
        var result = await ticketService.UpdateAsync(CurrentUser, id, request);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await ticketService.DeleteAsync(CurrentUser, id);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result.Error);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<TicketDto>> ChangeStatus(int id, ChangeStatusRequest request)
    {
        var result = await ticketService.ChangeStatusAsync(CurrentUser, id, request);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result.Error);
    }
}
