# PLAN.md — Notix (proiect web)

> Planul de implementare pentru agentul care scrie codul. Citește tot documentul o dată, apoi lucrează **în ordinea fazelor**, task cu task.
> Specificația funcțională este documentul „Notix – ce este aplicația”. Unde planul și documentul diferă, **are dreptate planul**.

---

## 0. Reguli pentru agent

1. Lucrează fază cu fază, în ordine. Nu începe o fază până nu sunt îndeplinite criteriile „Gata când” ale fazei anterioare.
2. După fiecare task: `dotnet build` fără erori, apoi un commit cu mesajul `[T-xx] descriere scurtă`.
3. **Nu adăuga** funcționalități care nu sunt în plan și **nu schimba** deciziile din secțiunea 2. Dacă ceva e imposibil sau neclar, notează în `docs/OPEN-QUESTIONS.md` și treci mai departe.
4. Codul, numele de fișiere, clasele și comentariile sunt în **engleză**. Tot textul vizibil pentru utilizator (UI, mesaje de eroare, e-mailuri) este în **română, cu diacritice**.
5. Niciun secret în Git: cheile stau în User Secrets (local) și în `/etc/notix/notix.env` (server). În repo există doar `appsettings.json` fără secrete și `.env.example`.
6. Bifează `[x]` taskurile terminate chiar în acest fișier.

---

## 1. Maparea pe barem (ce bifează fiecare parte)

| Notă | Cerință barem | Unde se rezolvă |
|---|---|---|
| 2 | BD în 3NF | T-03, `docs/database.md` |
| 2 | Minim 4 scenarii complete | Cumpărare, publicare, moderare, recenzie, căutare (Fazele 3–6, 8–9) |
| 2 | MVC, Repository | T-01, T-04 (Controllers + Models + View = HTML), Repository |
| 2 | Imagini, text, tabele, linkuri | Faza 8 (Acasă, Despre, Material, Panouri) |
| 3 | ≥3 clase, ≥1 interfață | Peste tot; listă explicită în `docs/oop.md` (T-41) |
| 3 | Autentificare | T-07 |
| 3 | Autorizare pe roluri | T-08 |
| 4 | Moștenire | `BaseEntity` → entități; `FileValidatorBase` → `PdfFileValidator`, `ImageFileValidator`; `NotixException` → excepții proprii |
| 4 | ≥2 tipuri de excepții tratate | T-06 (excepții proprii + `DbUpdateException`, `StripeException`, `SmtpCommandException`) |
| 4 | Carusel | T-31 (pagina materialului + Acasă) |
| 4 | Animație | T-30 (fade-in carduri, toast, badge coș) |
| 5 | ≥2 fișiere CSS, ≥10 proprietăți | T-29 (4 fișiere CSS) |
| 5 | Responsive | T-29, T-30, verificare T-50 |
| 6 | ≥5 teste unitare | T-40 (16 teste) |
| 6 | Git | T-00 |
| 6 | GitHub Actions | T-44 |
| 7 | Log în fișier, consultabil | T-10, T-26 (panou admin → Jurnal) |
| 7 | Hosting | Faza 13 |
| 8 | SEO ≥ 90 Lighthouse | T-32, verificare T-50 |
| 8 | Plăți Stripe | T-20, T-21 |
| 9 | CSRF | T-07 |
| 9 | XSS | T-12, T-28 |
| 9 | SQL Injection | T-04 (doar EF Core LINQ) + test T-40 |
| 9 | Max 12 MB | T-15 |
| 9 | Extensii aprobate | T-15 |
| 10 | Rate limiting 100/min | T-09 |
| 10 | CAPTCHA | T-11 |
| 10 | Anti-steganografie | T-16 |
| 10 | Security Headers nota A | T-12, T-42, T-47 |
| 10 | .htaccess | T-42, T-43 |

---

## 2. Decizii tehnice (fixe)

| Subiect | Decizie |
|---|---|
| Backend | **ASP.NET Core Web API, .NET 10**, cu Controllers (nu Minimal APIs). Un singur proiect `Notix.Api` + proiect de teste `Notix.Tests`. |
| IDE | Visual Studio, pe Windows. Soluția `backend/Notix.sln`. |
| Bază de date | **SQLite** prin **Entity Framework Core** (code-first, migrații). |
| Arhitectură | Controllers → Services → Repositories → DbContext. „View” = paginile HTML din `frontend/`. Controllerele primesc și întorc DTO-uri, niciodată entități. |
| Frontend | **HTML + CSS + JavaScript simplu** (ES modules, fără framework, fără bundler). O pagină HTML separată pentru fiecare ecran (nu SPA). |
| Comunicare | Frontend-ul cheamă `/api/...` cu `fetch`, prin JSON. Upload-urile folosesc `multipart/form-data`. |
| Același origin | Apache servește `frontend/` și redirecționează `/api/*` către Kestrel (`127.0.0.1:5000`). Frontend-ul și API-ul sunt pe același domeniu, deci nu e nevoie de CORS. |
| Autentificare | Cookie (`Notix.Auth`, HttpOnly, SameSite=Strict, Secure în producție), cu claims: `UserId`, `Email`, `Role`, `SecurityStamp`. Fără ASP.NET Identity complet: doar `PasswordHasher<User>` din `Microsoft.AspNetCore.Identity`. |
| CSRF | ASP.NET Core Antiforgery: token în cookie-ul `XSRF-TOKEN` (citibil din JS), trimis înapoi în header-ul `X-XSRF-TOKEN`. Filtru global pe toate POST/PUT/DELETE. Excepție: webhook-ul Stripe. |
| CAPTCHA | **Google reCAPTCHA v2** (căsuța „Nu sunt robot”) pe: autentificare, cont nou, parolă uitată, contact. |
| Plăți | **Stripe Checkout** (redirect, modul test), moneda `ron`. Confirmarea vine prin webhook `checkout.session.completed`, cu verificare suplimentară pe pagina de succes. |
| E-mail | **Brevo SMTP** (`smtp-relay.brevo.com:587`, STARTTLS), prin **MailKit**. În Development, dacă SMTP nu e configurat, e-mailul se scrie în log (`LogEmailSender`). |
| Fișiere | Stocate pe disc în `storage/` (**în afara** folderului public), cu nume GUID. PDF-urile se descarcă doar prin API, cu verificarea drepturilor. |
| Imagini | Reprocesate cu **SixLabors.ImageSharp**: redimensionate la maximum 1600px lățime, **convertite în JPEG la calitate 85**, fără metadate. |
| PDF | Verificat cu **UglyToad.PdfPig** (parsare + număr de pagini) și cu o scanare de octeți după cuvinte-cheie periculoase. |
| Logging | **Serilog** pentru logul tehnic (`logs/app-.log`). Clasă proprie `FileActivityLogger` pentru jurnalul de activitate: JSON pe linii în `logs/activity-YYYY-MM-DD.log`. |
| Rate limit | Middleware-ul ASP.NET Core `RateLimiter`: fereastră fixă, **100 de cereri / 60 s / IP** pe `/api/*`, răspuns `429` cu `Retry-After`. |
| Prețuri | Stocate ca **întreg în bani** (`PriceBani`, 1 leu = 100 bani). SQLite nu poate ordona după `decimal`. Afișare: `24,99 lei`. |
| Date | `DateTime` în UTC (SQLite nu poate ordona după `DateTimeOffset`). |
| Teste | **xUnit**. Fakes scrise de mână, plus SQLite in-memory pentru testele cu DbContext. Fără Moq. |
| Server web | **Apache**: local din **XAMPP** (Windows), în producție `apache2` pe Ubuntu 24.04. |
| Hosting | VPS Ubuntu 24.04, furnizor **[DE COMPLETAT]**, subdomeniu **DuckDNS** (`notix.duckdns.org`, numele exact [DE COMPLETAT]), HTTPS cu **Let's Encrypt (certbot)**. |
| CI/CD | GitHub Actions: build + teste la fiecare push/PR; la push pe `main`, după teste, **deploy automat prin SSH**. |
| Fonturi și iconițe | Stivă de fonturi de sistem (fără Google Fonts). Iconițe și ilustrații **SVG desenate de agent**. Fără imagini cu drepturi de autor. |

### Pachete NuGet

`Notix.Api`: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`, `Stripe.net`, `SixLabors.ImageSharp`, `UglyToad.PdfPig`, `MailKit`, `Serilog.AspNetCore`, `Serilog.Sinks.File`.
`Notix.Tests`: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.EntityFrameworkCore.Sqlite`.

---

## 3. Structura repo-ului

