# Helpdesk

## Nazwa kursu

Testowanie i Jakość Oprogramowania

## Autor

_TODO: imię i nazwisko_

## Temat projektu

Aplikacja webowa typu helpdesk do obsługi zgłoszeń (ticketów).

## Opis projektu

_TODO_

Zarys funkcjonalności:

- logowanie jako użytkownik lub administrator,
- użytkownik: tworzenie, edycja i usuwanie własnych ticketów,
- administrator: zarządzanie użytkownikami (tworzenie, edycja, usuwanie) i wszystkimi ticketami, zmiana statusu ticketu,
- priorytet ticketu (Niski / Średni / Wysoki),
- statusy: Otwarty → W trakcie → Ukończony,
- filtrowanie i sortowanie listy ticketów,
- blokada edycji ticketu przez użytkownika po oznaczeniu go jako ukończony.

## Uruchomienie projektu

Wymagania: .NET SDK 10.0

```bash
dotnet restore
dotnet run --project src/Helpdesk.Web
```

_TODO: adres aplikacji, domyślne konto administratora_

## Testy

```bash
dotnet test
```

### Testy jednostkowe

Lokalizacja: [`tests/Helpdesk.UnitTests`](tests/Helpdesk.UnitTests)

_TODO_

### Testy integracyjne

Lokalizacja: [`tests/Helpdesk.IntegrationTests`](tests/Helpdesk.IntegrationTests)

_TODO_

## Dokumentacja API

_TODO_

## Przypadki testowe dla testera manualnego (TestCase)

| ID | Tytuł | Warunki wstępne | Kroki | Oczekiwany rezultat |
|----|-------|-----------------|-------|---------------------|
| TC01 | _TODO_ | | | |

## Technologie użyte w projekcie

- C# / .NET 10
- ASP.NET Core Razor Pages
- Entity Framework Core + SQLite
- xUnit, FluentAssertions, Moq
- Bootstrap 5
