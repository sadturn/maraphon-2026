using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Ui;

/// <summary>The owner's group pre-order screen (`/reservations/{reservationId}/group-order`). It has no test ids, so it is
/// located by roles and visible text first, and by the participant card's own element and classes where nothing else
/// tells its parts apart. Participant cards are only rendered once someone has added a dish; until then the screen says
/// so instead.</summary>
public sealed partial class GroupOrderPage(IPage page) : BasePage(page)
{
    public ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Group Pre-Order", Level = 1 });

    /// <summary>"Updated at …" (or "Couldn't refresh …") — shown once the screen has loaded.</summary>
    public ILocator SyncStatus => Page.GetByRole(AriaRole.Status).Filter(new() { HasTextRegex = LoadedRegex() });

    public ILocator NothingAddedYet => Page.GetByText(NothingAddedRegex());

    public ILocator NotFound => Page.GetByRole(AriaRole.Alert).Filter(new() { HasText = "Group reservation not found" });

    public ILocator LoadFailed => Page.GetByRole(AriaRole.Alert).Filter(new() { HasTextRegex = LoadFailedRegex() });

    public ILocator GrandTotal => Page.Locator(".grand-total-value");

    public ILocator ParticipantCards => Page.Locator("app-participant-card").GetByRole(AriaRole.Article);

    public ParticipantCardComponent ParticipantCard(int index) => new(ParticipantCards.Nth(index));

    public Task NavigateAsync(string reservationId) => NavigateToAsync($"/reservations/{reservationId}/group-order");

    [GeneratedRegex("Updated at|Couldn.t refresh")]
    private static partial Regex LoadedRegex();

    // The screen writes typographic apostrophes (haven’t, Couldn’t); "." matches either kind.
    [GeneratedRegex("Participants haven.t added any dishes yet")]
    private static partial Regex NothingAddedRegex();

    [GeneratedRegex("Couldn.t load the group pre-order")]
    private static partial Regex LoadFailedRegex();
}
