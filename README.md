# Helpdesk

## Nazwa kursu

Testowanie i Jakość Oprogramowania

## Autor

Bartłomiej Dziedzic

## Temat projektu

Aplikacja webowa typu helpdesk do obsługi zgłoszeń.

## Opis projektu

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

Aplikacja uruchamia się pod adresem <http://localhost:5080>. Przy pierwszym starcie sama tworzy bazę SQLite
(`src/Helpdesk.Web/helpdesk.db`) i domyślne konto administratora:

| Nazwa użytkownika | Hasło |
|-------------------|-------|
| `admin`           | `admin` |

Dane domyślnego konta można zmienić w sekcji `AdminSeed` pliku `src/Helpdesk.Web/appsettings.json`
(konto powstaje tylko wtedy, gdy użytkownik o takiej nazwie jeszcze nie istnieje).
Zwykłych użytkowników tworzy administrator w panelu **Użytkownicy**.

## Testy

```bash
dotnet test
```

### Testy jednostkowe

Lokalizacja: [`tests/Helpdesk.UnitTests`](tests/Helpdesk.UnitTests)

65 testów (xUnit + FluentAssertions), każdy w układzie AAA (`// Arrange`, `// Act`, `// Assert`) i nazwany według
schematu `NazwaMetody_Scenariusz_OczekiwanyWynik`. Testy serwisów działają na prawdziwym schemacie bazy
(SQLite w pamięci), a czas jest podmieniany przez `TimeProvider`.

| Obszar | Co jest sprawdzane |
|--------|--------------------|
| `Pbkdf2PasswordHasher` | hash nie zawiera hasła, losowa sól, poprawne i błędne hasło, uszkodzony hash |
| `AuthService` | logowanie poprawne, złe hasło, nieznany użytkownik, rola administratora |
| `ClaimsPrincipalFactory`, `ClaimsPrincipalExtensions` | claimy tożsamości i ich odtworzenie |
| `DbSeeder` | utworzenie administratora z zahashowanym hasłem, brak duplikatu przy ponownym starcie |
| `TicketService` (zapytania) | widoczność zgłoszeń (użytkownik / administrator), filtry statusu, priorytetu i tytułu, sortowanie |
| `TicketService` (operacje) | tworzenie, edycja, usuwanie, blokada zakończonego zgłoszenia, zmiana statusu tylko przez administratora |
| `TicketRules` | reguła blokady zakończonego zgłoszenia |
| `UserService` | CRUD użytkowników, unikalność nazwy, ochrona ostatniego administratora, zakaz usunięcia własnego konta |
| `TicketDisplayExtensions`, `UserDisplayExtensions` | polskie etykiety i style statusów, priorytetów i ról |

### Testy integracyjne

Lokalizacja: [`tests/Helpdesk.IntegrationTests`](tests/Helpdesk.IntegrationTests)

27 testów na prawdziwej aplikacji uruchamianej przez `WebApplicationFactory<Program>`. Każdy test dostaje własną,
tymczasową bazę SQLite (usuwaną po teście), więc testy są od siebie niezależne i nie ruszają lokalnej bazy.

| Plik | Co jest sprawdzane |
|------|--------------------|
| `AuthApiTests` | logowanie i wylogowanie przez API, brak dostępu bez logowania |
| `TicketsApiTests` | CRUD zgłoszeń, walidacja, widoczność zgłoszeń, filtrowanie i sortowanie, blokada zakończonego zgłoszenia, zmiana statusu |
| `UsersApiTests` | dostęp tylko dla administratora, tworzenie użytkownika, duplikat nazwy, usuwanie użytkownika razem ze zgłoszeniami |
| `PagesTests` | strona logowania, przekierowanie niezalogowanego, logowanie formularzem, panel administratora |
| `SwaggerTests` | dostępność dokumentacji OpenAPI i Swagger UI |

## Dokumentacja API

Dokumentacja wygenerowana przez Swagger (OpenAPI) jest dostępna po uruchomieniu aplikacji pod adresem
<http://localhost:5080/swagger> (specyfikacja JSON: `/swagger/v1/swagger.json`).

Uwierzytelnianie odbywa się ciasteczkiem. W Swagger UI najpierw wywołaj `POST /api/auth/login`
(np. `admin` / `admin`), a przeglądarka dołączy ciasteczko do kolejnych zapytań.

