namespace Restaurant_Aqa_Framework.Screenplay;

/// <summary>A reservation for several guests and the group pre-order that comes with it.</summary>
public sealed record GroupBooking(string ReservationId, string GroupReservationId);
