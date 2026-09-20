using System.Text.Json;
using Microsoft.JSInterop;
using EventEase_App.Models;

namespace EventEase_App.Services;

public sealed class RegistrationStateService(IJSRuntime jsRuntime)
{
    private const string StorageKey = "eventease.registration";
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);
    private RegistrationState state = new();
    private bool isLoaded;

    public RegistrationState State => state;

    public async Task LoadAsync()
    {
        if (isLoaded)
        {
            return;
        }

        var storedValue = await jsRuntime.InvokeAsync<string?>("eventEaseStorage.get", StorageKey);
        if (!string.IsNullOrWhiteSpace(storedValue))
        {
            try
            {
                state = JsonSerializer.Deserialize<RegistrationState>(storedValue, jsonOptions) ?? new();
                state.RegisteredEventIds ??= [];
            }
            catch (JsonException)
            {
                await ClearAsync();
            }
        }

        isLoaded = true;
    }

    public bool IsRegistered(int eventId)
    {
        return state.RegisteredEventIds.Contains(eventId);
    }

    public async Task RegisterAsync(User user, int eventId)
    {
        await LoadAsync();
        state.User = user;
        state.RegisteredEventIds.Add(eventId);
        await SaveAsync();
    }

    public async Task UnregisterAsync(int eventId)
    {
        await LoadAsync();
        state.RegisteredEventIds.Remove(eventId);
        await SaveAsync();
    }

    public async Task ClearAsync()
    {
        state = new();
        await jsRuntime.InvokeVoidAsync("eventEaseStorage.remove", StorageKey);
    }

    private async Task SaveAsync()
    {
        var serializedState = JsonSerializer.Serialize(state, jsonOptions);
        await jsRuntime.InvokeVoidAsync("eventEaseStorage.set", StorageKey, serializedState);
    }
}