```
notix/
├─ .github/workflows/ci-cd.yml
├─ .gitignore                     # bin/ obj/ storage/ logs/ *.db .env .vs/
├─ README.md
├─ PLAN.md
├─ backend/
│  ├─ Notix.sln
│  ├─ src/Notix.Api/
│  │  ├─ Controllers/
│  │  ├─ Models/
│  │  │  ├─ Entities/             # BaseEntity, User, Material, ...
│  │  │  ├─ Enums/                # UserRole, MaterialStatus, OrderStatus, ActivityAction
│  │  │  └─ Dtos/                 # *Request, *Response
│  │  ├─ Data/                    # NotixDbContext, Configurations/, Migrations/, DbSeeder.cs
│  │  ├─ Repositories/            # IRepository<T>, Repository<T>, I*Repository, *Repository
│  │  ├─ Services/                # I*Service + implementări
│  │  ├─ Security/                # FileValidation/, Captcha/, SecurityHeadersMiddleware.cs
│  │  ├─ Exceptions/              # NotixException + derivate, ExceptionHandlingMiddleware.cs
│  │  ├─ Logging/                 # IActivityLogger, FileActivityLogger
│  │  ├─ Email/                   # IEmailSender, SmtpEmailSender, LogEmailSender
│  │  ├─ SeedData/                # PDF-uri și imagini demo (mici, < 300 KB fiecare)
│  │  ├─ appsettings.json
│  │  ├─ appsettings.Development.json
│  │  └─ Program.cs
│  └─ tests/Notix.Tests/
├─ frontend/                      # DocumentRoot pentru Apache
│  ├─ .htaccess
│  ├─ robots.txt
│  ├─ sitemap.xml
│  ├─ *.html                      # lista din T-28
│  ├─ css/ base.css layout.css components.css pages.css
│  ├─ js/  api.js auth.js ui.js carousel.js layout.js pages/*.js
│  ├─ img/                        # SVG-uri, favicon, imagine hero
│  └─ internal/                   # documentație internă, protejată prin .htaccess
│     ├─ .htaccess
│     └─ index.html
├─ deploy/
│  ├─ xampp/notix-local.conf
│  ├─ server/notix-apache.conf
│  ├─ server/notix.service
│  ├─ server/setup-server.md
│  └─ .env.example
├─ docs/
│  ├─ database.md                 # diagrama ER + justificarea 3NF
│  ├─ oop.md                      # clase, interfețe, moștenire, excepții
│  ├─ security.md                 # fiecare cerință de securitate și unde e implementată
│  ├─ api.md                      # lista endpoint-urilor
│  └─ OPEN-QUESTIONS.md
└─ storage/  (gitignored)         # pdf/, images/
```

---

## 4. Modelul de date (3NF)

Toate entitățile moștenesc `BaseEntity { int Id; DateTime CreatedAt; }`, cu excepția tabelelor de legătură, care au cheie compusă.
Enum-urile se stochează ca `int`, cu constrângere `CHECK`.

| Tabel | Coloane | Constrângeri |
|---|---|---|
| **Users** | Id, Email, PasswordHash, DisplayName, Role (enum: Buyer=1, Seller=2, Admin=3), IsBlocked, SecurityStamp (GUID string), CreatedAt | Email unic (stocat lowercase) |
| **PasswordResetTokens** | Id, UserId FK, TokenHash (SHA-256 hex), ExpiresAt, UsedAt? , CreatedAt | index pe TokenHash |
| **Universities** | Id, Name, City, CreatedAt | Name unic |
| **Faculties** | Id, UniversityId FK, Name, CreatedAt | unic (UniversityId, Name) |
| **Subjects** | Id, FacultyId FK, Name, StudyYear (1–6), CreatedAt | unic (FacultyId, Name, StudyYear) |
| **Categories** | Id, Name, Slug, CreatedAt | Name unic, Slug unic |
| **Materials** | Id, SellerId FK→Users, SubjectId FK, CategoryId FK, Title, Description, PriceBani, PageCount, FileSizeBytes, PdfFileName, CoverFileName, Status (enum: Pending=1, Approved=2, Rejected=3), RejectionReason?, IsDeleted, UpdatedAt, ApprovedAt?, CreatedAt | CHECK PriceBani între 200 și 50000 |
| **MaterialPreviews** | Id, MaterialId FK, FileName, SortOrder (1–4), CreatedAt | unic (MaterialId, SortOrder) |
| **Tags** | Id, Name, CreatedAt | Name unic (lowercase) |
| **MaterialTags** | MaterialId FK, TagId FK | PK (MaterialId, TagId) |
| **CartItems** | UserId FK, MaterialId FK, AddedAt | PK (UserId, MaterialId) |
| **Orders** | Id, BuyerId FK→Users, Status (enum: Pending=1, Paid=2, Canceled=3), StripeSessionId?, PaidAt?, CreatedAt | StripeSessionId unic |
| **OrderItems** | Id, OrderId FK, MaterialId FK, UnitPriceBani, CreatedAt | unic (OrderId, MaterialId) |
| **Reviews** | Id, MaterialId FK, BuyerId FK→Users, Rating (1–5), Comment?, CreatedAt | unic (MaterialId, BuyerId) |
| **ContactMessages** | Id, Name, Email, Subject, Message, UserId? FK, CreatedAt | — |

Reguli de 3NF, de explicat în `docs/database.md`:
- `Materials` **nu** are UniversityId sau FacultyId: se deduc prin Subject → Faculty → University. Altfel ar exista o dependență tranzitivă.
- `StudyYear` stă pe `Subjects`, pentru că anul ține de materie, nu de material.
- **Nu** se stochează valori calculabile: totalul comenzii (suma OrderItems), nota medie (din Reviews), numărul de vânzări și încasările (din OrderItems ale comenzilor plătite).
- `OrderItems.UnitPriceBani` **nu** e redundant: păstrează prețul din momentul cumpărării, care poate fi diferit de prețul curent al materialului.
- „Biblioteca mea” nu e tabel: e lista OrderItems din comenzile cu Status = Paid ale utilizatorului.

`docs/database.md` conține diagrama ER în Mermaid (`erDiagram`).

### Date inițiale (seed)

Mereu, prin migrare sau `DbSeeder`, idempotent:
- **Categorii**: Curs (`curs`), Seminar (`seminar`), Laborator (`laborator`), Subiecte rezolvate (`subiecte-rezolvate`), Proiect (`proiect`).
- **Universități și facultăți**:
  - Universitatea Tehnică din Cluj-Napoca (Cluj-Napoca): Facultatea de Automatică și Calculatoare; Facultatea de Electronică, Telecomunicații și Tehnologia Informației.
  - Universitatea Babeș-Bolyai (Cluj-Napoca): Facultatea de Matematică și Informatică; Facultatea de Științe Economice și Gestiunea Afacerilor.
  - Universitatea Națională de Știință și Tehnologie Politehnica București (București): Facultatea de Automatică și Calculatoare; Facultatea de Electronică, Telecomunicații și Tehnologia Informației.
- **Materii**: 3 pe fiecare facultate, cu ani diferiți (de exemplu: Analiză matematică, anul 1; Programare orientată pe obiecte, anul 2; Baze de date, anul 3).
- **Admin**: un singur cont, creat la pornire din configurație (`Admin:Email`, `Admin:Password`), doar dacă nu există deja un admin.

Doar când `Seed:DemoData = true` (Development):
- utilizatorii `vanzator@notix.test` (Seller) și `cumparator@notix.test` (Buyer), parola `Demo12345!`;
- 10 materiale aprobate, distribuite pe categorii și universități, și 2 materiale în așteptare. Fișierele demo sunt în `SeedData/`; agentul le generează o singură dată (PDF-uri simple cu 2–3 pagini și imagini JPEG) și le copiază în `storage/` la seed;
- câteva recenzii și o comandă plătită pentru cumpărătorul demo.

---

## 5. API — lista completă

Formatul erorilor: `{"error": "mesaj în română", "details": {...}?}`, cu status HTTP corect (400, 401, 403, 404, 409, 413, 415, 429, 500).
Paginare: `{"items": [...], "total": n, "page": p, "pageSize": 12}`.

