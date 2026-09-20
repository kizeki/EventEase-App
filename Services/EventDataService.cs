using System.Net.Http.Json;
using EventEase_App.Models;

namespace EventEase_App.Services;

public sealed class EventDataService(HttpClient httpClient)
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<IReadOnlyList<Event>> GetEventsAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Event>>("data/events.json") ?? [];
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync()
    {
        return await httpClient.GetFromJsonAsync<List<User>>("data/users.json") ?? [];
    }
}
