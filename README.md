# EventEase-App

EventEase is a Blazor WebAssembly event discovery application. Users can browse events, search and filter the catalog, inspect event details, and register for events from a responsive web interface.

## Live Website

[Open EventEase-App](https://kizeki.github.io/EventEase-App/)

## Features

- Responsive event discovery homepage
- Event cards with images, dates, locations, descriptions, and attendee counts
- Search by event name, location, description, or date
- Filters for date, location, and registered events
- Service-backed pagination with a Load more interaction
- Event detail pages at `/events/{event-id}`
- Custom not-found page for invalid routes and unknown event IDs
- Registration form with name and email validation
- Browser-local registration state that survives refreshes
- Registration across multiple events
- Cancel registration support
- “My events” filtering and registration badges
- Lazy-loaded event images
- xUnit tests for event data filtering, pagination, locations, and caching

## Technology

- .NET 10
- Blazor WebAssembly
- C# and Razor components
- xUnit
- JSON mock data loaded through `HttpClient`
- Browser `localStorage` through JavaScript interop
- GitHub Actions and GitHub Pages

## Project Structure

```text
EventEase-App/
├── EventEase-App.csproj
├── App.razor
├── Program.cs
├── Layout/                  Shared application layout and navigation
├── Models/                  Event, user, and paged-result models
├── Pages/                   Homepage, event details, and not-found pages
├── Services/                JSON data access and registration state
├── wwwroot/
│   ├── data/                events.json and users.json
│   ├── css/                 Application styling
│   └── js/                  Browser local-storage bridge
├── EventEase-App.Tests/     xUnit unit tests
└── .github/workflows/       GitHub Pages deployment workflow
```

## Data Flow

The application reads the complete mock dataset from:

- `wwwroot/data/events.json`: 350 events
- `wwwroot/data/users.json`: 250 users

`EventDataService` caches the JSON collections for the current app session and provides filtering, sorting, location discovery, and paged event results to the homepage.

The JSON files are read-only application data. New registrations are kept separately in browser local storage and layered onto the read-only attendee data at runtime.

## Requirements

- .NET SDK 10.0 or later
- VS Code with C# Dev Kit recommended for debugging

## Run Locally

```powershell
dotnet run --project EventEase-App.csproj
```

The development server displays the local URL in the terminal. You can also open the Run and Debug panel and start the `.NET: Launch EventEase-App (HTTP)` configuration.

## Build

```powershell
dotnet build EventEase-App.csproj --configuration Release
```

## Test

Run the unit tests with:

```powershell
dotnet test EventEase-App.Tests/EventEase-App.Tests.csproj --configuration Release
```

The test suite currently covers:

- Event pagination and total counts
- Search and location filtering
- Registered-event filtering
- Distinct sorted locations
- JSON request caching

## Registration Behavior

Registration stores the user's name, email, and registered event IDs in the browser's `localStorage`. This preserves registration status, selected events, and attendee counts after a refresh in the same browser.

Registration data is local to the browser. It is not shared between browsers or devices, is not written back to the JSON files, and is removed when the browser's site data is cleared.

## Deployment

The workflow in `.github/workflows/deploy-pages.yml` runs when changes are pushed to `main`:

1. Publishes the WebAssembly project in Release mode.
2. Configures the repository base path for GitHub Pages.
3. Creates the SPA `404.html` fallback and `.nojekyll` file.
4. Uploads the generated static files.
5. Deploys the site through GitHub Pages.

The deployment URL is:

https://kizeki.github.io/EventEase-App/

## Current Limitations

- Pagination is applied after the JSON collection is downloaded. True server-side pagination would require an API and database.
- Registrations are browser-local and are not shared between users.
- Event images, avatars, and fonts use external services and require network access.
- The application currently has no authentication or server-side duplicate-email enforcement.
