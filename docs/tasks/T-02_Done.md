# T-02 — Configurație ✅

## Ce am făcut

- **`appsettings.json`**: secțiunile `ConnectionStrings:Default` (`Data Source=notix.db`), `Storage:Root` (`storage`), `Logs:Root` (`logs`), `App:BaseUrl`, `Admin`, `Stripe`, `Recaptcha`, `Smtp` (`Port` 587, `FromName` „Notix”), `Contact:TeamEmail` și `Seed:DemoData` (`false`). Toate secretele sunt goale.
- **`appsettings.Development.json`**:
  - cheile de test reCAPTCHA de la Google;
  - `Seed:DemoData=true`;
  - `App:BaseUrl=http://localhost`;
  - `Storage:Root` și `Logs:Root` setate pe `../../../storage` și `../../../logs`, adică folderele din rădăcina repo-ului.
- **`Configuration/*Options.cs`**: câte o clasă pentru fiecare secțiune: `AppOptions`, `StorageOptions`, `LogsOptions`, `AdminOptions`, `StripeOptions`, `RecaptchaOptions`, `SmtpOptions` (cu `IsConfigured`, adevărat când `Host` nu e gol), `ContactOptions`, `SeedOptions`. Fiecare clasă are o constantă `SectionName`.
- **`Configuration/OptionsServiceCollectionExtensions.cs`** are două metode:
  - `AddNotixOptions(...)` leagă fiecare secțiune cu `IOptions<T>`. Tot aici:
    - căile relative din `Storage:Root` și `Logs:Root` devin absolute, față de folderul proiectului (content root);
    - `/`-ul de la finalul lui `BaseUrl` se elimină;
    - opțiunile se validează cu DataAnnotations: `BaseUrl` e obligatoriu și trebuie să fie un URL valid, rădăcinile de stocare și de loguri sunt obligatorii, iar `Port` trebuie să fie între 1 și 65535;
    - validarea rulează la pornire (`ValidateOnStart`).
  - `LogNotixConfiguration()` scrie configurația efectivă în log la pornire. Secretele apar doar ca „(set)” sau „(not set)”.
- **`Program.cs`**: apelează `AddNotixOptions` și `LogNotixConfiguration`.
- **`deploy/.env.example`**: toate variabilele de mediu, în formatul `Secțiune__Cheie=`, cu comentarii și cu valori implicite pentru server (`/var/lib/notix/...`, `smtp-relay.brevo.com`, `Seed__DemoData=false`).
- **User Secrets**: inițializat pentru `Notix.Api` (`UserSecretsId` adăugat în `.csproj`).

## Verificare

- `dotnet build Notix.sln`: 0 avertismente, 0 erori.
- Am pornit aplicația în Development, cu `Stripe__SecretKey=sk_test_SHOULD_NOT_APPEAR` setat ca variabilă de mediu. Logul arată toate opțiunile: căile au fost transformate corect (`C:\Users\Bogdan\Desktop\Notix\storage` și `...\logs`), iar Stripe apare ca `SecretKey = (set)`. Textul `SHOULD_NOT_APPEAR` apare de 0 ori în output.
- Am pornit aplicația în Production fără `App:BaseUrl`: se oprește din pornire cu `OptionsValidationException: The BaseUrl field is required.`, cum era de așteptat.
- Commit: `fd56b67`.

## Diferențe față de plan

1. **Folder nou: `Configuration/`.** Nu apare în structura din secțiunea 3; l-am adăugat pentru clasele de opțiuni.
2. **`App:BaseUrl` e gol în `appsettings.json`.** Subdomeniul DuckDNS e încă „[DE COMPLETAT]” în plan, așa că nu l-am ghicit. În producție vine din `/etc/notix/notix.env`; dacă lipsește, aplicația nu pornește și spune de ce.
3. **`Storage:Root` și `Logs:Root` pentru Development** duc la rădăcina repo-ului, ca în structura din secțiunea 3, nu în folderul proiectului.
4. **Validare la pornire** pentru `App`, `Storage`, `Logs` și `Smtp`. Nu era cerută explicit, dar oprește aplicația devreme dacă lipsește ceva important. Secretele **nu** sunt obligatorii la pornire, ca aplicația să meargă local și fără Stripe sau SMTP.

## Întrebări / nerezolvate

- **Contul de admin pentru Development.** `Admin:Email` și `Admin:Password` sunt goale, iar planul nu dă valori pentru Development. Trebuie setate în User Secrets înainte de T-05, altfel seed-ul nu are din ce să creeze contul:
  ```
  cd backend/src/Notix.Api
  dotnet user-secrets set "Admin:Email" "admin@notix.test"
  dotnet user-secrets set "Admin:Password" "<parolă>"
  ```
- **Subdomeniul DuckDNS** pentru `App:BaseUrl` în producție este încă „[DE COMPLETAT]” în plan.
