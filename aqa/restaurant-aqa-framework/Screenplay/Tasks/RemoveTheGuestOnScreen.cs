using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>The owner removes a guest on the group pre-order screen — Remove Guest on the guest's card, then confirm —
/// and waits until the card is gone, i.e. the screen has shown the service's updated list.</summary>
public sealed class RemoveTheGuestOnScreen(string guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new OpenTheGroupPreOrder(Viewer.Owner));

        var screen = new GroupOrderPage(actor.LookingAtThePage());
        var timeout = Settings.Current.NavigationTimeoutMs;
        // The screen makes several calls before it shows the cards, each possibly waking a cold backend.
        await screen.RemoveGuestButton(guestName).WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeout });
        await screen.RemoveGuestAsync(guestName);
        await screen.RemoveGuestButton(guestName).WaitForAsync(new() { State = WaitForSelectorState.Detached, Timeout = timeout });
    }
}
