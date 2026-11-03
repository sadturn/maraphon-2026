namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>GET /guest/groups/{groupReservationId}/sessions (owner only). <paramref name="Total"/> sums every
/// participant, submitted or not — unlike the owner's screen, whose Grand Total counts submitted carts only.</summary>
public sealed record GroupParticipantsResponse(IReadOnlyList<ParticipantResponse> Participants, decimal Total);
