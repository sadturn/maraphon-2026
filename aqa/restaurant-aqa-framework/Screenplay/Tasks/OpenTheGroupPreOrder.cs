using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Opens the owner's group pre-order screen of the scenario's booking (or of one that does not exist) as
/// <paramref name="viewer"/>: someone with an account signs in on the login form first; a guest has no account, so on
/// screen a guest is just a visitor who is not signed in. Someone already signed in is not signed in again, and a screen
/// that is already open stays as it is, so a scenario can look, remove a guest and submit on the same screen.</summary>
public sealed class OpenTheGroupPreOrder(Viewer viewer, bool ofABookingThatDoesNotExist = false) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new MeetTheViewer(viewer));

        var reservationId = ofABookingThatDoesNotExist ? Guid.NewGuid().ToString() : actor.Recall<GroupBooking>().ReservationId;
        var page = actor.LookingAtThePage();
        var screen = new GroupOrderPage(page);
        if (page.Url.EndsWith(GroupOrderPage.PathOf(reservationId), StringComparison.Ordinal))
            return;

        var account = viewer switch
        {
            Viewer.Owner => actor.Recall<Account>(TheBearerTokenOf.Owner),
            Viewer.AnotherCustomer => actor.Recall<Account>(MeetTheViewer.AnotherCustomer),
            Viewer.Waiter => actor.Recall<Account>(HireAWaiter.Who),
            _ => null,
        };
        var signedIn = actor.TryRecall<SignedInOnScreen>(out var session) ? session.Email : null;
        if (account is not null && signedIn != account.Email)
        {
            await actor.AttemptsToAsync(new SignOutOnScreen());
            await actor.AttemptsToAsync(new SignInThroughTheLoginForm(account.Email, account.Password));
        }

        await screen.NavigateAsync(reservationId);
    }
}
