namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>GET /reservations/{reservationId}/invite-token — the token a guest's invitation link carries.</summary>
public sealed record InviteTokenResponse(string Token);
