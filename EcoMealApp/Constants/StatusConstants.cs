namespace EcoMealApp.Constants;

public static class StatusConstants
{
    public const string Pending = "Pending";
    public const string Accepted = "Accepted";
    public const string Rejected = "Rejected";
    public const string ReadyForPickup = "ReadyForPickup";
    public const string Completed = "Completed";
    
    public static readonly Guid PendingId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AcceptedId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ReadyForPickupId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid CompletedId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid RejectedId = Guid.Parse("55555555-5555-5555-5555-555555555555");
}