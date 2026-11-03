using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The scenario's group's status as the service reports it: "Open", "Submitted" or "Closed".</summary>
public sealed class TheGroupStatus : IQuestion<string>
{
    public async Task<string> AnsweredByAsync(Actor actor)
    {
        var response = await new GroupPreOrderApi(actor.UsingTheApi()).GetStatusAsync(actor.Recall<GroupBooking>().GroupReservationId);
        if (!response.Ok)
            throw new InvalidOperationException($"The group's status could not be read: {response.Status}: {await response.TextAsync()}");

        return (await response.ReadAsAsync<GroupStatusResponse>()).Status;
    }
}
