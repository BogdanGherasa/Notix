# Întrebări deschise / abateri de la plan

## T-01 — Pachete NuGet

- **`UglyToad.PdfPig` → `PdfPig`.** Biblioteca PdfPig (autor UglyToad) se publică pe NuGet cu id-ul `PdfPig`; `UglyToad.PdfPig` este doar namespace-ul din cod. Id-ul `UglyToad.PdfPig` de pe NuGet are doar versiuni neoficiale de tip prerelease. Folosim `PdfPig` 0.1.16, ultima versiune stabilă.
- **⚠ ImageSharp cere cheie de licență pentru Release — DE DECIS.** Proiectul folosește `SixLabors.ImageSharp` 4.1.3. Începând cu 3.2.0, pachetul verifică la build o licență Six Labors: în **Debug** apare doar un avertisment, dar în **Release** (`dotnet publish -c Release`, deploy, CI) build-ul **pică** cu „No Six Labors license found”. Singura versiune fără verificare de licență, 3.1.12, are mai multe vulnerabilități publicate (high severity), deci nu e potrivită pentru codul care validează upload-urile. Variante:
  1. obținem o cheie de la Six Labors (https://sixlabors.com/pricing/; proiectele open-source sau non-comerciale se pot califica pentru o licență gratuită) și o punem în proprietatea MSBuild `SixLaborsLicenseKey`: local prin variabilă de mediu, în GitHub Actions ca secret;
  2. înlocuim ImageSharp cu o bibliotecă MIT (de exemplu SkiaSharp), ceea ce schimbă o decizie din secțiunea 2 a planului.

  Trebuie rezolvat înainte de T-44 (CI/CD) și de Faza 13 (hosting).
- **EF Core fixat pe 10.0.x.** Ultima versiune EF Core (11.x) cere .NET 11. Proiectul țintește .NET 10, deci `Microsoft.EntityFrameworkCore.Sqlite` și `Microsoft.EntityFrameworkCore.Design` sunt fixate pe 10.0.15.
