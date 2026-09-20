# EventEase-App

A Blazor WebAssembly application deployed to GitHub Pages.

## Live Website

[Open EventEase-App](https://kizeki.github.io/EventEase-App/)

## Requirements

- .NET SDK 10.0 or later

## Run Locally

```powershell
dotnet run
```

The development server displays the local URL in the terminal.

## Build

```powershell
dotnet build EventEase-App.csproj --configuration Release
```

The event and user JSON files contain the complete mock dataset: 350 events and 250 users. The data service reads these files and applies filtering and pagination through its data-access methods.

## Deployment

The GitHub Actions workflow in `.github/workflows/deploy-pages.yml` publishes the WebAssembly app and deploys it to GitHub Pages whenever changes are pushed to `main`.

The deployment URL is:

https://kizeki.github.io/EventEase-App/

## Registration Behavior

Event registrations are stored in the browser's local storage. This preserves the registered user, selected events, attendee counts, and registration status after a refresh in the same browser.

The app is a static GitHub Pages deployment, so registrations are not shared between browsers or devices and are cleared when the browser's site data is removed.
