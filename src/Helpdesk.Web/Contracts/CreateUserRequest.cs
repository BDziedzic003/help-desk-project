using System.ComponentModel.DataAnnotations;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Contracts;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    [StringLength(HelpdeskDbContext.UsernameMaxLength, ErrorMessage = "Nazwa użytkownika może mieć maksymalnie {1} znaków.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [StringLength(UserValidation.PasswordMaxLength, MinimumLength = UserValidation.PasswordMinLength,
        ErrorMessage = "Hasło musi mieć od {2} do {1} znaków.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.User;
}
