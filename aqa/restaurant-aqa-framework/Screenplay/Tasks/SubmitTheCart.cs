using Restaurant_Aqa_Framework.Api;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>A guest confirms their cart: from then on it counts towards the group's order and can no longer change.</summary>
public sealed class SubmitTheCart(string guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var guest = actor.Recall<GuestInGroup>(guestName);
        var response = actor.TheLastResponse();

        await response.CaptureAsync(
            await new GroupPreOrderApi(actor.UsingTheApi()).SubmitCartAsync(guest.SessionId, guest.SessionToken));
        response.ThrowUnlessSucceeded($"{guestName} submitting the cart");
    }
}
