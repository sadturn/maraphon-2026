namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>GET /guest/groups/{groupReservationId}/status (no sign-in needed). <paramref name="Status"/> is a string on
/// the wire: "Open", "Submitted" or "Closed".</summary>
public sealed record GroupStatusResponse(int CurrentParticipants, int MaxCapacity, string Status);
