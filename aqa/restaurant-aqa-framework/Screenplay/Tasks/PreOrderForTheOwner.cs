using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Dishes;
using Restaurant_Aqa_Framework.Api.Dtos.Orders;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>The owner pre-orders their own dishes for the booking. That order is also the one the restaurant receives
/// when the group pre-order is submitted: the guests' dishes are added to it. Nothing to remove afterwards: cancelling
/// the reservation takes the order with it.</summary>
public sealed class PreOrderForTheOwner(IReadOnlyList<CartLine> lines) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var owner = actor.Recall<Account>(TheBearerTokenOf.Owner);
        var booking = actor.Recall<GroupBooking>();
        var items = lines.Select(l => new OrderItemRequest(actor.Recall<DishResponse>(l.Dish).Id, l.Quantity)).ToList();
        var response = actor.TheLastResponse();

        await response.CaptureAsync(await new OrdersApi(actor.UsingTheApi())
            .CreateOrderAsync(new CreateOrderRequest(booking.ReservationId, items), owner.IdToken));
        response.ThrowUnlessSucceeded("The owner's own pre-order");
    }
}
