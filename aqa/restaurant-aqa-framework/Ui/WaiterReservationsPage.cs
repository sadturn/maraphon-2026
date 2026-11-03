using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

/// <summary>The waiter's reservations screen (`/waiter-reservations`): one `app-card-reservation` per booking assigned to
/// the waiter. A card offers "Pre-order" once the booking's order has dishes. The screen reads the signed-in waiter's
/// profile, which the app stores only after sign-in completes — the app then brings the waiter here by itself.</summary>
public sealed class WaiterReservationsPage(IPage page) : BasePage(page)
{
    public const string Path = "/waiter-reservations";

    public ILocator ReservationCards => Page.Locator("app-card-reservation");

    public static ILocator PreOrderButton(ILocator card) =>
        card.GetByRole(AriaRole.Button, new() { Name = "Pre-order", Exact = true });

    public Task NavigateAsync() => NavigateToAsync(Path);

    /// <summary>Opens the pre-order of the only booking on the screen.</summary>
    public Task OpenPreOrderAsync() => PreOrderButton(ReservationCards.First).ClickAsync();
}
