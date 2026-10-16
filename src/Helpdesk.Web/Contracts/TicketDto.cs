using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public record TicketDto(
    int Id,
    string Title,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int UserId,
    string AuthorName);
