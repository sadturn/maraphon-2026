using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>The owner opens the group pre-order screen, presses Submit Pre-Order and confirms in the dialog.</summary>
public sealed class SubmitTheGroupPreOrderOnScreen : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new OpenTheGroupPreOrder(Viewer.Owner));

        var screen = new GroupOrderPage(actor.LookingAtThePage());
        // The screen makes several calls before it shows the button, each possibly waking a cold backend.
        await screen.SubmitButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = Settings.Current.NavigationTimeoutMs });
        await screen.SubmitAsync();
    }
}
