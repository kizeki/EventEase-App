using System.Net;
using System.Net.Http.Json;
using EventEase_App.Models;
using EventEase_App.Services;

namespace EventEase_App.Tests;

public sealed class EventDataServiceTests
{
    [Fact]
    public async Task GetEventsPageAsync_ReturnsRequestedPageAndTotalCount()
    {
        var service = CreateService();

        var result = await service.GetEventsPageAsync(
            page: 2,
            pageSize: 2,
            searchTerm: string.Empty,
            dateFilter: "all",
            locationFilter: "all",
            viewFilter: "all",
            registeredEventIds: new HashSet<int>());

        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.False(result.HasMore);
        Assert.Equal([3, 4], result.Items.Select(eventItem => eventItem.Id));
    }

    [Fact]
    public async Task GetEventsPageAsync_FiltersBySearchAndLocation()
    {
        var service = CreateService();

        var result = await service.GetEventsPageAsync(
            page: 1,
            pageSize: 12,
            searchTerm: "studio",
            dateFilter: "all",
            locationFilter: "Studio North, Brooklyn",
            viewFilter: "all",
            registeredEventIds: new HashSet<int>());

        var eventItem = Assert.Single(result.Items);
        Assert.Equal(4, eventItem.Id);
        Assert.Equal("Studio North", eventItem.Name);
    }

    [Fact]
    public async Task GetEventsPageAsync_FiltersRegisteredEvents()
    {
        var service = CreateService();

        var result = await service.GetEventsPageAsync(
            page: 1,
            pageSize: 12,
            searchTerm: string.Empty,
            dateFilter: "all",
            locationFilter: "all",
            viewFilter: "registered",
            registeredEventIds: new HashSet<int> { 2, 4 });

        Assert.Equal([2, 4], result.Items.Select(eventItem => eventItem.Id));
    }

    [Fact]
    public async Task GetLocationsAsync_ReturnsDistinctSortedLocations()
    {
        var service = CreateService();

        var locations = await service.GetLocationsAsync();

        Assert.Equal(
            ["Innovation Hall, Austin", "Pier 7, Seattle", "Studio North, Brooklyn", "The Glasshouse, Portland"],
            locations);
    }

    [Fact]
    public async Task GetUsersAsync_CachesTheJsonRequest()
    {
        var handler = new CountingJsonHandler();
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://tests.local/") };
        var service = new EventDataService(httpClient);

        var firstResult = await service.GetUsersAsync();
        var secondResult = await service.GetUsersAsync();

        Assert.Same(firstResult, secondResult);
        Assert.Equal(1, handler.GetRequestCount("data/users.json"));
    }

    private static EventDataService CreateService()
    {
        var handler = new CountingJsonHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://tests.local/")
        };
        return new EventDataService(httpClient);
    }

    private sealed class CountingJsonHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, int> requestCounts = new(StringComparer.OrdinalIgnoreCase);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath.TrimStart('/') ?? string.Empty;
            requestCounts[path] = GetRequestCount(path) + 1;

            var content = path switch
            {
                "data/events.json" => TestData.EventsJson,
                "data/users.json" => TestData.UsersJson,
                _ => "[]"
            };

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            });
        }

        public int GetRequestCount(string path)
        {
            return requestCounts.GetValueOrDefault(path);
        }
    }

    private static class TestData
    {
        public const string EventsJson = """
        [
          { "id": 1, "name": "Harbor Lights", "description": "Music by the water", "date": "2026-10-03T18:30:00", "location": "Pier 7, Seattle", "imageUrl": "https://example.com/1.jpg", "attendees": [1] },
          { "id": 2, "name": "Future Makers", "description": "A technology summit", "date": "2026-10-17T09:00:00", "location": "Innovation Hall, Austin", "imageUrl": "https://example.com/2.jpg", "attendees": [1, 2] },
          { "id": 3, "name": "Autumn Table", "description": "A seasonal dinner", "date": "2026-11-06T19:00:00", "location": "The Glasshouse, Portland", "imageUrl": "https://example.com/3.jpg", "attendees": [] },
          { "id": 4, "name": "Studio North", "description": "A design workshop", "date": "2026-11-21T10:30:00", "location": "Studio North, Brooklyn", "imageUrl": "https://example.com/4.jpg", "attendees": [2, 3] }
        ]
        """;

        public const string UsersJson = """
        [
          { "id": 1, "name": "Maya Chen", "email": "maya@example.com", "avatarUrl": "https://example.com/maya.jpg" },
          { "id": 2, "name": "Jordan Ellis", "email": "jordan@example.com", "avatarUrl": "https://example.com/jordan.jpg" },
          { "id": 3, "name": "Avery Morgan", "email": "avery@example.com", "avatarUrl": "https://example.com/avery.jpg" }
        ]
        """;
    }
}