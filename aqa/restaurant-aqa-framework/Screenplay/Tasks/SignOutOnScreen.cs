using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Whoever is signed in on the screen signs out through the header's user menu; does nothing when nobody is.</summary>
public sealed class SignOutOnScreen : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        if (!actor.TryRecall<SignedInOnScreen>(out var session) || session.Email is null)
            return;

        var header = new HeaderComponent(actor.LookingAtThePage());
        await header.SignOutAsync();
        await header.SignInLink.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        actor.Remember(new SignedInOnScreen(null));
    }
}
