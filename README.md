# BlazorApp

**********------------**********-------------***********------------


# BlazorApp (Client + Application + Domain)

BlazorApp is a .NET 8 Blazor (Razor Components) application that demonstrates a clean, layered architecture for Customer Management with localization, theming, and a simple API client service.

## Features
- Customer Management (list, view details, create, edit)
- Localization with `IStringLocalizer<SharedResource>` and `.resx` resources
- Theme toggle persisted via cookie
- Language switch persisted via cookie
- Clean separation into `Client`, `Application`, and `Domain` projects
- Visual Studio 2022 friendly

## Project Structure
- BlazorApp.Client (UI)
  - Components
    - Layout: `MainLayout`, `NavMenu`
    - Pages/Customers: `CustomerList`, `CustomerCreate`, `CustomerEdit`
    - Shared: `LanguageSwitcher`, `ThemeToggle` (referenced by layout)
  - Resources: `SharedResource.resx` (+ per-culture resx)
  - Services: `CustomerService` (HttpClient-based)
  - `Program.cs`: Razor Components + Localization + cookies + routes
  - `SharedResource.cs`: marker type for shared localization
- BlazorApp.Application (Use cases)
  - MediatR-based commands (e.g., `CreateCustomer`, `UpdateCustomer`)
- BlazorApp.Domain (Core)
  - Entities, value objects (domain-centric)

## Quickstart
1. Prerequisites: .NET 8 SDK, Visual Studio 2022
2. Open the solution in VS.
3. Set `BlazorApp.Client` as startup project.
4. Configure API base URL in `appsettings.*` or update `Program.cs`:
   - `ApiBaseUrl` (defaults to `https://localhost:7043`)
5. Run with __Debug > Start Debugging__.

## Internationalization (i18n)
- Localizer: `IStringLocalizer<SharedResource>`
- Resource files placed at project root or under `Resources` (see docs)
- Culture is resolved via cookie, query string, or Accept-Language
- Endpoints:
  - `/set-culture?culture=<code>&redirectUri=<path>`
  - `/set-theme?theme=<name>&redirectUri=<path>`

See docs/LOCALIZATION.md for a step-by-step guide.

## Documentation
- docs/ARCHITECTURE.md
- docs/LOCALIZATION.md
- docs/USE-CASES.md
- docs/HELP.md

## Troubleshooting (short)
- Keys show instead of values: check resource path, base name, and build action = Embedded resource. See docs/LOCALIZATION.md.
- Caret appears everywhere: ensure no `contenteditable` or `document.designMode = "on"`. See docs/HELP.md.


---------**********------------**********-------------***********------------**********------------**********-------------***********------------

# Architecture Overview



## Layers
- Domain (Core)
  - Defines entities/value objects and business concepts.
- Application (Use Cases)
  - Coordinates domain operations via commands/handlers (MediatR).
  - Example: `CreateCustomer`, `UpdateCustomer` commands.
- Client (Presentation, Blazor)
  - UI built with .NET 8 Razor Components, Interactive Server render mode.
  - Uses `CustomerService` (HttpClient) to talk to the API.
  - Provides localization (`IStringLocalizer<SharedResource>`) and theming.

## Client Composition
- Layout
  - `MainLayout.razor`: header, language/theme actions, sidebar, main container.
  - `NavMenu.razor`: navigation to pages, localized labels.
- Pages (Customers)
  - `CustomerList.razor`: list/search customers, view details modal.
  - `CustomerCreate.razor`: create form + validation.
  - `CustomerEdit.razor`: edit form + validation.
- Shared
  - `LanguageSwitcher`: switches UI culture via `/set-culture`.
  - `ThemeToggle`: switches theme via `/set-theme`.
- Services
  - `CustomerService`: wraps `HttpClient` (base address from `ApiBaseUrl`) for customer endpoints.

## Cross-Cutting
- Localization
  - `SharedResource` marker type + `.resx` files.
  - `Program.cs` adds localization, configures `RequestLocalizationOptions`.
- State
  - `ThemeState` (scoped) to propagate theme preference.
- Cookies/Endpoints
  - `/set-culture` writes `.AspNetCore.Culture` cookie.
  - `/set-theme` writes `.Theme` cookie.

## Request Flow (UI)
1. User navigates to a page (e.g., `/customers/create`).
2. Razor Component renders; `IStringLocalizer<SharedResource>` supplies localized strings.
3. On actions (submit), component calls `CustomerService` -> API.
4. Results/errors are reflected in the UI; validation uses DataAnnotations.

## Build/Runtime
- Target framework: `.NET 8`
- C# language version: 12
- Runs under Visual Studio 2022.


---------**********------------**********-------------***********------------**********------------**********-------------***********------------


# Localization Guide



This app uses `IStringLocalizer<SharedResource>` and `.resx` files.

## File Placement
Choose one approach and keep it consistent:

