namespace Helpdesk.Web.Services;

public static class ServiceErrorMessages
{
    public static string For(ServiceError error) => error switch
    {
        ServiceError.NotFound => "Nie znaleziono zasobu.",
        ServiceError.Forbidden => "Brak uprawnień do wykonania tej operacji.",
        ServiceError.Locked => "Zakończone zgłoszenie nie może być edytowane ani usunięte.",
        ServiceError.UsernameTaken => "Użytkownik o takiej nazwie już istnieje.",
        ServiceError.LastAdminProtected => "Nie można usunąć ani zdegradować ostatniego administratora.",
        ServiceError.SelfDeletion => "Nie można usunąć własnego konta.",
        _ => "Wystąpił nieoczekiwany błąd."
    };
}