### Public (fără cont)
| Metodă | Ruta | Descriere |
|---|---|---|
| GET | `/api/config/public` | `{ recaptchaSiteKey }` |
| GET | `/api/auth/csrf` | Setează cookie-ul `XSRF-TOKEN`; 204 |
| GET | `/api/auth/me` | Utilizatorul curent `{id,email,displayName,role}` sau 401 |
| POST | `/api/auth/register` | `{email,password,displayName,role:"Buyer"|"Seller",captchaToken}` |
| POST | `/api/auth/login` | `{email,password,captchaToken}` |
| POST | `/api/auth/logout` | — |
| POST | `/api/auth/forgot-password` | `{email,captchaToken}`; răspunde **mereu** 200, cu același mesaj |
| POST | `/api/auth/reset-password` | `{token,newPassword}` |
| GET | `/api/lookups/categories` | Lista categoriilor |
| GET | `/api/lookups/universities` | Universități, cu facultăți și materii imbricate |
| GET | `/api/materials` | Catalog: `search, universityId, facultyId, categoryId, minPrice, maxPrice` (în lei), `sort=newest|price_asc|price_desc|title`, `page` |
| GET | `/api/materials/featured` | 6 materiale aprobate (cele mai bine notate, apoi cele mai noi) |
| GET | `/api/materials/{id}` | Detalii; include `viewer: {isAuthor, owned, inCart, canBuy, canReview}` |
| GET | `/api/materials/{id}/similar` | Până la 4: aceeași materie, apoi aceeași categorie |
| GET | `/api/materials/{id}/reviews` | Recenziile, cele mai noi primele |
| GET | `/api/materials/{id}/cover` | Imaginea de copertă (JPEG) |
| GET | `/api/materials/{id}/previews/{n}` | Previzualizarea n (1–4) |
| GET | `/api/stats` | `{materials, universities, sellers, sales}` pentru Acasă |
| POST | `/api/contact` | `{name,email,subject,message,captchaToken}` |
| POST | `/api/stripe/webhook` | Fără auth, fără CSRF; semnătura Stripe verificată |

Imaginile unui material neaprobat sau șters sunt vizibile doar pentru autor și admin; altfel 404.

### Buyer + Seller
| Metodă | Ruta | Descriere |
|---|---|---|
| GET | `/api/cart` | Materialele din coș + total |
| POST | `/api/cart/{materialId}` | Adaugă (reguli în T-19) |
| DELETE | `/api/cart/{materialId}` | Scoate din coș |
| POST | `/api/checkout` | Creează comanda Pending + sesiunea Stripe → `{url}` |
| GET | `/api/checkout/confirm?session_id=` | Verifică sesiunea la Stripe, confirmă comanda dacă e plătită → `{status}` |
| GET | `/api/library` | Materialele cumpărate |
| POST | `/api/materials/{id}/reviews` | `{rating, comment}` |

### Buyer + Seller + Admin
| Metodă | Ruta | Descriere |
|---|---|---|
| GET | `/api/materials/{id}/download` | PDF-ul, dacă utilizatorul l-a cumpărat (comandă Paid), e autorul sau e admin; altfel 403 |

### Seller
| Metodă | Ruta | Descriere |
|---|---|---|
| GET | `/api/seller/summary` | `{totalMaterials, approved, pending, rejected, totalSales, totalRevenueBani}` |
| GET | `/api/seller/materials` | Materialele proprii (inclusiv șterse, marcate), cu status, motivul respingerii, vânzări, încasări |
| GET | `/api/seller/materials/{id}` | Pentru formularul de editare |
| POST | `/api/seller/materials` | multipart: `title, description, subjectId, categoryId, price, tags` (separate prin virgulă), `pdf, cover, previews[]` (0–4) |
| PUT | `/api/seller/materials/{id}` | multipart; fișierele sunt opționale. Un `pdf` nou → Status = Pending |
| DELETE | `/api/seller/materials/{id}` | Soft delete (IsDeleted = true) |

### Admin
| Metodă | Ruta | Descriere |
|---|---|---|
| GET | `/api/admin/materials?status=` | Toate materialele, filtrate după status |
| POST | `/api/admin/materials/{id}/approve` | → Approved, ApprovedAt = acum |
| POST | `/api/admin/materials/{id}/reject` | `{reason}` (5–500 caractere) → Rejected |
| PUT | `/api/admin/materials/{id}` | Editează câmpurile text, prețul, materia, categoria |
| DELETE | `/api/admin/materials/{id}` | Soft delete |
| GET | `/api/admin/users` | Toți utilizatorii |
| POST | `/api/admin/users/{id}/block` · `/unblock` | Adminul nu se poate bloca singur |
| GET/POST/PUT/DELETE | `/api/admin/categories[/{id}]` | DELETE → 409 dacă are materiale |
| GET/POST/PUT/DELETE | `/api/admin/universities[/{id}]` | DELETE → 409 dacă are facultăți |
| GET/POST/PUT/DELETE | `/api/admin/faculties[/{id}]` | DELETE → 409 dacă are materii |
| GET/POST/PUT/DELETE | `/api/admin/subjects[/{id}]` | DELETE → 409 dacă are materiale |
| GET | `/api/admin/logs?date=YYYY-MM-DD&userId=&action=` | Liniile din jurnalul zilei, filtrate |
| GET | `/api/admin/logs/download?date=YYYY-MM-DD` | Fișierul jurnalului zilei |

### Reguli de validare (server, oglindite în frontend)
- Email valid, max 254; parolă 8–100 caractere, cel puțin o literă și o cifră; DisplayName 2–50.
- Title 5–120; Description 20–3000; preț 2,00–500,00 lei (max 2 zecimale); maximum 10 etichete, fiecare 2–30 caractere, salvate lowercase.
- Recenzie: rating întreg 1–5; comentariu 0–1000.
- Contact: name 2–80, subject 3–120, message 10–3000.

---

## 6. Fazele și taskurile

### FAZA 0 — Proiectul

- [x] **T-00 Repo Git.** `git init`, `.gitignore` (Visual Studio + `storage/`, `logs/`, `*.db`, `.env`), `README.md` cu titlu și descriere scurtă, branch `main`.
  **Gata când:** primul commit există.
- [ ] **T-01 Soluția.** Creează `backend/Notix.sln` cu `src/Notix.Api` (template `webapi`, cu controllers) și `tests/Notix.Tests` (xunit), cu referință de la teste la Api. Instalează pachetele din secțiunea 2. Creează folderele din secțiunea 3, chiar dacă sunt goale. Șterge controllerul `WeatherForecast`.
  **Gata când:** `dotnet build` și `dotnet test` rulează fără erori.
- [ ] **T-02 Configurație.** `appsettings.json` cu secțiunile `ConnectionStrings:Default` (`Data Source=notix.db`), `Storage:Root`, `Logs:Root`, `App:BaseUrl`, `Admin`, `Stripe` (`SecretKey`, `WebhookSecret`), `Recaptcha` (`SiteKey`, `SecretKey`), `Smtp` (`Host`, `Port`, `User`, `Password`, `FromEmail`, `FromName`), `Contact:TeamEmail`, `Seed:DemoData` — secretele lăsate goale. Clase tipizate de opțiuni (`StripeOptions` etc.) legate cu `IOptions<T>`. În `appsettings.Development.json`: cheile de test reCAPTCHA de la Google (site `6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI`, secret `6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe`), `Seed:DemoData=true`, `App:BaseUrl=http://localhost`. `deploy/.env.example` listează toate variabilele de mediu, în format `Stripe__SecretKey=`.
  **Gata când:** aplicația pornește, iar opțiunile se citesc corect (log la pornire, fără să afișeze secrete).

### FAZA 1 — Date

- [ ] **T-03 Entități și DbContext.** Creează `BaseEntity`, enum-urile și toate entitățile din secțiunea 4. Fiecare entitate are configurația ei în `Data/Configurations/*Configuration.cs` (`IEntityTypeConfiguration<T>`), cu chei, indecși unici, CHECK-uri și `OnDelete(DeleteBehavior.Restrict)` pe FK-uri. **Nu** folosi un query filter global pentru `IsDeleted`: materialele șterse se exclud explicit în interogările de catalog din repository, pentru că biblioteca și descărcarea trebuie să le vadă în continuare. Prima migrare: `InitialCreate`.
  **Gata când:** `dotnet ef database update` creează toate tabelele; `docs/database.md` conține diagrama ER și justificarea 3NF.
- [ ] **T-04 Repository.** `IRepository<T>` (GetByIdAsync, ListAsync(predicate), AddAsync, Update, Remove, SaveChangesAsync) + `Repository<T>`. Repository-uri specifice: `IMaterialRepository` (catalog cu filtre, sortare, paginare; detalii cu Include; similare), `IUserRepository` (după email), `IOrderRepository` (după StripeSessionId; „a cumpărat utilizatorul X materialul Y?”), `IReviewRepository`, `ICartRepository`, `ILookupRepository`. **Regula SQL Injection:** doar LINQ; se interzice `FromSqlRaw` / `ExecuteSqlRaw` cu șiruri concatenate. Căutarea în titlu se face cu `EF.Functions.Like(m.Title, $"%{term}%")`, cu `%` și `_` escapate în `term`.
  **Gata când:** toate se înregistrează în DI (`AddScoped`) și compilează.
