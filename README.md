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

## Deployment

The GitHub Actions workflow in `.github/workflows/deploy-pages.yml` publishes the WebAssembly app and deploys it to GitHub Pages whenever changes are pushed to `main`.

The deployment URL is:

https://kizeki.github.io/EventEase-App/
