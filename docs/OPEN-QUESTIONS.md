# Întrebări deschise / abateri de la plan

## T-01 — Pachete NuGet

- ✅ **REZOLVAT — `UglyToad.PdfPig` → `PdfPig`.** Biblioteca PdfPig (autor UglyToad) se publică pe NuGet cu id-ul `PdfPig`; `UglyToad.PdfPig` este doar namespace-ul din cod. Id-ul `UglyToad.PdfPig` de pe NuGet are doar versiuni neoficiale de tip prerelease. Folosim `PdfPig` 0.1.16, ultima versiune stabilă.
  *Decizie: acceptat, trecut în PLAN.md (secțiunea 2).*
- ✅ **REZOLVAT — ImageSharp cerea cheie de licență pentru Release.** Începând cu 3.2.0, `SixLabors.ImageSharp` verifică la build o licență Six Labors, iar fără ea build-ul în Release pică. Singura versiune fără verificare de licență (3.1.12) are vulnerabilități publicate.
  *Decizie: ImageSharp a fost scos complet și înlocuit cu `SkiaSharp` + `SkiaSharp.NativeAssets.Linux.NoDependencies` (MIT). PLAN.md este actualizat (secțiunea 2, T-16, T-40). Verificat: `dotnet build -c Release` are 0 avertismente, iar `dotnet list package --vulnerable --include-transitive` nu găsește nimic.*
- ✅ **REZOLVAT — EF Core fixat pe 10.0.x.** Ultima versiune EF Core (11.x) cere .NET 11. Proiectul țintește .NET 10, deci `Microsoft.EntityFrameworkCore.Sqlite` și `Microsoft.EntityFrameworkCore.Design` sunt fixate pe 10.0.15.
  *Decizie: acceptat, trecut în PLAN.md (secțiunea 2).*

## T-02 — Configurație

- ⏳ **DESCHIS — Contul de admin în Development.** `Admin:Email` și `Admin:Password` sunt goale, iar planul nu dă valori pentru Development. Trebuie setate în User Secrets înainte de T-05 (`dotnet user-secrets set "Admin:Email" ...` și `dotnet user-secrets set "Admin:Password" ...` în `backend/src/Notix.Api`).
- ⏳ **DESCHIS — Subdomeniul DuckDNS.** `App:BaseUrl` pentru producție e gol, pentru că numele exact al subdomeniului este încă „[DE COMPLETAT]” în plan.
