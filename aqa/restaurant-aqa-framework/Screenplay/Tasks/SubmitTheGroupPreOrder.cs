using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Which group and booking a submission names: the scenario's own, or one of them made up.</summary>
public enum SubmissionTarget
{
    TheScenariosGroup,
    AGroupThatDoesNotExist,
    ABookingThatDoesNotExist,
}

/// <summary>Submits the group pre-order through the service as <paramref name="viewer"/>. The outcome is the scenario's
/// last response; a set-up that needs the submission to have happened passes <paramref name="mustSucceed"/>.</summary>
public sealed class SubmitTheGroupPreOrder(
    Viewer viewer, SubmissionTarget target = SubmissionTarget.TheScenariosGroup, bool mustSucceed = false) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new MeetTheViewer(viewer));

        var booking = actor.Recall<GroupBooking>();
        var groupId = target == SubmissionTarget.AGroupThatDoesNotExist ? Guid.NewGuid().ToString() : booking.GroupReservationId;
        var reservationId = target == SubmissionTarget.ABookingThatDoesNotExist ? Guid.NewGuid().ToString() : booking.ReservationId;
        var token = await actor.AsksForAsync(new TheBearerTokenOf(viewer));
        var response = actor.TheLastResponse();

        await response.CaptureAsync(await new GroupPreOrderApi(actor.UsingTheApi()).SubmitAsync(groupId, reservationId, token));
        if (mustSucceed)
            response.ThrowUnlessSucceeded("Submitting the group pre-order");
    }
}
