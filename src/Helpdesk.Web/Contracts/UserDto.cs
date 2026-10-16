using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public record UserDto(int Id, string Username, Role Role, int TicketCount);
