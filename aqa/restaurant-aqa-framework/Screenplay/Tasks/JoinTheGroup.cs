using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>A guest follows the invitation and joins the group pre-order under <paramref name="guestName"/>; the guest is
/// remembered under that name. Nothing to remove afterwards: cancelling the reservation closes the group.</summary>
public sealed class JoinTheGroup(string guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var booking = actor.Recall<GroupBooking>();
        var response = actor.TheLastResponse();

        await response.CaptureAsync(await new GroupPreOrderApi(actor.UsingTheApi())
            .JoinAsync(new JoinGroupRequest(booking.GroupReservationId, guestName)));
        response.ThrowUnlessSucceeded($"{guestName} joining the group");

        var session = response.Json<GuestSessionResponse>();
        actor.Remember(guestName, new GuestInGroup(guestName, session.SessionId, session.SessionToken));
    }
}
