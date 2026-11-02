using System.ComponentModel.DataAnnotations;

namespace APIHandler.Models.GuestSessions;

public record SubmitGroupPreOrderRequest(
    [property: Required] string ReservationId);

public record GroupPreOrderItemResponse(string DishId, int Quantity);
