using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The outcome of submitting on the owner's screen: the badge turns to "Sent to Restaurant", or an error toast
/// carries the service's own message. Waits for the badge rather than for any success toast — an earlier action on the
/// same screen (removing a guest) leaves a success toast of its own for a few seconds. Error toasts disappear too, so
/// ask straight after submitting.</summary>
public sealed class TheSubmissionOutcomeOnScreen : IQuestion<SubmissionOutcome>
{
    public const string SentBadge = "Sent to Restaurant";

    public async Task<SubmissionOutcome> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var toast = new ToastComponent(page);
        var sent = new GroupOrderPage(page).StatusBadge.Filter(new() { HasText = SentBadge });

        await sent.Or(toast.Error).First
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = Settings.Current.NavigationTimeoutMs });

        var evidence = await page.Locator("body").InnerTextAsync();
        if (await sent.IsVisibleAsync())
            return new SubmissionOutcome(true, null, SentBadge, evidence);

        return new SubmissionOutcome(false, null, (await toast.Error.First.InnerTextAsync()).Trim(), evidence);
    }
}
