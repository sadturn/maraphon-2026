using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Dishes;
using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>A guest puts dishes in their cart, or takes them out. The dishes are the ones the scenario put on the menu
/// under those names.</summary>
public sealed class ChangeTheCart : ITask
{
    private readonly string _guestName;
    private readonly IReadOnlyList<CartLine> _lines;
    private readonly bool _takeOut;

    private ChangeTheCart(string guestName, IReadOnlyList<CartLine> lines, bool takeOut)
    {
        _guestName = guestName;
        _lines = lines;
        _takeOut = takeOut;
    }

    public static ChangeTheCart PutIn(string guestName, IReadOnlyList<CartLine> lines) => new(guestName, lines, takeOut: false);

    public static ChangeTheCart TakeOut(string guestName, IReadOnlyList<CartLine> lines) => new(guestName, lines, takeOut: true);

    public async Task PerformAsAsync(Actor actor)
    {
        var guest = actor.Recall<GuestInGroup>(_guestName);
        var api = new GroupPreOrderApi(actor.UsingTheApi());
        var response = actor.TheLastResponse();

        foreach (var line in _lines)
        {
            var item = new GuestCartItemRequest(actor.Recall<DishResponse>(line.Dish).Id, line.Quantity);
            await response.CaptureAsync(_takeOut
                ? await api.RemoveFromCartAsync(guest.SessionId, item, guest.SessionToken)
                : await api.AddToCartAsync(guest.SessionId, item, guest.SessionToken));
            response.ThrowUnlessSucceeded($"{_guestName} {(_takeOut ? "taking out" : "putting in")} {line}");
        }
    }
}
