using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The outcome of submitting on the owner's screen: a success toast and the "Sent to Restaurant" badge, or an
/// error toast carrying the service's own message. Toasts disappear after a few seconds, so ask straight after submitting.</summary>
public sealed class TheSubmissionOutcomeOnScreen : IQuestion<SubmissionOutcome>
{
    public const string SentBadge = "Sent to Restaurant";

    public async Task<SubmissionOutcome> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var toast = new ToastComponent(page);

        await toast.Success.Or(toast.Error).First
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = Settings.Current.NavigationTimeoutMs });

        var evidence = await page.Locator("body").InnerTextAsync();
        if (await toast.Error.IsVisibleAsync())
            return new SubmissionOutcome(false, null, (await toast.Error.InnerTextAsync()).Trim(), evidence);

        var badge = (await new GroupOrderPage(page).StatusBadge.InnerTextAsync()).Trim();
        return new SubmissionOutcome(badge == SentBadge, null, (await toast.Success.InnerTextAsync()).Trim(), evidence);
    }
}