A) Resources folder (recommended)
- `BlazorApp.Client/Resources/SharedResource.resx` (neutral)
- `BlazorApp.Client/Resources/SharedResource.fr-FR.resx`
- `BlazorApp.Client/Resources/SharedResource.hi-IN.resx`
- In `Program.cs`: `builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");`

B) Root folder
- `BlazorApp.Client/SharedResource.resx` (neutral)
- `BlazorApp.Client/SharedResource.fr-FR.resx`
- `BlazorApp.Client/SharedResource.hi-IN.resx`
- In `Program.cs`: `builder.Services.AddLocalization();` (no `ResourcesPath`)

Marker type must be at `BlazorApp.Client.SharedResource`:
- `BlazorApp.Client/SharedResource.cs`

Set Build Action of `.resx` files: Embedded resource.

## Injecting in Components
- In `_Imports.razor`: `@using BlazorApp.Client`
- In components: `@inject IStringLocalizer<SharedResource> L`

Usage:
- `@L["Customers"]`
- `@L["CreateCustomer_Title"]`

## Culture Resolution
Configured providers (in priority order):
1. Cookie (`.AspNetCore.Culture`)
2. Query string (`?culture=fr-FR`)
3. Accept-Language header

Default culture: `en-US`.

## Switching Language
- Endpoint: `/set-culture?culture=fr-FR&redirectUri=/`
  - Writes `.AspNetCore.Culture` cookie and redirects.
- Ensure `app.UseRequestLocalization(locOptions);` runs early in the pipeline.

## Verification
- Clean + Rebuild the solution.
- Check `bin/Debug/net8.0/<culture>/BlazorApp.Client.resources.dll`.
- Add a diagnostics page to print `CultureInfo.CurrentUICulture.Name` and a sample key.
- If keys are shown:
  - Confirm base name matches `SharedResource` location.
  - Confirm `.resx` Build Action = Embedded resource.
  - Confirm the folder path matches configuration.

---------**********------------**********-------------***********------------**********------------**********-------------***********------------


# Use Cases



## U1. Browse Customers
- As a user, I want to view a list of customers to quickly scan available records.
- Steps
  - Navigate to `/`
  - See the list, search field, and actions.
- Success
  - List renders, search filters work, details modal opens with correct info.

## U2. View Customer Details
- As a user, I want to view a customer’s details in a modal to avoid leaving the list.
- Steps
  - Click a “View Details” action on a customer.
- Success
  - Modal shows name, email, phone, address, created/updated dates.

## U3. Create Customer
- As a user, I want to add a new customer via a form with validation.
- Steps
  - Navigate to `/customers/create`, fill the form, submit.
- Success
  - Validations show for missing/invalid fields, customer is persisted, navigate or show success.

## U4. Edit Customer
- As a user, I want to edit an existing customer.
- Steps
  - Navigate to `/customers/edit/{id}`, adjust fields, submit.
- Success
  - Changes are persisted, navigate back with confirmation.

## U5. Switch Language
- As a user, I want to switch the UI language and keep my choice.
- Steps
  - Use language switcher (invokes `/set-culture`), page reloads.
- Success
  - UI labels translate; cookie `.AspNetCore.Culture` is set.

## U6. Switch Theme
- As a user, I want to toggle between light/dark themes and keep my choice.
- Steps
  - Use theme toggle (invokes `/set-theme`), UI updates.
- Success
  - Theme persists via `.Theme` cookie.

## Sequences


---------**********------------**********-------------***********------------**********------------**********-------------***********------------



# Help & Troubleshooting


## Common Issues


### Keys show instead of translated text
- Ensure `.resx` location matches `AddLocalization` configuration.
- Build Action = Embedded resource.
- Marker type: `BlazorApp.Client.SharedResource`.
- Clean + Rebuild.
- Verify satellite assemblies in `bin/Debug/net8.0/<culture>/`.

### Caret appears everywhere (blinking cursor on headings)
- Check no element has `contenteditable="true"`.
- Ensure no script sets `document.designMode = "on"`.
- Quick guard: add `contenteditable="false"` to layout root while investigating.

### Language doesn’t change
- Confirm cookie `.AspNetCore.Culture` is written after hitting `/set-culture?culture=fr-FR`.
- Ensure `app.UseRequestLocalization(locOptions);` is registered early in the pipeline.
- Temporarily force default culture in `RequestLocalizationOptions` to test.

### Visual Studio tips
- Clean: __Build > Clean Solution__
- Rebuild: __Build > Rebuild Solution__
- Run/Debug: __Debug > Start Debugging__
- Search in files: __Edit > Find and Replace > Find in Files__

## Getting Help
- Open an issue with:
  - Steps to reproduce
  - Current behavior vs expected
  - Logs/screenshots
  - Culture selected and the `.resx` placement screenshot

---------**********------------**********-------------***********------------**********------------**********-------------***********------------