- [ ] **T-05 Seeder.** `DbSeeder.SeedAsync()` apelat la pornire, după `Database.MigrateAsync()`. Implementează seed-ul din secțiunea 4. Idempotent: verifică înainte să insereze.
  **Gata când:** rulat de două ori, nu dublează nimic; contul admin există; în Development catalogul are 10 materiale aprobate, cu fișiere în `storage/`.

### FAZA 2 — Fundație și securitate

- [ ] **T-06 Excepții și middleware.** `NotixException` (abstract, cu `StatusCode`) → `NotFoundException` (404), `ForbiddenException` (403), `ValidationException` (400, cu dicționar de erori), `ConflictException` (409), `FileRejectedException` (400 sau 413 sau 415), `PaymentException` (502). `ExceptionHandlingMiddleware` prinde: `NotixException` → status-ul ei; `DbUpdateException` → 409 „Datele intră în conflict cu informații existente”; `StripeException` → 502 „Plata nu a putut fi procesată”; orice altceva → 500 „A apărut o eroare neașteptată” (fără stack trace în răspuns, doar în log).
  **Gata când:** un `throw new NotFoundException(...)` dintr-un controller întoarce 404 JSON în formatul din secțiunea 5.
- [ ] **T-07 Autentificare + CSRF.** `AuthService` cu register, login, logout. Login: email lowercase, verificare hash, **refuz dacă `IsBlocked`** („Contul tău a fost blocat”), mesaj generic la credențiale greșite („Email sau parolă incorecte”). Register: refuză `role = Admin` (400); email deja folosit → 409. Cookie auth configurat ca în secțiunea 2, cu `ExpireTimeSpan` de 7 zile și sliding expiration. Pentru API, `OnRedirectToLogin` → 401 și `OnRedirectToAccessDenied` → 403 (fără redirect). `OnValidatePrincipal`: încarcă utilizatorul; dacă nu există, e blocat sau `SecurityStamp` diferă → `RejectPrincipal()` + `SignOutAsync()`. Antiforgery: `AddAntiforgery(o => o.HeaderName = "X-XSRF-TOKEN")`, `GET /api/auth/csrf` emite token-ul în cookie-ul `XSRF-TOKEN` (HttpOnly=false, SameSite=Strict), plus filtru global `AutoValidateAntiforgeryTokenAttribute`. `[IgnoreAntiforgeryToken]` doar pe webhook-ul Stripe.
  **Gata când:** un POST fără header-ul `X-XSRF-TOKEN` → 400; login → cookie setat; `/api/auth/me` → datele utilizatorului; un utilizator blocat este delogat la următoarea cerere.
- [ ] **T-08 Autorizare.** Politici: `BuyerOrSeller` (Role ∈ {Buyer, Seller}), `SellerOnly`, `AdminOnly`, `AnyUser`. Aplicate cu `[Authorize(Policy=...)]` pe controllere și acțiuni, conform tabelelor din secțiunea 5. Regulile de proprietate (doar autorul își editează materialul) se verifică în servicii și aruncă `ForbiddenException`.
  **Gata când:** adminul primește 403 la `/api/cart`; un buyer primește 403 la `/api/seller/*`; un seller nu poate edita materialul altui seller (403).
- [ ] **T-09 Rate limiting + proxy.** `UseForwardedHeaders` (X-Forwarded-For, X-Forwarded-Proto; KnownProxies = 127.0.0.1 și ::1) **înainte** de rate limiter. `AddRateLimiter`: politică globală, partiționată după IP, `FixedWindowLimiter` (PermitLimit = 100, Window = 60 s, QueueLimit = 0), aplicată pe `/api/*`. La respingere: 429, header `Retry-After: 60`, JSON `{"error":"Prea multe cereri. Încearcă din nou peste un minut."}`, plus scriere în jurnalul de activitate (`RateLimited`).
  **Gata când:** un script care trimite 101 cereri într-un minut primește 429 la cererea 101.
- [ ] **T-10 Jurnalul de activitate.** `IActivityLogger.LogAsync(ActivityAction action, int? userId, string? email, string? details)` + `FileActivityLogger`: scrie o linie JSON `{"timestamp","action","userId","email","ip","details"}` în `{Logs:Root}/activity-YYYY-MM-DD.log`, protejat cu `SemaphoreSlim`. IP-ul vine din `IHttpContextAccessor`. `ActivityAction`: Login, LoginFailed, Logout, Register, PasswordResetRequested, PasswordResetCompleted, MaterialCreated, MaterialUpdated, MaterialDeleted, MaterialApproved, MaterialRejected, OrderCreated, OrderPaid, Download, ReviewCreated, FileRejected, UserBlocked, UserUnblocked, CategoryChanged, CatalogDataChanged, ContactMessage, CaptchaFailed, RateLimited. Metodă de citire: `ReadAsync(date, userId?, action?)`. Serilog separat pentru logul tehnic, în `logs/app-.log` (rotație zilnică, păstrare 14 zile).
  **Gata când:** fiecare acțiune din listă e apelată din serviciul corespunzător (verifică la final, în T-50).
- [ ] **T-11 CAPTCHA.** `ICaptchaVerifier.VerifyAsync(token, ip)` + `RecaptchaVerifier`: POST la `https://www.google.com/recaptcha/api/siteverify` prin `HttpClient` (`AddHttpClient`). Token lipsă sau invalid → `ValidationException("Confirmă că nu ești robot.")` + log `CaptchaFailed`. Apelat din register, login, forgot-password și contact.
  **Gata când:** cu cheile de test Google, verificarea trece; fără token → 400.
- [ ] **T-12 Antete de securitate + XSS pe server.** `SecurityHeadersMiddleware` adaugă pe **toate** răspunsurile:
  - `Content-Security-Policy: default-src 'self'; script-src 'self' https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/; frame-src https://www.google.com/recaptcha/ https://recaptcha.google.com/recaptcha/; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'` (+ `; upgrade-insecure-requests` doar în producție)
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY`
  - `Referrer-Policy: strict-origin-when-cross-origin`
  - `Permissions-Policy: camera=(), microphone=(), geolocation=(), payment=()`
  - `Cross-Origin-Opener-Policy: same-origin`
  - `Strict-Transport-Security: max-age=31536000; includeSubDomains` (doar în producție, pe HTTPS)

  Kestrel: `AddServerHeader = false`. XSS pe server: toate input-urile text se `Trim()`-uiesc și se validează ca lungime. Răspunsurile sunt doar JSON, codat implicit de System.Text.Json. Fișierele se servesc mereu cu `Content-Type` fix (`application/pdf`, `image/jpeg`).
  **Gata când:** `curl -I` pe orice endpoint arată toate antetele.
- [ ] **T-13 Program.cs — ordinea middleware-urilor.** ForwardedHeaders → ExceptionHandling → SecurityHeaders → (Development: StaticFiles din `../../frontend`, ca aplicația să meargă și fără Apache) → RateLimiter → Authentication → Authorization → Antiforgery → MapControllers. Kestrel ascultă pe `http://127.0.0.1:5000`. La pornire: migrare + seed.
  **Gata când:** `dotnet run` → `http://localhost:5000/index.html` se deschide, iar `/api/stats` răspunde.

### FAZA 3 — Catalog public

- [ ] **T-14 Lookups, catalog, detalii.** `LookupsController`, `MaterialsController` (acțiunile publice), `StatsController`, `ConfigController`, cu `CatalogService` / `LookupService`. Catalogul arată doar materialele cu `Status = Approved` și `!IsDeleted`. Sortare implicită: `newest`. Prețurile din filtre vin în lei și se convertesc în bani. Detaliile includ: titlu, descriere, etichete, universitate, facultate, materie, an, categorie, numărul de pagini, mărimea fișierului, prețul, nota medie (rotunjită la o zecimală), numărul de recenzii, URL-urile imaginilor, vânzătorul (DisplayName) și obiectul `viewer`. Materialele neaprobate sau șterse → 404 pentru oricine nu e autor sau admin.
  **Gata când:** toate rutele publice din secțiunea 5 funcționează pe datele demo.

### FAZA 4 — Fișiere și panoul vânzătorului

