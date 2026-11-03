using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Makes sure whoever is about to look at the group pre-order exists: the owner and the waiter come with the
/// restaurant and the booking, another customer is registered now, a guest joins the group unless one already has.</summary>
public sealed class MeetTheViewer(Viewer viewer) : ITask
{
    public const string AnotherCustomer = "another customer";
    private const string VisitingGuest = "Yarema";

    public async Task PerformAsAsync(Actor actor)
    {
        switch (viewer)
        {
            case Viewer.AnotherCustomer:
                await actor.AttemptsToAsync(new RegisterACustomer(AnotherCustomer));
                break;
            case Viewer.GuestOfTheGroup when !actor.TryRecall<GuestInGroup>(out _):
                await actor.AttemptsToAsync(new JoinTheGroup(VisitingGuest));
                break;
        }
    }
}
