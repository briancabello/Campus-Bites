# Contributing

[![README](https://img.shields.io/badge/README-e5e7eb?style=for-the-badge&logoColor=333333)](./README.md)
[![Contributing](https://img.shields.io/badge/Contributing-111827?style=for-the-badge)](./CONTRIBUTING.md)

Campus Bites was built as an NBCC term project by a two-person team under the name **MVCode**.

<img src="src/mvc2025TermProject/wwwroot/img/MVCode Logo.png" alt="MVCode Logo" width="200">

## Team & Roles

| Name | GitHub | Role |
| --- | --- | --- |
| **Luc Langis** | [@llangis](https://github.com/llangis) | Full-Stack Lead & Client Liaison |
| **Brian Cabello** | [@briancabello](https://github.com/briancabello) | Front-End Lead & UX Designer |

## Working in This Repository

This is a two-person academic project, not an actively maintained open-source repository, so the guidance below reflects how the project was actually built rather than a formal open-source process.

- **Branching:** development happened directly on `master`. There is no enforced feature-branch or pull-request workflow in this repository's history.
- **Commit messages:** recent history loosely follows a `type: summary` style (`feat:`, `fix:`, `chore:`), but this was not strictly enforced. If you add commits, prefer a short, descriptive summary in that style.
- **Solution layout:** the buildable code lives under `src/` (`mvc2025TermProject`, the MVC app, and `EmailService`, a small class library it depends on). Open `src/mvc2025TermProject.sln` to work in Visual Studio, or use the `dotnet` CLI from `src/mvc2025TermProject`.
- **Database changes:** schema changes go through EF Core Code First migrations (`Data/Migrations`). Add a migration with `dotnet ef migrations add <Name>` from `src/mvc2025TermProject`, then apply it with `dotnet ef database update`. Do not hand-edit generated migration files.
- **Content model:** `Recipe`, `Category`, and `Ingredient` share the same approval pattern (`IsApproved` / `IsPendingModification`, or `Status` for recipes). If you extend one of these models, keep the existing review states consistent rather than introducing a parallel status scheme.
- **Secrets:** `appsettings.json` contains local development configuration, including SMTP settings for a local mail catcher. Do not commit real credentials or a production connection string to this file.

## Reporting Issues

This project does not have an issue tracker. If you find a problem, note it directly to one of the contributors above.
