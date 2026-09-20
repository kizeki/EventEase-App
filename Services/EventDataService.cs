using System.Net.Http.Json;
using EventEase_App.Models;

namespace EventEase_App.Services;

public sealed class EventDataService(HttpClient httpClient)
{
    private readonly HttpClient httpClient = httpClient;
    private List<Event>? events;
    private List<User>? users;

    public async Task<IReadOnlyList<Event>> GetEventsAsync()
    {
        if (events is not null)
        {
            return events;
        }

        events = await httpClient.GetFromJsonAsync<List<Event>>("data/events.json") ?? [];
        return events;
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync()
    {
        if (users is not null)
        {
            return users;
        }

        users = await httpClient.GetFromJsonAsync<List<User>>("data/users.json") ?? [];
        return users;
    }

    public async Task<EventPage> GetEventsPageAsync(
        int page,
        int pageSize,
        string searchTerm,
        string dateFilter,
        string locationFilter,
        string viewFilter,
        IReadOnlySet<int> registeredEventIds)
    {
        var allEvents = await GetEventsAsync();
        var today = DateTime.Today;
        var nextMonth = today.AddMonths(1);
        var search = searchTerm.Trim();
        var filteredEvents = allEvents
            .Where(eventItem => string.IsNullOrWhiteSpace(search)
                || eventItem.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || eventItem.Location.Contains(search, StringComparison.OrdinalIgnoreCase)
                || eventItem.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                || eventItem.Date.ToString("MMMM d, yyyy").Contains(search, StringComparison.OrdinalIgnoreCase))
            .Where(eventItem => dateFilter switch
            {
                "this-month" => eventItem.Date.Year == today.Year && eventItem.Date.Month == today.Month,
                "next-month" => eventItem.Date.Year == nextMonth.Year && eventItem.Date.Month == nextMonth.Month,
                _ => true
            })
            .Where(eventItem => locationFilter == "all" || eventItem.Location == locationFilter)
            .Where(eventItem => viewFilter != "registered" || registeredEventIds.Contains(eventItem.Id))
            .OrderBy(eventItem => eventItem.Date)
            .ToList();

        var safePage = Math.Max(1, page);
        var safePageSize = Math.Clamp(pageSize, 1, 50);
        return new EventPage
        {
            Items = filteredEvents.Skip((safePage - 1) * safePageSize).Take(safePageSize).ToList(),
            TotalCount = filteredEvents.Count,
            Page = safePage,
            PageSize = safePageSize
        };
    }

    public async Task<IReadOnlyList<string>> GetLocationsAsync()
    {
        return (await GetEventsAsync())
            .Select(eventItem => eventItem.Location)
            .Distinct()
            .OrderBy(location => location)
            .ToList();
    }

}
