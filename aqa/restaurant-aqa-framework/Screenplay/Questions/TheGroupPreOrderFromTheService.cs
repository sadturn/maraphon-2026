using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The group pre-order as the service reported it in the scenario's last response. The service's grand total
/// sums every participant, submitted or not. A refusal is told apart by its status: 401 when the caller is not signed in,
/// 403 when they are but not the owner, 404 when the group does not exist.</summary>
public sealed class TheGroupPreOrderFromTheService : IQuestion<GroupPreOrderView>
{
    public Task<GroupPreOrderView> AnsweredByAsync(Actor actor)
    {
        var response = actor.TheLastResponse();
        if (!response.Succeeded)
        {
            var state = response.Status switch
            {
                401 => GroupPreOrderState.SignInRequired,
                403 => GroupPreOrderState.Refused,
                404 => GroupPreOrderState.NotFound,
                _ => GroupPreOrderState.Failed,
            };
            return Task.FromResult(new GroupPreOrderView(state, [], 0, $"{response.Status}: {response.Body}"));
        }

        var group = response.Json<GroupParticipantsResponse>();
        var participants = group.Participants
            .Select(p => new ParticipantView(
                p.Name,
                p.Type == ParticipantType.Owner,
                p.Items.Select(i => new CartLine(SampleDataFactory.WithoutPrefix(i.Dish.Name), i.Quantity)).ToList(),
                p.Total,
                p.IsSubmitted))
            .ToList();

        return Task.FromResult(new GroupPreOrderView(
            participants.Count == 0 ? GroupPreOrderState.Empty : GroupPreOrderState.Shown,
            participants,
            group.Total,
            response.Body));
    }
}
