namespace EventEase_App.Models;

public sealed class EventPage
{
    public IReadOnlyList<Event> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public bool HasMore => Page * PageSize < TotalCount;
}
