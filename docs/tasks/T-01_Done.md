# T-01 — Soluția ✅

## Ce am făcut

- **`backend/Notix.sln`**: soluția, în formatul clasic `.sln`. SDK-ul .NET 10 creează implicit `.slnx`, așa că am folosit `--format sln`.
- **`backend/src/Notix.Api`**: proiect creat din template-ul `webapi` cu `--use-controllers`, pentru `net10.0`. Am șters `WeatherForecastController.cs` și `WeatherForecast.cs`, iar din `Notix.Api.http` am scos cererea de exemplu.
- **`backend/tests/Notix.Tests`**: proiect xUnit, cu referință la `Notix.Api`. Testul de exemplu `UnitTest1.cs` a rămas, ca `dotnet test` să aibă ce rula; se înlocuiește în T-40.
- **Pachete NuGet în `Notix.Api`**:

  | Pachet | Versiune |
  |---|---|
  | Microsoft.EntityFrameworkCore.Sqlite | 10.0.15 |
  | Microsoft.EntityFrameworkCore.Design | 10.0.15 |
  | Stripe.net | 53.2.0 |
  | SkiaSharp | 4.157.0 |
  | SkiaSharp.NativeAssets.Linux.NoDependencies | 4.157.0 |
  | PdfPig | 0.1.16 |
  | MailKit | 5.0.0 |
  | Serilog.AspNetCore | 11.0.0 |
  | Serilog.Sinks.File | 7.0.0 |
  | Microsoft.AspNetCore.OpenApi | 10.0.15 (venit din template) |

- **Pachete NuGet în `Notix.Tests`**: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` (din template) și `Microsoft.EntityFrameworkCore.Sqlite` 10.0.15.
- **Structura de foldere din secțiunea 3**: `Controllers`, `Models/{Entities,Enums,Dtos}`, `Data/{Configurations,Migrations}`, `Repositories`, `Services`, `Security/{FileValidation,Captcha}`, `Exceptions`, `Logging`, `Email`, `SeedData`, `frontend/{css,js/pages,img,internal}`, `deploy/{xampp,server}`, `docs`, `.github/workflows`. Fiecare folder gol are un fișier `.gitkeep`, ca Git să-l păstreze.
- **`docs/OPEN-QUESTIONS.md`**: creat, cu abaterile de mai jos.

## Verificare

- `dotnet build Notix.sln`: 0 avertismente, 0 erori.
- `dotnet build Notix.sln -c Release`: 0 avertismente, 0 erori.
- `dotnet test Notix.sln`: 1 test trecut, 0 eșuate.
- `dotnet list Notix.sln package --vulnerable --include-transitive`: niciun pachet vulnerabil, nici în dependențele indirecte.
- Commit-uri: `1b6e20e`, `ad094ea`.

## Diferențe față de plan

1. **`PdfPig` în loc de `UglyToad.PdfPig`.** Pachetul oficial are id-ul NuGet `PdfPig`; `UglyToad.PdfPig` e doar namespace-ul. Sub id-ul `UglyToad.PdfPig`, pe NuGet există doar două versiuni prerelease neoficiale. *Acceptat și trecut în plan.*
2. **EF Core fixat pe 10.0.15.** Ultima versiune (11.x) cere .NET 11. *Acceptat și trecut în plan.*
3. **SkiaSharp în loc de SixLabors.ImageSharp.** ImageSharp 3.2.0 și 4.x cer cheie de licență la build, iar fără ea build-ul în Release pică. Versiunea 3.1.12, care nu cere cheie, are vulnerabilități publicate. *Decis de om: varianta SkiaSharp, cu plan actualizat (secțiunea 2, T-16, T-40). ImageSharp a fost scos complet.*

## Întrebări / nerezolvate

Niciuna. Toate punctele de mai sus sunt marcate ca rezolvate în `docs/OPEN-QUESTIONS.md`.
