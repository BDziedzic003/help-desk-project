using Helpdesk.Web.Contracts;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services.Tickets;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Web.Services;

public class TicketService(
    HelpdeskDbContext dbContext,
    IEnumerable<ITicketFilter> filters,
    IEnumerable<ITicketSortStrategy> sortStrategies,
    TimeProvider timeProvider) : ITicketService
{
    public async Task<IReadOnlyList<TicketDto>> GetAllAsync(AuthenticatedUser actor, TicketQuery query)
    {
        var tickets = VisibleTo(actor);
        tickets = filters.Aggregate(tickets, (current, filter) => filter.Apply(current, query));

        var sorted = await Sort(tickets, query).ToListAsync();
        return sorted.Select(ToDto).ToList();
    }

    public async Task<ServiceResult<TicketDto>> GetByIdAsync(AuthenticatedUser actor, int ticketId)
    {
        var ticket = await VisibleTo(actor).FirstOrDefaultAsync(t => t.Id == ticketId);

        return ticket is null
            ? ServiceResult<TicketDto>.Failure(ServiceError.NotFound)
            : ServiceResult<TicketDto>.Success(ToDto(ticket));
    }

    public async Task<TicketDto> CreateAsync(AuthenticatedUser actor, TicketRequest request)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedAt = now,
            UpdatedAt = now,
            UserId = actor.Id
        };

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync();

        return await LoadDtoAsync(ticket.Id);
    }

    public async Task<ServiceResult<TicketDto>> UpdateAsync(AuthenticatedUser actor, int ticketId, TicketRequest request)
    {
        var (ticket, error) = await FindModifiableAsync(actor, ticketId);
        if (ticket is null)
        {
            return ServiceResult<TicketDto>.Failure(error);
        }

        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description.Trim();
        ticket.Priority = request.Priority;
        ticket.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync();

        return ServiceResult<TicketDto>.Success(await LoadDtoAsync(ticket.Id));
    }

    public async Task<ServiceResult> DeleteAsync(AuthenticatedUser actor, int ticketId)
    {
        var (ticket, error) = await FindModifiableAsync(actor, ticketId);
        if (ticket is null)
        {
            return ServiceResult.Failure(error);
        }

        dbContext.Tickets.Remove(ticket);
        await dbContext.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult<TicketDto>> ChangeStatusAsync(
        AuthenticatedUser actor, int ticketId, ChangeStatusRequest request)
    {
        if (actor.Role != Role.Admin)
        {
            return ServiceResult<TicketDto>.Failure(ServiceError.Forbidden);
        }

        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null)
        {
            return ServiceResult<TicketDto>.Failure(ServiceError.NotFound);
        }

        ticket.Status = request.Status;
        ticket.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync();

        return ServiceResult<TicketDto>.Success(await LoadDtoAsync(ticket.Id));
    }

    /// <summary>
    /// Admins see every ticket, regular users only their own.
    /// </summary>
    private IQueryable<Ticket> VisibleTo(AuthenticatedUser actor)
    {
        var tickets = dbContext.Tickets.AsNoTracking().Include(ticket => ticket.User).AsQueryable();
        return actor.Role == Role.Admin ? tickets : tickets.Where(ticket => ticket.UserId == actor.Id);
    }

    /// <summary>
    /// Loads a tracked ticket the actor may edit or delete. Tickets of other users look like missing ones;
    /// a completed ticket is locked for regular users.
    /// </summary>
    private async Task<(Ticket? Ticket, ServiceError Error)> FindModifiableAsync(AuthenticatedUser actor, int ticketId)
    {
        var ticket = await dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null || (actor.Role != Role.Admin && ticket.UserId != actor.Id))
        {
            return (null, ServiceError.NotFound);
        }

        if (actor.Role != Role.Admin && ticket.Status == TicketStatus.Completed)
        {
            return (null, ServiceError.Locked);
        }

        return (ticket, ServiceError.None);
    }

    private IOrderedQueryable<Ticket> Sort(IQueryable<Ticket> tickets, TicketQuery query)
    {
        var strategy = sortStrategies.First(s => s.Field == query.SortBy);
        return strategy.Apply(tickets, query.Descending);
    }

    private async Task<TicketDto> LoadDtoAsync(int ticketId)
    {
        var ticket = await dbContext.Tickets.AsNoTracking().Include(t => t.User).FirstAsync(t => t.Id == ticketId);
        return ToDto(ticket);
    }

    private static TicketDto ToDto(Ticket ticket) => new(
        ticket.Id,
        ticket.Title,
        ticket.Description,
        ticket.Priority,
        ticket.Status,
        ticket.CreatedAt,
        ticket.UpdatedAt,
        ticket.UserId,
        ticket.User?.Username ?? string.Empty);
}
