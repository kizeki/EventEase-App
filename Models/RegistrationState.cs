namespace EventEase_App.Models;

public sealed class RegistrationState
{
    public User? User { get; set; }
    public HashSet<int> RegisteredEventIds { get; set; } = [];
}
