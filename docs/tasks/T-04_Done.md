# T-04 — Repository ✅

## Ce am făcut

Toate fișierele sunt în `backend/src/Notix.Api/Repositories/`, împărțite pe subfoldere (cu namespace-ul corespunzător):

```
Repositories/
├─ Interfaces/        IRepository<T>, IMaterialRepository, IUserRepository, IOrderRepository,
│                     IReviewRepository, ICartRepository, ILookupRepository
├─ Implementations/   Repository<T>, MaterialRepository, UserRepository, OrderRepository,
│                     ReviewRepository, CartRepository, LookupRepository
├─ Common/            PagedResult<T>, MaterialCatalogQuery (+ MaterialSort), RatingSummary, LikePattern
└─ RepositoryServiceCollectionExtensions.cs
```

Codul nu are comentarii.

- **`IRepository<T>` + `Repository<T>`**: repository-ul generic, cu `GetByIdAsync`, `ListAsync(predicate)`, `AddAsync`, `Update`, `Remove`, `SaveChangesAsync`. `GetByIdAsync` primește `params object[]`, ca să meargă și pentru cheile compuse (`CartItem`: `GetByIdAsync(userId, materialId)`).
- **`IMaterialRepository` / `MaterialRepository`**:
  - `GetCatalogAsync(MaterialCatalogQuery)`: doar materialele `Approved` și `!IsDeleted`; filtre după căutare în titlu, universitate, facultate, categorie, preț minim și maxim (în bani); sortare `Newest` / `PriceAsc` / `PriceDesc` / `Title`, cu `Id` ca departajare, ca paginarea să fie stabilă; paginare (12 pe pagină implicit), cu total;
  - `GetDetailsAsync(id)`: materialul cu vânzătorul, categoria, materia → facultatea → universitatea, etichetele și previzualizările (ordonate după `SortOrder`). Îl încarcă oricare ar fi statusul: serviciul (T-14) decide cine îl poate vedea;
  - `GetSimilarAsync(material, 4)`: aceeași materie întâi, apoi aceeași categorie;
  - `GetFeaturedAsync(6)`: cele mai bine notate, apoi cele mai noi (pentru `/api/materials/featured`).
- **`MaterialCatalogQuery`** (cu enum-ul `MaterialSort`) și **`PagedResult<T>`**: parametrii catalogului și rezultatul paginat.
- **`LikePattern`**: construiește modelul pentru `LIKE`, cu `\`, `%` și `_` escapate. Căutarea se face cu `EF.Functions.Like(m.Title, pattern, "\\")`, iar SQL-ul generat este `"Title" LIKE @pattern ESCAPE '\'` (parametru, niciodată concatenat).
- **`IUserRepository`**: `GetByEmailAsync`, `EmailExistsAsync` (email normalizat cu `Trim()` + lowercase).
- **`IOrderRepository`**: `GetByStripeSessionIdAsync` (cu articolele), `HasPurchasedAsync(userId, materialId)` (doar comenzile `Paid`).
- **`IReviewRepository`**: `GetForMaterialAsync` (cele mai noi primele, cu autorul), `ExistsAsync(materialId, buyerId)`, `GetRatingSummaryAsync` (media și numărul, calculate la citire, fără să fie stocate).
- **`ICartRepository`**: `GetItemsAsync(userId)` (cu materialul, categoria, materia, facultatea, universitatea), `GetItemAsync`, `ContainsAsync`, `RemoveItemsAsync(userId, materialIds)` (pentru T-21; doar marchează ștergerea, salvarea o face serviciul).
- **`ILookupRepository`**: `GetCategoriesAsync`, `GetUniversitiesWithFacultiesAndSubjectsAsync` (imbricate, ordonate), `GetSubjectAsync`, `CategoryExistsAsync`.
- **`RepositoryServiceCollectionExtensions.AddNotixRepositories()`**: înregistrează toate repository-urile ca `Scoped`; apelat în `Program.cs`.
- Am șters `Repositories/.gitkeep`.

## Verificare

- `dotnet build Notix.sln`: 0 avertismente, 0 erori.
- Căutare după SQL brut în `backend/src` (fără migrări): niciun `FromSql*`, `ExecuteSql*` sau `SqlQuery`.
- Am rulat un test temporar (șters după, pentru că testele oficiale sunt cele 16 din T-40), pe SQLite in-memory cu migrarea `InitialCreate` aplicată și date de test. A trecut:
  - catalogul exclude materialele `Pending` și cele șterse; filtrele, sortările și paginarea dau rezultatele așteptate;
  - `100%` și `_` sunt căutate literal (`_` găsește doar „Baze_de_date”, nu și „Baze de date curs”); `a%z` → 0 rezultate;
  - `' OR 1=1 --` → 0 rezultate, iar tabelul `Materials` rămâne intact;
  - `GetFeaturedAsync` ordonează după nota medie, iar `GetSimilarAsync` pune aceeași materie înaintea aceleiași categorii;
  - `GetDetailsAsync` încarcă și un material șters;
  - `GetByEmailAsync("  B@X.RO ")` găsește utilizatorul; `HasPurchasedAsync` ignoră comenzile `Pending`;
  - media notelor e `null` pentru un material fără recenzii; coșul, lookups-urile și repository-ul generic (inclusiv cheia compusă) funcționează.

## Diferențe față de plan

1. **Metode în plus față de lista din plan**, necesare taskurilor următoare: `GetFeaturedAsync` (T-14), `GetRatingSummaryAsync` (T-14), `RemoveItemsAsync` (T-21), `GetSubjectAsync` și `CategoryExistsAsync` (validarea din T-18).
2. **`ILookupRepository` nu moștenește `IRepository<T>`**, pentru că lucrează cu patru entități diferite și e doar pentru citire. CRUD-ul din panoul admin (T-25) poate folosi `IRepository<Category>`, `IRepository<University>` etc., înregistrate generic.
3. **`PageSize` limitat la 1–100** în repository, ca un parametru greșit să nu încarce tot tabelul.
4. **Fișierele `.gitkeep`** din `Models/Entities`, `Models/Enums` și `Data/Configurations` sunt încă în repo, deși raportul T-03 spune că au fost șterse. Nu le-am atins în acest task.

## Întrebări / nerezolvate

Niciuna.
