namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>Body of both POST and DELETE /guest/sessions/{guestSessionId}/dishes: the quantity is added or taken away.</summary>
public sealed record GuestCartItemRequest(string DishId, int Quantity);
