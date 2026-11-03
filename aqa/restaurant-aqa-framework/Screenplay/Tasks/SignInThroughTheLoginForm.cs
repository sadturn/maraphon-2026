using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Signs in through the storefront's login form and waits until the app has finished signing in: it has moved on
/// from the login page and stored the signed-in user's profile. Remembers
/// who is now signed in on the screen (<see cref="SignedInOnScreen"/>).</summary>
public sealed class SignInThroughTheLoginForm(string email, string password, bool openTheLoginPage = true) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var loginPage = new LoginPage(page);

        if (openTheLoginPage)
            await loginPage.NavigateAsync();

        await loginPage.SignInAsync(email, password);
        await page.WaitForURLAsync(url => !url.Contains("/login", StringComparison.Ordinal));

        // Signing in finishes only once the app has loaded the profile (GET /me) and stored it: the header's user menu and
        // the waiter's dashboard read it. Opening another page before that cancels the request and leaves both empty.
        await page.WaitForFunctionAsync("() => localStorage.getItem('user') !== null", null,
            new() { Timeout = Settings.Current.NavigationTimeoutMs });
        actor.Remember(new SignedInOnScreen(email));
    }
}
