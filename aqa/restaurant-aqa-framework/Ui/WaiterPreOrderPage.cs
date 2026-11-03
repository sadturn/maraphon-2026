using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

/// <summary>A booking's pre-order as the waiter sees it (`/waiter-pre-order`, opened from the reservations screen — the
/// booking travels in the navigation state, so the page cannot be opened by its address alone). One dish card per dish,
/// with its quantity ("Quantity: 3") and Confirm / Cancel buttons.</summary>
public sealed class WaiterPreOrderPage(IPage page) : BasePage(page)
{
    public ILocator Title => Page.GetByRole(AriaRole.Heading, new() { NameRegex = new System.Text.RegularExpressions.Regex("^Pre-order of") });

    public ILocator DishCards => Page.Locator(".preorder-list app-dish-card");

    public static ILocator DishName(ILocator card) => card.Locator(".dish-name");

    public static ILocator DishQuantity(ILocator card) => card.Locator(".pre-order-quantity");
}
