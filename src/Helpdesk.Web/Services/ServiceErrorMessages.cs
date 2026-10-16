namespace Helpdesk.Web.Services;

public static class ServiceErrorMessages
{
    public static string For(ServiceError error) => error switch
    {
        ServiceError.NotFound => "Nie znaleziono zasobu.",
        ServiceError.Forbidden => "Brak uprawnień do wykonania tej operacji.",
        ServiceError.Locked => "Zakończone zgłoszenie nie może być edytowane ani usunięte.",
        _ => "Wystąpił nieoczekiwany błąd."
    };
}
