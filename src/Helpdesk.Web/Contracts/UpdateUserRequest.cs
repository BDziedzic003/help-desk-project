using System.ComponentModel.DataAnnotations;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    [StringLength(HelpdeskDbContext.UsernameMaxLength, ErrorMessage = "Nazwa użytkownika może mieć maksymalnie {1} znaków.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Optional: when empty, the current password stays unchanged.
    /// </summary>
    [StringLength(UserValidation.PasswordMaxLength, MinimumLength = UserValidation.PasswordMinLength,
        ErrorMessage = "Hasło musi mieć od {2} do {1} znaków.")]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    public Role Role { get; set; } = Role.User;
}