- [ ] **T-15 Validarea fișierelor (mărime + extensie).** `IFileValidator { Task<ValidatedFile> ValidateAsync(IFormFile file); }`, cu clasa abstractă `FileValidatorBase`, care implementează pașii comuni:
  1. fișier gol → respins;
  2. `Length > 12 * 1024 * 1024` → `FileRejectedException(413, "Fișierul {nume} depășește limita de 12 MB.")`;
  3. extensia (lowercase, după **ultimul** punct) trebuie să fie în lista permisă a clasei derivate; nume cu două extensii (`x.pdf.exe`) sau fără extensie → 415;
  4. **semnătura** (magic bytes) trebuie să corespundă: PDF `25 50 44 46 2D` (`%PDF-`), JPEG `FF D8 FF`, PNG `89 50 4E 47 0D 0A 1A 0A`;
  5. metoda abstractă `InspectContentAsync`, implementată de clasele derivate.

  `PdfFileValidator` permite `.pdf`; `ImageFileValidator` permite `.jpg`, `.jpeg`, `.png`. Fiecare respingere → log `FileRejected`, cu motivul. Limite de request: `[RequestSizeLimit(80 * 1024 * 1024)]` și `FormOptions.MultipartBodyLengthLimit` = 80 MB, doar pe endpoint-urile de upload.
  **Gata când:** testele din T-40 pentru mărime, extensie și semnătură trec.
- [ ] **T-16 Anti-steganografie / conținut ascuns.**
  - **PDF** (`PdfFileValidator.InspectContentAsync`): scanează octeții după `/JavaScript`, `/JS`, `/OpenAction`, `/AA`, `/Launch`, `/EmbeddedFile`, `/EmbeddedFiles`, `/RichMedia`, `/XFA`, `/SubmitForm`, `/ImportData` → respins („PDF-ul conține scripturi, atașamente sau acțiuni ascunse”). Date de peste 1024 de octeți după ultimul `%%EOF` → respins („PDF-ul conține date ascunse după sfârșitul documentului”). Deschide fișierul cu PdfPig: dacă nu se poate parsa sau are 0 pagini → respins. Pentru un PDF valid, întoarce `PageCount`.
  - **Imagini** (`ImageFileValidator.InspectContentAsync`): încarcă imaginea cu ImageSharp (eșec → respins; dimensiuni sub 200×200 sau peste 8000×8000 → respins), `image.Metadata.ExifProfile = null` (la fel IPTC, XMP, ICC), redimensionare la lățime maximă 1600 px și **salvare ca JPEG, calitate 85**. Re-encodarea elimină datele atașate după marcajul de final și distruge datele ascunse în biții mai puțin semnificativi. Se salvează **doar** imaginea re-encodată, niciodată originalul.
  - Documentează în `docs/security.md` ce verifică fiecare pas și de ce.

  **Gata când:** un PDF cu `/JavaScript` e respins; o imagine cu EXIF iese fără EXIF; un PNG cu date adăugate la final iese ca JPEG curat.
- [ ] **T-17 Stocarea fișierelor.** `IFileStorage` + `LocalFileStorage`: `SaveAsync(stream, folder) → fileName` (GUID + extensia fixă `.pdf` sau `.jpg`), `OpenRead`, `Delete`. Folderele: `{Storage:Root}/pdf/` și `{Storage:Root}/images/`. Calea se construiește doar din GUID; nu se folosește niciodată numele trimis de utilizator.
  **Gata când:** fișierele apar în `storage/` cu nume GUID.
- [ ] **T-18 Panoul vânzătorului — API.** `SellerController` + `MaterialService`: creare (validează câmpurile, fișierele; `pdf` și `cover` obligatorii, `previews` 0–4; etichete create dacă nu există; Status = Pending; log `MaterialCreated`), editare (doar autorul; `pdf` nou → re-validare, Status = Pending, RejectionReason = null; imaginile noi le înlocuiesc pe cele vechi; log `MaterialUpdated`), ștergere (soft; log `MaterialDeleted`), listă cu vânzări și încasări calculate, rezumat. Dacă validarea unui fișier eșuează, **nu** se salvează nimic (fișierele salvate deja în request se șterg).
  **Gata când:** prin Swagger sau curl, un seller publică un material care apare ca Pending în lista proprie și nu apare în catalog.

### FAZA 5 — Coș, plată, bibliotecă, recenzii

- [ ] **T-19 Coș.** `CartService.AddAsync(userId, materialId)`, care aruncă: material inexistent, neaprobat sau șters → `NotFoundException`; autorul materialului → `ValidationException("Nu poți cumpăra propriul material.")`; deja cumpărat → `ConflictException("Ai cumpărat deja acest material.")`; deja în coș → `ConflictException("Materialul este deja în coș.")`. Lista și ștergerea sunt simple. Totalul se calculează la citire.
  **Gata când:** regulile sunt acoperite de testele din T-40.
- [ ] **T-20 Checkout Stripe.** `IPaymentService` + `StripePaymentService`. `POST /api/checkout`: coș gol → 400. Se re-validează fiecare articol (aprobat, nu e propriu, nu e deja cumpărat; articolele invalide se scot din coș și se întoarce 409 cu lista lor). Se creează `Order` (Pending) + `OrderItems` cu prețul curent, apoi o sesiune Stripe Checkout: `mode=payment`, `currency=ron`, câte un `line_item` (`price_data.unit_amount = PriceBani`, `product_data.name = Title`) pentru fiecare material, `metadata.orderId`, `client_reference_id = userId`, `success_url = {BaseUrl}/checkout-success.html?session_id={CHECKOUT_SESSION_ID}`, `cancel_url = {BaseUrl}/checkout-cancel.html`. Salvează `StripeSessionId`, log `OrderCreated`, întoarce `{url}`. **Coșul nu se golește aici.**
  **Gata când:** cu cheile de test, butonul duce la pagina Stripe.
- [ ] **T-21 Confirmarea plății.** `OrderService.ConfirmPaidAsync(sessionId)`, **idempotent**: găsește comanda; dacă e deja Paid, nu face nimic; altfel o marchează Paid (`PaidAt`), scoate articolele din coșul cumpărătorului și scrie log `OrderPaid`. Apelată din:
  - `POST /api/stripe/webhook`: citește body-ul brut, `EventUtility.ConstructEvent` cu `WebhookSecret` (semnătură invalidă → 400); pe `checkout.session.completed` cu `payment_status == "paid"` → confirmă; pe `checkout.session.expired` → comanda devine Canceled.
  - `GET /api/checkout/confirm?session_id=`: doar proprietarul comenzii; preia sesiunea de la Stripe (`SessionService.GetAsync`); dacă `PaymentStatus == "paid"` → confirmă. Întoarce `{status}`.

  O comandă **nu** devine Paid în niciun alt mod.
  **Gata când:** plata cu cardul de test `4242 4242 4242 4242` → materialul apare în bibliotecă și dispare din coș; anularea → coșul rămâne neschimbat.
- [ ] **T-22 Bibliotecă + descărcare.** `GET /api/library`: articolele din comenzile Paid, inclusiv materialele șterse ulterior, cu data cumpărării. `GET /api/materials/{id}/download`: permis pentru cumpărător (Paid), autor sau admin; altfel 403. Răspuns: `PhysicalFile`, `application/pdf`, `Content-Disposition: attachment; filename="{titlu-slug}.pdf"`. Log `Download`.
  **Gata când:** un vizitator → 401; un buyer care nu a cumpărat → 403; după cumpărare → fișierul se descarcă.
- [ ] **T-23 Recenzii.** `ReviewService.CreateAsync`: doar cine are materialul într-o comandă Paid; maximum o recenzie per material (409 la a doua); autorul nu își poate recenza materialul. Log `ReviewCreated`.
  **Gata când:** regulile sunt acoperite de testele din T-40.

### FAZA 6 — Admin

- [ ] **T-24 Moderare.** Endpoint-urile `/api/admin/materials*`: aprobare, respingere cu motiv, editare, ștergere, cu logurile aferente. Adminul poate descărca orice PDF (prin T-22).
- [ ] **T-25 Utilizatori, categorii, universități, facultăți, materii.** CRUD conform secțiunii 5. Blocarea setează `IsBlocked = true` și schimbă `SecurityStamp` (delogare imediată). Ștergerea unei entități folosite → `ConflictException` cu mesaj clar. Loguri: `UserBlocked`, `UserUnblocked`, `CategoryChanged`, `CatalogDataChanged`.
- [ ] **T-26 Jurnal.** `GET /api/admin/logs` (citește fișierul zilei și filtrează după userId și action; zi inexistentă → listă goală) și `GET /api/admin/logs/download` (fișierul `.log`; parametrul `date` validat strict cu formatul `yyyy-MM-dd`, ca să nu permită path traversal).
  **Gata când (T-24..T-26):** toate rutele admin funcționează, iar un non-admin primește 403.
