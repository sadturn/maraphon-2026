using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui.Components;

/// <summary>One participant on the owner's group pre-order screen (`app-participant-card`). The title reads "My Pre-Order"
/// on the viewer's own card and "Guest: &lt;name&gt;" / "Owner: &lt;name&gt;" on the others; the chip is "Submitted" or
/// "Still choosing". Prices are formatted by the currency pipe ("$12.50").</summary>
public sealed class ParticipantCardComponent(ILocator root)
{
    public const string OwnTitle = "My Pre-Order";
    public const string GuestTitlePrefix = "Guest: ";
    public const string OwnerTitlePrefix = "Owner: ";

    public ILocator Title => root.GetByRole(AriaRole.Heading);

    public ILocator SubmittedChip => root.GetByText("Submitted", new() { Exact = true });

    public ILocator DishLines => root.Locator(".item");

    public ILocator Subtotal => root.Locator(".subtotal span").Last;

    public static ILocator DishName(ILocator line) => line.Locator(".name");

    /// <summary>"× 2"</summary>
    public static ILocator DishQuantity(ILocator line) => line.Locator(".qty");
}
