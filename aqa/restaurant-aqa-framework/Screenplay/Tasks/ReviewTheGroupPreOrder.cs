using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Asks the service for the group pre-order of the scenario's booking (or of one that does not exist) as
/// <paramref name="viewer"/>. The outcome is the scenario's last response, refusals included.</summary>
public sealed class ReviewTheGroupPreOrder(Viewer viewer, bool ofABookingThatDoesNotExist = false) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new MeetTheViewer(viewer));

        var groupId = ofABookingThatDoesNotExist ? Guid.NewGuid().ToString() : actor.Recall<GroupBooking>().GroupReservationId;
        var token = await actor.AsksForAsync(new TheBearerTokenOf(viewer));

        await actor.TheLastResponse().CaptureAsync(
            await new GroupPreOrderApi(actor.UsingTheApi()).GetParticipantsAsync(groupId, token));
    }
}
