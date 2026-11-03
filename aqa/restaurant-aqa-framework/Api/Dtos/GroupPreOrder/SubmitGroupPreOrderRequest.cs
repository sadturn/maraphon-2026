namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>Body of POST /group-reservations/{groupReservationId}/submit: the reservation whose order receives the guests' dishes.</summary>
public sealed record SubmitGroupPreOrderRequest(string ReservationId);
