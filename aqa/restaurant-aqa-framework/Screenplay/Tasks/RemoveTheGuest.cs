using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Removes a guest from the owner's group pre-order through the service, as <paramref name="viewer"/>. The guest
/// is the one remembered as <paramref name="guestName"/> — possibly a guest of another group — or, without a name, a guest
/// who does not exist. The outcome is the scenario's last response: on success, the group's updated participants.</summary>
public sealed class RemoveTheGuest(Viewer viewer, string? guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new MeetTheViewer(viewer));

        // Always the owner's group: a scenario with a second group has made that one the current booking.
        var groupId = actor.Recall<GroupBooking>(TheBearerTokenOf.Owner).GroupReservationId;
        var guestSessionId = guestName is null ? Guid.NewGuid().ToString() : actor.Recall<GuestInGroup>(guestName).SessionId;
        var token = await actor.AsksForAsync(new TheBearerTokenOf(viewer));

        await actor.TheLastResponse().CaptureAsync(
            await new GroupPreOrderApi(actor.UsingTheApi()).RemoveGuestAsync(groupId, guestSessionId, token));
    }
}
