# Campus Bites Recipe Web Application

[![README](https://img.shields.io/badge/README-111827?style=for-the-badge)](./README.md)
[![Contributing](https://img.shields.io/badge/Contributing-e5e7eb?style=for-the-badge&logoColor=333333)](./CONTRIBUTING.md)

<img src="src/mvc2025TermProject/wwwroot/img/Campus Bites Logo.png" alt="Campus Bites Logo" width="200">

A student recipe web application built with ASP.NET Core MVC. Students can discover, create, and share affordable, easy-to-cook meals, with a content moderation system so that user-submitted images and ingredients are reviewed before they reach the public library.

**Catch phrase:** "Easy Meals for Student Life"

## Overview

Campus Bites lets registered students create recipes, propose ingredients, and upload photos. New content does not go public immediately: recipe images and newly proposed ingredients are held in a pending state until an administrator reviews them. Categories go through the same review model. The goal is to keep the recipe library open to student contribution without letting unverified content reach public visitors.

This is an NBCC term project, developed collaboratively (see [CONTRIBUTING.md](./CONTRIBUTING.md) for the team and roles).

## Tech Stack

| Layer | Tech |
| --- | --- |
| Framework | ASP.NET Core 8.0 (MVC) |
| Language | C# (.NET 8) |
| ORM | Entity Framework Core 8.0.23 (Code First, with lazy-loading proxies) |
| Database | SQL Server (local instance, Windows/trusted authentication) |
| Auth | ASP.NET Core Identity, role-based (`User`, `Administrator`) |
| Email | NETCore.MailKit (MailKit/MimeKit) via a local `EmailService` project |
| Front-end | Razor views, Bootstrap 5, jQuery + jQuery Validation |

## Project Structure

```
Campus Bites/
├── ERD/                        Entity relationship diagram (Visual Paradigm)
├── SQL/                        Reference and sample data scripts
├── src/
│   ├── EmailService/           Small class library wrapping MailKit for sending email
│   └── mvc2025TermProject/     The ASP.NET Core MVC application
│       ├── Areas/Identity/     Scaffolded ASP.NET Core Identity UI (register, login, manage account)
│       ├── Controllers/        Public, User, and Admin controllers (see below)
│       ├── Data/                ApplicationDbContext, Identity user model, EF Core migrations, role seeding
│       ├── Helpers/             ImageFileHelper (moving approved images between folders)
│       ├── Models/              Recipe, Ingredient, Category, Image, Contact, and related view models
│       ├── Views/               Razor views, one folder per controller area
│       └── wwwroot/             Static assets: css, js, lib (Bootstrap, jQuery), images
└── mvc2025TermProject.sln
```

Controllers are split by audience: `BrowseController`, `HomeController`, and `Contact(s)Controller` are public-facing; `RecipesController`, `CategoriesController`, `IngredientsController`, and `ImagesController` under `Controllers/User` require a signed-in `User` or `Administrator`; and `CategoriesAdminController`, `IngredientsAdminController`, and `ImagesAdminController` under `Controllers/Admin` are restricted to `Administrator` and handle the review queues.

## Core Features

Grounded in what the controllers and models actually implement:

- **Recipe lifecycle.** A recipe starts as `Initial`, moves to `Draft` once it has at least one ingredient, and can be set to `Draft`, `Public`, `Private`, or `Unlisted`. Publishing to `Public` is blocked server-side unless the recipe has an approved main image, an approved category, and every attached ingredient is approved.
- **Ingredient and category review.** Both ingredients and categories carry an `IsApproved` / `IsPendingModification` pair, which resolves to one of four states: Pending Approval, Approved, Pending Update, or Rejected. Administrators review these from dedicated admin list pages with search, sort, and status filtering.
- **Image moderation.** Uploaded recipe images live in a temp location until an administrator approves them, at which point the file is physically moved into the public images folder and marked approved; rejected images are deleted. Administrators can also set which image is the recipe's main image.
- **Role-based access.** ASP.NET Core Identity with two seeded roles, `User` and `Administrator`. Recipe visibility is enforced per status: `Public` recipes are visible to everyone, `Draft`/`Initial`/`Private` are visible only to the owner (or an admin), and `Unlisted` is visible to anyone with the link.
- **Browse and search.** Keyword, category, and date-range filtering across public recipes, with pagination, plus a paginated "My Recipes" view for a signed-in user's own content.
- **Recipe sharing and reporting.** A visitor can email a public recipe to someone else, or report a recipe with a reason; reports are stored as a `Contact` record and emailed to the project's admin address.
- **Account email confirmation.** Identity is configured with `RequireConfirmedAccount = true`, so new registrations require confirming an email address before signing in, plus standard Identity account-management pages (password reset, 2FA scaffolding, personal data).

## Database Setup

The application expects a local SQL Server instance reachable with Windows/trusted authentication. The active connection string (`DefaultConnection` in `appsettings.json`) is:

```
Server=(local); Database=campus-bites; Trusted_Connection=True; MultipleActiveResultSets=true; Encrypt=false
```

Adjust `Server` if your local SQL Server instance has a different name (for example, a named instance or `(localdb)\mssqllocaldb`).

Schema is managed with EF Core Code First migrations in `src/mvc2025TermProject/Data/Migrations`. From `src/mvc2025TermProject`:

```bash
dotnet ef database update
```

This creates the Identity schema plus the application tables (Recipes, Categories, Ingredients, RecipeIngredients, Images, Contacts). On first run, the app also seeds the `User` and `Administrator` roles (see `Data/ContextSeed.cs`); it does not seed sample recipes or users. Reference SQL and sample data scripts for manual seeding are available under `/SQL`.

## Getting Started

Prerequisites: .NET 8 SDK, a local SQL Server instance.

```bash
cd src/mvc2025TermProject
dotnet restore
dotnet ef database update
dotnet run
```

Email-dependent features (recipe sharing, recipe reporting, and Identity's account confirmation emails) read SMTP settings from the `EmailConfiguration` section of `appsettings.json`. For local development, point these at a local SMTP catcher (for example a tool like smtp4dev or Papercut) rather than a real mail server.

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md) for the team, individual roles, and how to work in this repository.
