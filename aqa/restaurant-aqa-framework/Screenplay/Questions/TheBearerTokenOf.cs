using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>What a viewer's calls carry: a customer's or waiter's id token, a guest's session token (the only credential
/// a guest has), or nothing for a visitor who is not signed in.</summary>
public sealed class TheBearerTokenOf(Viewer viewer) : IQuestion<string?>
{
    /// <summary>Who the owner is remembered as; <see cref="BookATableForAGroup"/> books for them unless told otherwise.</summary>
    public const string Owner = "owner";

    public Task<string?> AnsweredByAsync(Actor actor) => Task.FromResult<string?>(viewer switch
    {
        Viewer.Owner => actor.Recall<Account>(Owner).IdToken,
        Viewer.AnotherCustomer => actor.Recall<Account>(MeetTheViewer.AnotherCustomer).IdToken,
        Viewer.Waiter => actor.Recall<Account>(HireAWaiter.Who).IdToken,
        Viewer.GuestOfTheGroup => actor.Recall<GuestInGroup>().SessionToken,
        Viewer.VisitorNotSignedIn => null,
        _ => throw new InvalidOperationException($"Unknown viewer '{viewer}'."),
    });
}
