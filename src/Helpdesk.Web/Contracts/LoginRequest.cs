using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Web.Contracts;

public class LoginRequest
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    public string Password { get; set; } = string.Empty;
}
