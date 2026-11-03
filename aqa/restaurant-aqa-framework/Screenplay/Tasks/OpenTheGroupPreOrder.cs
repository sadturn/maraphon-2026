using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Opens the owner's group pre-order screen of the scenario's booking (or of one that does not exist) as
/// <paramref name="viewer"/>: someone with an account signs in on the login form first; a guest has no account, so on
/// screen a guest is just a visitor who is not signed in. Meant to be done once per scenario: a signed-in browser is
/// sent away from the login form.</summary>
public sealed class OpenTheGroupPreOrder(Viewer viewer, bool ofABookingThatDoesNotExist = false) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new MeetTheViewer(viewer));

        var account = viewer switch
        {
            Viewer.Owner => actor.Recall<Account>(TheBearerTokenOf.Owner),
            Viewer.AnotherCustomer => actor.Recall<Account>(MeetTheViewer.AnotherCustomer),
            Viewer.Waiter => actor.Recall<Account>(HireAWaiter.Who),
            _ => null,
        };
        if (account is not null)
            await actor.AttemptsToAsync(new SignInThroughTheLoginForm(account.Email, account.Password));

        var reservationId = ofABookingThatDoesNotExist ? Guid.NewGuid().ToString() : actor.Recall<GroupBooking>().ReservationId;
        await new GroupOrderPage(actor.LookingAtThePage()).NavigateAsync(reservationId);
    }
}