- [ ] **T-27 Parolă uitată (e-mail real).** `IEmailSender` + `SmtpEmailSender` (MailKit, Brevo, STARTTLS pe 587) + `LogEmailSender` (Development, când `Smtp:Host` e gol). `PasswordResetService`:
  - **forgot**: dacă emailul există și contul nu e blocat → invalidează token-urile vechi nefolosite, generează 32 de octeți aleatori (`RandomNumberGenerator`), codați base64url, salvează **doar hash-ul SHA-256**, cu `ExpiresAt = acum + 30 min`, apoi trimite un e-mail HTML + text cu link-ul `{BaseUrl}/reset-password.html?token=...`. Răspunsul e mereu același: „Dacă adresa există, vei primi un e-mail cu instrucțiuni.” Log `PasswordResetRequested`.
  - **reset**: hash-uiește token-ul primit, îl caută; dacă e inexistent, expirat sau folosit → 400 „Link invalid sau expirat”. Altfel setează parola nouă, `UsedAt = acum`, **schimbă `SecurityStamp`** (delogare peste tot) și scrie log `PasswordResetCompleted`.
  - Contact: salvează `ContactMessage` + trimite e-mail la `Contact:TeamEmail` + log `ContactMessage`.

  **Gata când:** în Development, link-ul apare în log și resetarea funcționează; cu Brevo configurat, e-mailul ajunge în inbox.

### FAZA 7 — Fundația frontend

- [ ] **T-28 Pagini și JS comun.** Fișiere HTML (toate cu `lang="ro"`, `<meta charset>`, `viewport`, `title` unic, `meta description` unic, `link rel="canonical"`, favicon, header și footer comune):
  `index.html` (Acasă), `catalog.html`, `material.html?id=`, `about.html`, `faq.html`, `contact.html`, `terms.html`, `privacy.html`, `login.html`, `register.html`, `forgot-password.html`, `reset-password.html`, `cart.html`, `library.html`, `seller.html` (panou), `seller-material.html` (formular nou/editare, `?id=` la editare), `admin.html`, `checkout-success.html`, `checkout-cancel.html`, `404.html`, `403.html`.
  JS:
  - `api.js`: funcția `api(path, {method, body, form})` — la prima cerere care modifică date apelează `/api/auth/csrf`; trimite `X-XSRF-TOKEN` citit din cookie, `credentials: 'same-origin'`; parsează JSON; la eroare aruncă `ApiError {status, message, details}`; 401 pe paginile protejate → redirect la `login.html?returnUrl=...`; 429 → toast.
  - `auth.js`: `getCurrentUser()` (din `/api/auth/me`, păstrat în memorie), `requireRole(...roles)` (redirect la login sau 403).
  - `layout.js`: inserează meniul în funcție de rol (vizitator: Catalog, Despre, Întrebări, Contact, Autentificare, Cont nou; buyer/seller: + Coș cu badge, Biblioteca mea; seller: + Panou vânzător; admin: Panou admin, **fără** Coș), meniul hamburger, butonul de logout.
  - `ui.js`: `toast(msg, type)`, `formatPrice(bani)` → „24,99 lei”, `formatSize(bytes)`, `el(tag, attrs, ...children)` (creare DOM sigură), `renderStars(n)`.
  - `carousel.js`: componenta carusel (T-31).
  - `pages/<pagina>.js`: logica fiecărei pagini.

  **Regula XSS pentru frontend (obligatorie):** datele venite de la server se pun în pagină **doar** cu `textContent`, `el()` sau `setAttribute` pe atribute sigure. `innerHTML`, `outerHTML`, `insertAdjacentHTML`, `document.write` și `eval` sunt interzise pentru date dinamice. Fără scripturi inline și fără `onclick=""` în HTML: totul în fișiere `.js` (cerință CSP). URL-urile construite din date folosesc `encodeURIComponent`.
  **Gata când:** toate paginile se deschid, meniul se schimbă după rol, iar consola browserului nu are erori CSP.
- [ ] **T-29 CSS și responsive.** 4 fișiere: `base.css` (variabile CSS pentru culori, spațiere, raze; reset; tipografie; stivă de fonturi de sistem), `layout.css` (header, nav, footer, container, grid), `components.css` (butoane, carduri, formulare, tabele, badge, toast, carusel, stele, paginare, skeleton loader), `pages.css` (stiluri specifice paginilor). Folosește cel puțin: `display:grid`, `flex`, `gap`, `grid-template-columns`, `position`, `transition`, `transform`, `animation`, `box-shadow`, `border-radius`, `@media`, `object-fit`, `aspect-ratio`, `var()`. Breakpoint-uri: telefon ≤ 600px, tabletă 601–1024px, desktop > 1024px. Sub 768px, meniul se ascunde în spatele unui buton hamburger (`aria-expanded`, `aria-controls`). Gridul catalogului: 1, 2 și apoi 4 coloane. Tabelele late (panouri) au scroll orizontal pe mobil. Zonele de atingere au minimum 44×44px, iar textul minimum 16px pe mobil.
  **Gata când:** în DevTools, la 375px, 768px și 1440px, nicio pagină nu are scroll orizontal sau elemente suprapuse.
- [ ] **T-30 Animații.** (1) Cardurile din catalog apar cu fade-in + slide-up (`@keyframes`), eșalonat prin `animation-delay`. (2) Toast-urile intră glisând din dreapta. (3) Badge-ul coșului face „bounce” la adăugare. (4) Skeleton loader pulsând cât timp se încarcă datele. (5) Hover pe carduri: `transform: translateY(-4px)` + umbră. Respectă `@media (prefers-reduced-motion: reduce)`.
- [ ] **T-31 Carusel.** O componentă reutilizabilă `createCarousel(container, slides, {autoplay})`: butoane anterior/următor, puncte, navigare cu săgețile de la tastatură, swipe pe touch, tranziție CSS de tip slide, `aria-label` pe controale, `alt` pe imagini. Folosit pe pagina materialului (copertă + până la 4 previzualizări, fără autoplay) și pe Acasă (materiale recomandate, autoplay la 5 s, oprit la hover).
- [ ] **T-32 SEO.** `robots.txt` (permite totul, blochează `/admin.html`, `/seller*.html`, `/cart.html`, `/library.html`, `/internal/`, `/api/`; linie `Sitemap:`), `sitemap.xml` cu paginile publice, `<meta name="robots" content="noindex">` pe paginile private, imagini cu `alt` și `width`/`height`, linkuri cu text descriptiv (nu „click aici”), un singur `h1` per pagină, ierarhie corectă de titluri, `<a href>` reale pentru navigare (nu butoane cu JS). Nu se adaugă date structurate JSON-LD: nu sunt necesare pentru scorul 90 și ar complica CSP-ul.
  **Gata când:** Lighthouse → SEO ≥ 90 pe `index.html`, `catalog.html`, `material.html?id=1`, `about.html`.

### FAZA 8 — Paginile publice

- [ ] **T-33 Acasă.** Hero (titlu, subtitlu, ilustrație SVG proprie, căutare care trimite la `catalog.html?search=`), carusel cu materiale recomandate, cele 5 categorii cu iconițe SVG (link la catalog filtrat), secțiunea „Cum funcționează” (3 pași pentru cumpărător, 3 pentru vânzător), cifre din `/api/stats`, call-to-action „Devino vânzător”.
- [ ] **T-34 Catalog.** Căutare după titlu (debounce de 300 ms), filtre (universitate → facultate în cascadă, categorie, preț minim/maxim), sortare, paginare. Starea filtrelor se păstrează în query string (link-uri care pot fi trimise). Mesaj pentru zero rezultate. Cardul de material: copertă, titlu, materie, universitate, categorie, preț, stele.
- [ ] **T-35 Pagina materialului.** Carusel, titlu, etichete, **tabel** cu detalii (universitate, facultate, materie, an, categorie, pagini, mărime, preț), descriere, vânzător, nota medie + lista recenziilor, formular de recenzie (doar dacă `viewer.canReview`), materiale asemănătoare. Butonul principal, după `viewer`: vizitator → „Autentifică-te pentru a cumpăra”; în coș → „Vezi coșul”; cumpărat → „Descarcă PDF”; autor → „Materialul tău” (dezactivat); admin → doar „Descarcă pentru verificare”; altfel → „Adaugă în coș”.
- [ ] **T-36 Despre, Întrebări, Contact, Termeni, Confidențialitate.** Despre: cine suntem, cum verificăm materialele (cei doi pași de verificare), **tabel** cu universitățile și facultățile. Întrebări: minimum 10 întrebări, în `<details>`/`<summary>` cu animație, grupate pentru cumpărători și vânzători. Contact: formular + reCAPTCHA. Termeni și Confidențialitate: text real, adaptat la Notix (doar materiale proprii; plăți Stripe în modul test; ce date păstrăm: email, nume, comenzi, jurnal de activitate cu IP). Toate conțin linkuri interne și externe (Stripe, reCAPTCHA).
- [ ] **T-37 Autentificare, cont nou, parolă uitată, resetare.** Formulare cu validare în browser (aceleași reguli ca serverul), reCAPTCHA (widget încărcat din `https://www.google.com/recaptcha/api.js`, cu site key din `/api/config/public`; resetat după fiecare eroare), erorile serverului afișate lângă câmpuri. Cont nou: alegere între „Vreau să cumpăr” și „Vreau să vând”. După login: redirect la `returnUrl` (doar dacă e o cale relativă care începe cu `/` și nu cu `//`) sau la Acasă.

