namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>GET /reservations/{reservationId}/guest-session-id — the owner's own session in the group, created by
/// the backend the first time it is asked for (the owner's screen asks as soon as it opens).</summary>
public sealed record OwnerSessionResponse(string GuestSessionId);
