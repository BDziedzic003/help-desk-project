using Helpdesk.Web.Contracts;

namespace Helpdesk.Web.Services;

public interface ITicketService
{
    Task<IReadOnlyList<TicketDto>> GetAllAsync(AuthenticatedUser actor, TicketQuery query);

    Task<ServiceResult<TicketDto>> GetByIdAsync(AuthenticatedUser actor, int ticketId);

    Task<TicketDto> CreateAsync(AuthenticatedUser actor, TicketRequest request);

    Task<ServiceResult<TicketDto>> UpdateAsync(AuthenticatedUser actor, int ticketId, TicketRequest request);

    Task<ServiceResult> DeleteAsync(AuthenticatedUser actor, int ticketId);

    Task<ServiceResult<TicketDto>> ChangeStatusAsync(AuthenticatedUser actor, int ticketId, ChangeStatusRequest request);
}
