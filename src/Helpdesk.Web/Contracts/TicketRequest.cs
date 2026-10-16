using System.ComponentModel.DataAnnotations;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

/// <summary>
/// Editable ticket fields, shared by the create and update operations.
/// </summary>
public class TicketRequest
{
    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    [StringLength(HelpdeskDbContext.TicketTitleMaxLength, ErrorMessage = "Tytuł może mieć maksymalnie {1} znaków.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany.")]
    [StringLength(HelpdeskDbContext.TicketDescriptionMaxLength, ErrorMessage = "Opis może mieć maksymalnie {1} znaków.")]
    public string Description { get; set; } = string.Empty;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
