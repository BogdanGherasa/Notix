# Notix

Notix este un site unde studenții cumpără și vând notițe și materiale de studiu în format PDF: cursuri, seminarii, laboratoare, proiecte și subiecte de examen rezolvate, ordonate după universitate, facultate, materie și an de studiu.

## Tehnologii

- **Backend:** ASP.NET Core Web API (.NET 10), Entity Framework Core, SQLite
- **Frontend:** HTML, CSS și JavaScript simplu (fără framework)
- **Plăți:** Stripe Checkout (modul test)
- **Server web:** Apache (XAMPP local, Ubuntu în producție)

## Structura

- `backend/` — soluția .NET (`Notix.Api` + `Notix.Tests`)
- `frontend/` — paginile HTML, CSS și JS
- `deploy/` — configurații pentru Apache și server
- `docs/` — documentația proiectului

Planul de implementare se află în [`PLAN.md`](PLAN.md).