### FAZA 9 — Paginile cu cont

- [ ] **T-38 Coș, checkout, bibliotecă.** Coș: lista materialelor (copertă, titlu, preț, buton de ștergere), total, butonul „Plătește” → `POST /api/checkout` → `window.location = url`. `checkout-success.html`: apelează `/api/checkout/confirm`, afișează „Plată confirmată” cu link la bibliotecă, sau „Se procesează…”, cu reîncercare de 3 ori la câte 2 s. `checkout-cancel.html`: „Plata a fost anulată. Materialele au rămas în coș.” Bibliotecă: grid cu butonul „Descarcă” și link la recenzie.
- [ ] **T-39 Panou vânzător + Panou admin.**
  - `seller.html`: carduri de rezumat (materiale, vânzări, încasări), **tabel** cu titlu, stare (badge colorat), motivul respingerii, vânzări, încasări și acțiuni (Editează, Șterge cu confirmare într-un modal propriu, nu `confirm()`). Buton „Publică un material nou”.
  - `seller-material.html`: formular cu selecție în cascadă universitate → facultate → materie, categorie, preț, etichete, PDF, copertă, previzualizări (max 4). Validare înainte de trimitere: extensie, **12 MB per fișier**, numărul de previzualizări. Previzualizare locală a imaginilor alese, bară de progres la upload (`XMLHttpRequest.upload.onprogress`, cu header-ul CSRF). La editare: avertisment că un PDF nou retrimite materialul la verificare.
  - `admin.html`: tab-uri **Moderare** (filtru după stare; aprobare; respingere cu motiv într-un modal; descărcare PDF; editare; ștergere), **Utilizatori** (tabel, blocare/deblocare), **Categorii**, **Universități** (universități → facultăți → materii, cu adăugare, redenumire, ștergere), **Jurnal** (selector de dată, filtru după utilizator și acțiune, tabel, buton „Descarcă jurnalul zilei”).

### FAZA 10 — Teste

- [ ] **T-40 Teste unitare (16).** În `Notix.Tests`, cu fakes scrise de mână pentru `IActivityLogger`, `IEmailSender`, `IPaymentService` și `ICaptchaVerifier`, plus SQLite in-memory (`DataSource=:memory:`, conexiune deschisă pe durata testului) pentru servicii:
  1. `PdfValidator_RejectsFileWithoutPdfSignature`
  2. `PdfValidator_RejectsPdfContainingJavaScript`
  3. `PdfValidator_RejectsDataAfterEof`
  4. `PdfValidator_AcceptsCleanPdf_AndReturnsPageCount`
  5. `FileValidator_RejectsFileLargerThan12MB`
  6. `FileValidator_RejectsDisallowedExtensions` (Theory: `.exe`, `.php`, `.pdf.exe`, `.svg`, fără extensie)
  7. `ImageValidator_StripsExifAndOutputsJpeg`
  8. `MaterialService_RejectsPriceOutsideRange` (Theory: 1,99 și 500,01)
  9. `MaterialService_ReplacingPdf_ResetsStatusToPending`
  10. `CartService_CannotAddOwnMaterial`
  11. `CartService_CannotAddAlreadyPurchasedMaterial`
  12. `ReviewService_RejectsReviewWithoutPurchase`
  13. `ReviewService_RejectsSecondReview`
  14. `OrderService_ConfirmPaid_IsIdempotent`
  15. `PasswordResetService_RejectsExpiredToken`
  16. `CatalogSearch_TreatsSqlAsPlainText` (căutarea `' OR 1=1 --` întoarce 0 rezultate, iar tabelele rămân intacte)

  Fișierele de test (PDF curat, PDF cu JS, JPEG cu EXIF) se generează în cod sau stau în `tests/Notix.Tests/TestFiles/`.
  **Gata când:** `dotnet test` → toate trec.
- [ ] **T-41 Documentația OOP și de securitate.** `docs/oop.md`: tabel cu clasele principale, interfețele (`IRepository<T>`, `IFileValidator`, `IFileStorage`, `IPaymentService`, `IEmailSender`, `ICaptchaVerifier`, `IActivityLogger`, `I*Service`), cele trei ierarhii de moștenire și excepțiile tratate, fiecare cu calea fișierului. `docs/security.md`: cele 10 cerințe de securitate din barem, fiecare cu „cum funcționează”, „unde e în cod” și „cum se demonstrează”. `docs/api.md`: secțiunea 5, actualizată.

### FAZA 11 — Apache local (XAMPP) și .htaccess

- [ ] **T-42 `frontend/.htaccess`.**
  ```apache
  Options -Indexes
  DirectoryIndex index.html
  ErrorDocument 403 /403.html
  ErrorDocument 404 /404.html

  # Blochează fișierele ascunse (.git, .env, .htaccess etc.)
  <FilesMatch "^\.">
      Require all denied
  </FilesMatch>
  # Blochează fișierele sensibile după extensie
  <FilesMatch "\.(md|log|bak|old|sql|db|sqlite|ini|conf|config|json|env|sh|ps1|cs|csproj)$">
      Require all denied
  </FilesMatch>

  <IfModule mod_headers.c>
      # aceleași antete ca în T-12 (CSP, nosniff, X-Frame-Options, Referrer-Policy,
      # Permissions-Policy, COOP), plus HSTS doar pe HTTPS:
      Header always set Strict-Transport-Security "max-age=31536000; includeSubDomains" "expr=%{HTTPS} == 'on'"
      Header unset X-Powered-By
      # cache: 7 zile pentru css/js/img, fără cache pentru html
  </IfModule>

  <IfModule mod_rewrite.c>
      RewriteEngine On
      # Redirect HTTP → HTTPS doar în producție (dacă există variabila NOTIX_PROD)
      RewriteCond %{ENV:NOTIX_PROD} =1
      RewriteCond %{HTTPS} off
      RewriteRule ^ https://%{HTTP_HOST}%{REQUEST_URI} [L,R=301]
  </IfModule>
  ```
  Agentul completează antetele exact ca în T-12. Folderul `frontend/internal/` are propriul `.htaccess`, cu **Basic Auth** (`AuthType Basic`, `AuthName "Notix intern"`, `AuthUserFile` cu cale absolută către un `.htpasswd` **în afara** DocumentRoot, `Require valid-user`), și o pagină `index.html` cu documentație internă (de exemplu checklist-ul de moderare). Documentează în `docs/security.md` cum se demonstrează: `/internal/` → cere parolă; `/sitemap.xml.bak` sau `/.git/` → 403; un folder fără index → 403 (fără listare).
- [ ] **T-43 Configurația XAMPP.** `deploy/xampp/notix-local.conf` (inclus din `httpd.conf` cu `Include`): activează `mod_proxy`, `mod_proxy_http`, `mod_rewrite`, `mod_headers`; `DocumentRoot` = calea către `frontend/`; `<Directory>` cu `AllowOverride All` și `Require all granted`; `ProxyPreserveHost On`; `ProxyPass /api/ http://127.0.0.1:5000/api/` + `ProxyPassReverse`; `RequestHeader set X-Forwarded-Proto "http"`; `LimitRequestBody 83886080`. Pași în `README.md`: (1) instalează XAMPP, (2) copiază calea repo-ului în conf, (3) adaugă `Include` în `C:\xampp\apache\conf\httpd.conf`, (4) creează `.htpasswd` cu `C:\xampp\apache\bin\htpasswd.exe -c C:\xampp\notix\.htpasswd admin`, (5) pornește Apache din XAMPP Control Panel, (6) pornește API-ul din Visual Studio, (7) deschide `http://localhost/`. Pentru webhook-ul Stripe local: `stripe listen --forward-to http://localhost/api/stripe/webhook`, cu secretul afișat pus în User Secrets.
  **Gata când:** prin `http://localhost/` site-ul complet merge prin Apache, iar regulile din `.htaccess` se pot demonstra.

