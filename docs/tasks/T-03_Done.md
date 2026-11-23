# T-03 — Entități și DbContext ✅

## Ce am făcut

- **`Models/Enums/`**: `UserRole` (Buyer = 1, Seller = 2, Admin = 3), `MaterialStatus` (Pending = 1, Approved = 2, Rejected = 3), `OrderStatus` (Pending = 1, Paid = 2, Canceled = 3).
- **`Models/Entities/`**:
  - `BaseEntity` (abstract: `Id`, `CreatedAt`);
  - cele 15 entități din secțiunea 4: `User`, `PasswordResetToken`, `University`, `Faculty`, `Subject`, `Category`, `Material`, `MaterialPreview`, `Tag`, `MaterialTag`, `CartItem`, `Order`, `OrderItem`, `Review`, `ContactMessage`;
  - toate au proprietăți de navigare, pentru `Include` în repository-uri;
  - `MaterialTag` și `CartItem` nu moștenesc `BaseEntity`, pentru că au cheie compusă.
- **`Data/Configurations/*Configuration.cs`**: câte un `IEntityTypeConfiguration<T>` pentru fiecare entitate, cu:
  - chei primare (compuse pentru `MaterialTags` și `CartItems`);
  - lungimi maxime, luate din regulile de validare din secțiunea 5;
  - indecșii unici din secțiunea 4;
  - `OnDelete(DeleteBehavior.Restrict)` pe toate cele 17 chei străine;
  - CHECK-uri: `PriceBani` între 200 și 50000, `StudyYear` între 1 și 6, `SortOrder` între 1 și 4, `Rating` între 1 și 5, câte un CHECK pentru fiecare coloană enum (`Role`, `Materials.Status`, `Orders.Status`), plus `Email` și `Tags.Name` lowercase.
- **`Data/NotixDbContext.cs`**:
  - câte un `DbSet` pentru fiecare tabel;
  - `ApplyConfigurationsFromAssembly`;
  - convertor care citește toate `DateTime` cu `Kind = Utc`;
  - suprascrierea `SaveChanges` / `SaveChangesAsync`: completează `CreatedAt` și `CartItems.AddedAt` la inserare (dacă au rămas goale) și `Materials.UpdatedAt` la inserare și modificare;
  - **fără** query filter global pentru `IsDeleted`.
- **`Program.cs`**: `AddDbContext<NotixDbContext>` cu `UseSqlite(ConnectionStrings:Default)`.
- **`Data/Migrations/`**: migrarea `InitialCreate` și snapshot-ul modelului.
- **`backend/dotnet-tools.json`**: manifest de tool local, cu `dotnet-ef` 10.0.15.
- **`docs/database.md`**: convenții, diagrama ER în Mermaid (`erDiagram`), tabelul de constrângeri și justificarea 1NF / 2NF / 3NF.
- Am șters fișierele `.gitkeep` din `Models/Entities`, `Models/Enums`, `Data/Configurations` și `Data/Migrations`, pentru că folderele nu mai sunt goale.

## Verificare

- `dotnet build Notix.sln`: 0 avertismente, 0 erori.
- `dotnet ef migrations add InitialCreate --output-dir Data/Migrations`: reușit. Migrarea are 17 FK-uri cu `ReferentialAction.Restrict`, niciunul cu `Cascade`, și toate cele 9 CHECK-uri.
- `dotnet ef database update`: a creat `notix.db` cu toate cele 15 tabele (plus `__EFMigrationsHistory`) și 25 de indecși.
- Am testat schema creată, pe o copie în memorie:
  - email cu majuscule → `CHECK constraint failed: CK_Users_Email_Lowercase`;
  - `Role = 4` → `CK_Users_Role`;
  - email duplicat → `UNIQUE constraint failed: Users.Email`;
  - `StudyYear = 7` → `CK_Subjects_StudyYear`;
  - `PriceBani = 199` → `CK_Materials_PriceBani`, iar `2499` trece;
  - ștergerea unei universități folosite → `FOREIGN KEY constraint failed`.

## Diferențe față de plan

1. **`dotnet-ef` ca tool local.** Tool-ul global instalat pe mașină era 9.0.21, mai vechi decât EF Core 10. Am adăugat `backend/dotnet-tools.json` cu `dotnet-ef` 10.0.15, ca versiunea să fie aceeași pe orice mașină (și în GitHub Actions): `dotnet tool restore`, apoi `dotnet ef ...`.
2. **CHECK-uri în plus.**
   - Pe coloanele enum (`Users.Role`, `Materials.Status`, `Orders.Status`), cerute de regula „enum-urile se stochează ca `int`, cu constrângere `CHECK`” din secțiunea 4.
   - Pe lowercase pentru `Users.Email` și `Tags.Name`, care impun în baza de date regula „stocat lowercase”.
3. **Lungimi maxime ale coloanelor** care nu sunt fixate de plan (de exemplu `Universities.Name` 150, `City` 80, `RejectionReason` 500, numele de fișiere 100, `StripeSessionId` 255). SQLite nu le impune, dar sunt documentație utilă în model.
4. **Completarea automată a datelor** (`CreatedAt`, `AddedAt`, `UpdatedAt`) și citirea lor ca UTC, în `NotixDbContext`, ca serviciile să nu poată uita să le seteze.
5. **Enum-ul `ActivityAction`** nu e creat aici, deși apare în `Models/Enums` în secțiunea 3: nu ține de baza de date și e definit în T-10, unde va fi creat.

## Întrebări / nerezolvate

Niciuna.
