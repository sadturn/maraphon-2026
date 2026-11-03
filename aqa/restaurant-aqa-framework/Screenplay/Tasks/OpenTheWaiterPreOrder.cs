using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>The scenario's waiter signs in (whoever used the screen before signs out first), lands on their reservations
/// screen and opens the booking's pre-order. The waiter was hired for this scenario's location, so this booking is the
/// only one on their screen.</summary>
public sealed class OpenTheWaiterPreOrder : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var waiter = actor.Recall<Account>(HireAWaiter.Who);
        await actor.AttemptsToAsync(new SignOutOnScreen(), new SignInThroughTheLoginForm(waiter.Email, waiter.Password));

        // Not opened by its address: the screen needs the profile the app loads after signing in, and the app itself
        // brings a waiter here once it has it. Going there sooner shows "Your session has expired" and no bookings.
        var page = actor.LookingAtThePage();
        await page.WaitForURLAsync(url => url.Contains(WaiterReservationsPage.Path, StringComparison.Ordinal),
            new() { Timeout = Settings.Current.NavigationTimeoutMs });

        await new WaiterReservationsPage(page).OpenPreOrderAsync();
    }
}
