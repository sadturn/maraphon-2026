using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Orders;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>A booking's order as the restaurant has it. <see cref="Status"/> is only known when it was read from the service.</summary>
public sealed record RestaurantOrder(IReadOnlyList<CartLine> Dishes, OrderStatus? Status, string Evidence);

/// <summary>The scenario's booking's order, asked for by the waiter assigned to it — the restaurant's side of the
/// group pre-order. Asks through the service, so a `@ui` scenario can check it too.</summary>
public sealed class TheRestaurantsOrder : IQuestion<RestaurantOrder>
{
    public async Task<RestaurantOrder> AnsweredByAsync(Actor actor)
    {
        var waiter = actor.Recall<Account>(HireAWaiter.Who);
        var booking = actor.Recall<GroupBooking>();

        var response = await new OrdersApi(actor.UsingTheApi()).GetOrderAsync(booking.ReservationId, waiter.IdToken);
        var body = await response.TextAsync();
        if (!response.Ok)
            throw new InvalidOperationException($"The waiter could not open the booking's order: {response.Status}: {body}");

        var order = await response.ReadAsAsync<GetOrderResponse>();
        var dishes = order.OrderItems
            .Select(i => new CartLine(SampleDataFactory.WithoutPrefix(i.Dish.Name), i.Quantity))
            .ToList();
        return new RestaurantOrder(dishes, order.Status, body);
    }
}
