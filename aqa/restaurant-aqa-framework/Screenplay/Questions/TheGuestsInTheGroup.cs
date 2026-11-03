using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The names of the guests in the owner's group, as the service tells the owner — independent of whatever the
/// scenario's last call was, so a Then step can check that a refused change left the group as it was.</summary>
public sealed class TheGuestsInTheGroup : IQuestion<IReadOnlyList<string>>
{
    public async Task<IReadOnlyList<string>> AnsweredByAsync(Actor actor)
    {
        var owner = actor.Recall<Account>(TheBearerTokenOf.Owner);
        var booking = actor.Recall<GroupBooking>(TheBearerTokenOf.Owner);

        var response = await new GroupPreOrderApi(actor.UsingTheApi()).GetParticipantsAsync(booking.GroupReservationId, owner.IdToken);
        if (!response.Ok)
            throw new InvalidOperationException($"The owner could not see the group: {response.Status}: {await response.TextAsync()}");

        return (await response.ReadAsAsync<GroupParticipantsResponse>()).Participants
            .Where(p => p.Type == ParticipantType.Guest)
            .Select(p => p.Name)
            .ToList();
    }
}