| Metoda | Adres | Opis | Dostęp |
|--------|-------|------|--------|
| POST | `/api/auth/login` | logowanie | wszyscy |
| POST | `/api/auth/logout` | wylogowanie | zalogowani |
| GET | `/api/tickets` | lista zgłoszeń (filtry: `status`, `priority`, `search`; sortowanie: `sortBy`, `descending`) | zalogowani (użytkownik: własne, administrator: wszystkie) |
| GET | `/api/tickets/{id}` | pojedyncze zgłoszenie | zalogowani |
| POST | `/api/tickets` | utworzenie zgłoszenia | zalogowani |
| PUT | `/api/tickets/{id}` | edycja zgłoszenia (użytkownik nie może edytować zakończonego) | zalogowani |
| DELETE | `/api/tickets/{id}` | usunięcie zgłoszenia (użytkownik nie może usunąć zakończonego) | zalogowani |
| PUT | `/api/tickets/{id}/status` | zmiana statusu | administrator |
| GET | `/api/users` | lista użytkowników | administrator |
| GET | `/api/users/{id}` | pojedynczy użytkownik | administrator |
| POST | `/api/users` | utworzenie użytkownika | administrator |
| PUT | `/api/users/{id}` | edycja użytkownika (nazwa, rola, opcjonalnie hasło) | administrator |
| DELETE | `/api/users/{id}` | usunięcie użytkownika wraz z jego zgłoszeniami | administrator |

Kody odpowiedzi: `400` – niepoprawne dane, `401` – brak logowania, `403` – brak uprawnień,
`404` – zasób nie istnieje (także cudze zgłoszenie), `409` – konflikt (zakończone zgłoszenie, zajęta nazwa
użytkownika, ostatni administrator, usunięcie własnego konta).

## Przypadki testowe dla testera manualnego (TestCase)

