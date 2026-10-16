using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public record AuthenticatedUser(int Id, string Username, Role Role);
