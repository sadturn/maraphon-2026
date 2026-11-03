namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>GET /guest/invites/{token} — what an invitation link opens. It is the only route that turns a
/// reservation into its group: every other group route is keyed by <paramref name="GroupReservationId"/>.</summary>
public sealed record GroupInviteResponse(
    string GroupReservationId,
    string LocationId,
    string RestaurantName,
    string LocationAddress,
    DateTime StartTime,
    DateTime EndTime,
    int MaxGuestCount,
    int TableNumber,
    string InviterName);