| ID | Tytuł | Warunki wstępne | Kroki | Oczekiwany rezultat |
|----|-------|-----------------|-------|---------------------|
| TC01 | Logowanie administratora poprawnymi danymi | Aplikacja uruchomiona, użytkownik niezalogowany | 1. Otwórz `http://localhost:5080/Account/Login`. 2. Wpisz `admin` i hasło `admin`. 3. Kliknij „Zaloguj". | Następuje przekierowanie na stronę główną. W pasku widać nazwę `admin`, plakietkę „Administrator" i przycisk „Wyloguj". |
| TC02 | Logowanie błędnym hasłem | Użytkownik niezalogowany | 1. Otwórz stronę logowania. 2. Wpisz `admin` i hasło `zle`. 3. Kliknij „Zaloguj". | Użytkownik pozostaje na stronie logowania i widzi komunikat „Nieprawidłowa nazwa użytkownika lub hasło." |
| TC03 | Pole hasła jest maskowane | Użytkownik niezalogowany | 1. Otwórz stronę logowania. 2. Wpisz dowolne hasło w polu „Hasło". | Wpisane znaki są wyświetlane jako kropki. |
| TC04 | Dostęp do zgłoszeń bez logowania | Użytkownik niezalogowany | 1. Wpisz w przeglądarce `http://localhost:5080/Tickets`. | Następuje przekierowanie na stronę logowania. |
| TC05 | Utworzenie użytkownika przez administratora | Zalogowany administrator | 1. Wybierz „Użytkownicy" w menu. 2. Kliknij „Nowy użytkownik". 3. Wpisz nazwę `jan`, hasło `haslo123`, rolę „Użytkownik". 4. Kliknij „Utwórz". | Użytkownik `jan` pojawia się na liście z rolą „Użytkownik" i komunikatem „Użytkownik został utworzony." |
| TC06 | Próba utworzenia użytkownika o zajętej nazwie | Istnieje użytkownik `jan` | 1. Otwórz „Nowy użytkownik". 2. Wpisz nazwę `jan` i dowolne hasło (min. 4 znaki). 3. Kliknij „Utwórz". | Formularz pozostaje otwarty z komunikatem „Użytkownik o takiej nazwie już istnieje." |
| TC07 | Utworzenie zgłoszenia przez użytkownika | Zalogowany użytkownik `jan` | 1. Kliknij „Nowe zgłoszenie". 2. Wpisz tytuł „Nie działa drukarka", opis i priorytet „Wysoki". 3. Kliknij „Zapisz". | Zgłoszenie pojawia się na liście ze statusem „Otwarte" i priorytetem „Wysoki". Widoczny jest komunikat „Zgłoszenie zostało utworzone." |
| TC08 | Walidacja pustego tytułu | Zalogowany użytkownik | 1. Otwórz „Nowe zgłoszenie". 2. Zostaw tytuł pusty, wpisz opis. 3. Kliknij „Zapisz". | Zgłoszenie nie zostaje zapisane, przy polu tytułu widać komunikat „Tytuł jest wymagany." |
| TC09 | Użytkownik widzi tylko własne zgłoszenia | Istnieją zgłoszenia użytkowników `jan` i `anna` | 1. Zaloguj się jako `jan`. 2. Otwórz „Moje zgłoszenia". | Na liście są wyłącznie zgłoszenia użytkownika `jan`. |
| TC10 | Administrator widzi wszystkie zgłoszenia | Istnieją zgłoszenia użytkowników `jan` i `anna` | 1. Zaloguj się jako `admin`. 2. Otwórz „Wszystkie zgłoszenia". | Na liście są zgłoszenia obu użytkowników, w tabeli widoczna jest kolumna „Autor". |
| TC11 | Edycja własnego zgłoszenia | Zalogowany `jan`, ma otwarte zgłoszenie | 1. Kliknij „Edytuj" przy zgłoszeniu. 2. Zmień tytuł i priorytet. 3. Kliknij „Zapisz zmiany". | Lista pokazuje nowy tytuł i priorytet oraz zaktualizowaną datę w kolumnie „Zaktualizowano". |
| TC12 | Usunięcie własnego zgłoszenia | Zalogowany `jan`, ma otwarte zgłoszenie | 1. Kliknij „Usuń" przy zgłoszeniu. 2. Na stronie potwierdzenia kliknij „Usuń". | Zgłoszenie znika z listy, widoczny jest komunikat „Zgłoszenie zostało usunięte." |
| TC13 | Zmiana statusu przez administratora | Istnieje otwarte zgłoszenie użytkownika `jan`, zalogowany `admin` | 1. Otwórz „Wszystkie zgłoszenia". 2. W wierszu zgłoszenia wybierz status „W trakcie". 3. Kliknij „Zmień". | Status zgłoszenia zmienia się na „W trakcie" i pojawia się komunikat o zmianie statusu. |
| TC14 | Blokada edycji zakończonego zgłoszenia | Administrator ustawił zgłoszenie użytkownika `jan` na „Zakończone" | 1. Zaloguj się jako `jan`. 2. Otwórz „Moje zgłoszenia". 3. Spróbuj wejść na `/Tickets/Edit/{id}` tego zgłoszenia. | Przy zgłoszeniu zamiast przycisków widać „Zablokowane". Wejście na adres edycji przekierowuje na listę z komunikatem „Zakończone zgłoszenie nie może być edytowane ani usunięte." |
| TC15 | Administrator edytuje zakończone zgłoszenie | Istnieje zgłoszenie ze statusem „Zakończone", zalogowany `admin` | 1. Otwórz „Wszystkie zgłoszenia". 2. Kliknij „Edytuj" przy zakończonym zgłoszeniu. 3. Zmień tytuł i zapisz. | Zmiana zostaje zapisana, komunikat „Zgłoszenie zostało zaktualizowane." |
| TC16 | Filtrowanie listy zgłoszeń | Istnieje kilka zgłoszeń o różnych statusach i priorytetach | 1. Otwórz listę zgłoszeń. 2. Wybierz status „Otwarte" i priorytet „Wysoki". 3. Kliknij „Filtruj". | Na liście są tylko otwarte zgłoszenia o wysokim priorytecie. |
| TC17 | Wyszukiwanie po tytule | Istnieje zgłoszenie z tytułem „Nie działa drukarka" | 1. W polu „Szukaj w tytule" wpisz `DRUKARKA`. 2. Kliknij „Filtruj". | Lista zawiera zgłoszenie „Nie działa drukarka" (wielkość liter nie ma znaczenia). |
| TC18 | Sortowanie po priorytecie | Istnieją zgłoszenia o priorytetach niski, średni i wysoki | 1. Wybierz „Sortuj wg" → „Priorytetu" i „Kolejność" → „Malejąco". 2. Kliknij „Filtruj". | Kolejność na liście: Wysoki, Średni, Niski. |
| TC19 | Brak dostępu użytkownika do panelu administratora | Zalogowany zwykły użytkownik | 1. Wpisz w przeglądarce `http://localhost:5080/Admin/Users`. | Wyświetla się strona „Brak dostępu". W menu użytkownika nie ma linku „Użytkownicy". |
| TC20 | Ochrona przed usunięciem własnego konta | Zalogowany `admin` | 1. Otwórz „Użytkownicy". 2. Sprawdź wiersz z kontem `admin`. | Przy własnym koncie nie ma przycisku „Usuń". |
| TC21 | Usunięcie użytkownika razem ze zgłoszeniami | Istnieje użytkownik `jan` z co najmniej jednym zgłoszeniem, zalogowany `admin` | 1. Otwórz „Użytkownicy". 2. Kliknij „Usuń" przy `jan`. 3. Przeczytaj ostrzeżenie i kliknij „Usuń". | Użytkownik znika z listy, a jego zgłoszenia znikają z „Wszystkie zgłoszenia". |
| TC22 | Wylogowanie | Zalogowany użytkownik | 1. Kliknij „Wyloguj" w pasku. 2. Spróbuj otworzyć `/Tickets`. | Użytkownik zostaje wylogowany, a otwarcie `/Tickets` przekierowuje na stronę logowania. |
| TC23 | Dokumentacja API | Aplikacja uruchomiona | 1. Otwórz `http://localhost:5080/swagger`. | Widoczna jest dokumentacja „Helpdesk API" z endpointami `auth`, `tickets` i `users`. |

## Technologie użyte w projekcie

- C# / .NET 10
- ASP.NET Core Razor Pages
- Entity Framework Core + SQLite
- xUnit, FluentAssertions, Moq
- Microsoft.AspNetCore.Mvc.Testing (testy integracyjne)
- Swashbuckle (Swagger / OpenAPI)
- Bootstrap 5