### FAZA 12 — CI/CD

- [ ] **T-44 `.github/workflows/ci-cd.yml`.**
  - Declanșare: `push` pe orice branch și `pull_request` către `main`.
  - Job `build-test` (ubuntu-latest): `actions/checkout@v4` → `actions/setup-dotnet@v4` (`dotnet-version: 10.0.x`) → `dotnet restore backend/Notix.sln` → `dotnet build -c Release --no-restore` → `dotnet test -c Release --no-build --logger trx` → upload rezultate teste (`actions/upload-artifact@v4`) → `dotnet publish backend/src/Notix.Api -c Release -r linux-x64 --self-contained true -o publish/api` → upload artefact `notix-api` + `frontend/`.
  - Job `deploy`: `needs: build-test`, `if: github.ref == 'refs/heads/main' && github.event_name == 'push'`, `environment: production`. Descarcă artefactele; scrie cheia SSH din secretul `SSH_PRIVATE_KEY` în `~/.ssh/id_ed25519`; `ssh-keyscan` pentru `SSH_HOST`. Cu `rsync -az --delete`: `publish/api/` → `/opt/notix/api/` și `frontend/` → `/var/www/notix/` (**fără** să șteargă `.htpasswd`, `storage` sau baza de date, care stau în afara acestor foldere). Apoi `ssh ... 'sudo systemctl restart notix && sleep 3 && curl -fsS http://127.0.0.1:5000/api/stats > /dev/null'`.
  - Secrete GitHub: `SSH_HOST`, `SSH_USER`, `SSH_PRIVATE_KEY`.
  - Badge-ul CI apare în `README.md`.

  **Gata când:** un push pe un branch rulează build + teste (verde); un push pe `main` face și deploy (după Faza 13).

### FAZA 13 — Hosting (VPS)

> Furnizorul VPS e încă **[DE COMPLETAT]**. Pașii sunt pentru orice Ubuntu 24.04 cu IP public. Agentul scrie `deploy/server/setup-server.md` + fișierele de configurare; comenzile pe server le rulează omul (sau agentul, dacă primește acces SSH).

- [ ] **T-45 Pregătirea serverului** (`setup-server.md`, pas cu pas): utilizator `deploy` cu cheie SSH și `sudo` fără parolă **doar** pentru `systemctl restart notix`; `ufw allow OpenSSH, 80, 443`; `apt install apache2 rsync`; `a2enmod proxy proxy_http rewrite headers ssl`; în `/etc/apache2/conf-available/security.conf`: `ServerTokens Prod`, `ServerSignature Off`; foldere: `/opt/notix/api`, `/var/www/notix`, `/var/lib/notix/{storage,logs}`, `/etc/notix/` (cu proprietar și permisiuni corecte; aplicația rulează ca utilizatorul `notix`).
- [ ] **T-46 Serviciul aplicației.** `deploy/server/notix.service` (systemd): `ExecStart=/opt/notix/api/Notix.Api`, `WorkingDirectory=/opt/notix/api`, `User=notix`, `EnvironmentFile=/etc/notix/notix.env`, `Environment=ASPNETCORE_ENVIRONMENT=Production`, `Environment=ASPNETCORE_URLS=http://127.0.0.1:5000`, `Restart=always`. `notix.env` (permisiuni 600) conține: connection string-ul către `/var/lib/notix/notix.db`, `Storage__Root`, `Logs__Root`, `App__BaseUrl=https://<subdomeniu>.duckdns.org`, cheile Stripe, reCAPTCHA și Brevo, datele adminului, `Seed__DemoData=true` (pentru prezentare).
- [ ] **T-47 Apache + domeniu + HTTPS.** DuckDNS: creează subdomeniul și setează IP-ul VPS-ului. `deploy/server/notix-apache.conf`: VirtualHost pe `:80` pentru `<subdomeniu>.duckdns.org`, `DocumentRoot /var/www/notix`, `AllowOverride All`, `SetEnv NOTIX_PROD 1`, proxy `/api/` ca la T-43, plus `RequestHeader set X-Forwarded-Proto "https"` pe vhost-ul 443. `certbot --apache -d <subdomeniu>.duckdns.org` (redirect automat la HTTPS, reînnoire automată). `.htpasswd` în `/etc/notix/.htpasswd`.
- [ ] **T-48 Servicii externe în producție.** Stripe: webhook în Dashboard (modul test) către `https://<subdomeniu>.duckdns.org/api/stripe/webhook`, cu evenimentele `checkout.session.completed` și `checkout.session.expired`; secretul webhook-ului pus în `notix.env`. reCAPTCHA: adaugă domeniul DuckDNS în consola reCAPTCHA (cheile reale de producție). Brevo: expeditor verificat, cheia SMTP pusă în `notix.env`.
  **Gata când (Faza 13):** `https://<subdomeniu>.duckdns.org` se deschide, iar fluxul complet (cont nou → coș → plată test → descărcare) funcționează online.

### FAZA 14 — Verificare finală

- [ ] **T-49 Scenarii end-to-end** (manual, în browser, documentate în `docs/demo-script.md` ca script pentru prezentare):
  1. Vizitator: caută, filtrează, deschide un material, vede caruselul și recenziile.
  2. Vânzător: cont nou → publică un material (PDF + copertă + 2 previzualizări) → apare „În așteptare”.
  3. Admin: respinge cu motiv → vânzătorul vede motivul → editează și înlocuiește PDF-ul → admin aprobă → apare în catalog.
  4. Cumpărător: adaugă 2 materiale în coș → plătește cu `4242 4242 4242 4242` → bibliotecă → descarcă → lasă recenzie → încearcă a doua recenzie (refuzat).
  5. Plată anulată → coșul rămâne neschimbat.
  6. Admin blochează cumpărătorul → acesta e delogat și nu se mai poate autentifica.
  7. Parolă uitată → e-mail → resetare → login cu parola nouă.
  8. Admin → Jurnal → filtrează după utilizator → descarcă jurnalul zilei.
  9. Securitate: upload PDF de 13 MB (refuzat), `.exe` redenumit `.pdf` (refuzat după semnătură), PDF cu JavaScript (refuzat), 101 cereri pe minut (429), POST fără token CSRF (400), `<script>alert(1)</script>` ca titlu sau recenzie (afișat ca text), `' OR 1=1 --` în căutare (0 rezultate), `/internal/` (cere parolă), `/.git/config` (403).
- [ ] **T-50 Checklist barem.** Rulează și notează rezultatele în `docs/verificare.md`:
  - Lighthouse (mod incognito, mobil și desktop): SEO ≥ 90 pe paginile publice; dacă nu, repară și rulează din nou.
  - Extensia Security Headers Inspector pe Acasă și pe un endpoint API: nota **A** (sau A+).
  - DevTools responsive: 375px, 768px și 1440px pe toate paginile.
  - `dotnet test` verde; GitHub Actions verde.
  - Fiecare `ActivityAction` din T-10 apare în jurnal după scenariile din T-49.
  - Consola browserului: zero erori pe toate paginile.
  - Fiecare rând din secțiunea 1 are o dovadă (fișier sau captură de ecran) în `docs/verificare.md`.

---

## 7. Ce face omul, nu agentul (conturi și chei)

| Ce | Unde | Ce iese |
|---|---|---|
| Repo GitHub (privat sau public) | github.com | URL-ul repo-ului; secretele `SSH_*` (după Faza 13) |
| Stripe (modul test) | dashboard.stripe.com | `sk_test_...`, secretul webhook-ului (`whsec_...`); Stripe CLI instalat local |
| Google reCAPTCHA v2 (checkbox) | google.com/recaptcha/admin | site key + secret key, cu domeniile `localhost` și cel DuckDNS |
| Brevo | brevo.com | expeditor verificat, login și cheie SMTP |
| DuckDNS | duckdns.org | subdomeniul + token |
| VPS | **[DE COMPLETAT]** | IP, acces SSH |
| XAMPP | apachefriends.org | Apache local |
| Lighthouse + Security Headers Inspector | Chrome | verificările din T-50 |

Local, secretele se pun cu: `dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."` (la fel pentru celelalte), din folderul `backend/src/Notix.Api`.

---

## 8. Ce NU intră în proiect

Conform documentului Notix: verificarea adresei de e-mail, pagina de profil, mesaje între utilizatori, rambursări, plata efectivă către vânzători, materiale non-PDF, plăți reale (doar modul test Stripe). Agentul nu le implementează.
