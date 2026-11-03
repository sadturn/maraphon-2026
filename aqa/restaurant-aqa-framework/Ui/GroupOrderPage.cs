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

    /// <summary>"Collecting pre-orders" while the group is open, "Sent to Restaurant" once it is submitted.</summary>
    public ILocator StatusBadge => Page.Locator(".title-row .badge");

    /// <summary>Only there while the group is open.</summary>
    public ILocator SubmitButton => Page.GetByRole(AriaRole.Button, new() { Name = "Submit Pre-Order", Exact = true });

    public ILocator ConfirmDialog => Page.GetByRole(AriaRole.Dialog, new() { Name = "Submit pre-order?" });

    public async Task SubmitAsync()
    {
        await SubmitButton.ClickAsync();
        await ConfirmDialog.GetByRole(AriaRole.Button, new() { Name = "Submit", Exact = true }).ClickAsync();
    }

    public ILocator ParticipantCards => Page.Locator("app-participant-card").GetByRole(AriaRole.Article);

    public ParticipantCardComponent ParticipantCard(int index) => new(ParticipantCards.Nth(index));

    public static string PathOf(string reservationId) => $"/reservations/{reservationId}/group-order";

    public Task NavigateAsync(string reservationId) => NavigateToAsync(PathOf(reservationId));

    /// <summary>The guest's card button is labelled "Remove &lt;name&gt;"; the dialog it opens asks to confirm.</summary>
    public ILocator RemoveGuestButton(string guestName) =>
        Page.GetByRole(AriaRole.Button, new() { Name = $"Remove {guestName}", Exact = true });

    public ILocator RemoveGuestDialog => Page.GetByRole(AriaRole.Dialog, new() { Name = "Remove Guest" });

    public async Task RemoveGuestAsync(string guestName)
    {
        await RemoveGuestButton(guestName).ClickAsync();
        await RemoveGuestDialog.GetByRole(AriaRole.Button, new() { Name = "Remove Guest", Exact = true }).ClickAsync();
    }

    [GeneratedRegex("Updated at|Couldn.t refresh")]
    private static partial Regex LoadedRegex();

    // The screen writes typographic apostrophes (haven’t, Couldn’t); "." matches either kind.
    [GeneratedRegex("Participants haven.t added any dishes yet")]
    private static partial Regex NothingAddedRegex();

    [GeneratedRegex("Couldn.t load the group pre-order")]
    private static partial Regex LoadFailedRegex();
}
