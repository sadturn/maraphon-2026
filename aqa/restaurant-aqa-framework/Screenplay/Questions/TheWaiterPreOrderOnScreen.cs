using System.Globalization;
using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The dishes and quantities on the waiter's pre-order screen, once it has loaded its dishes.</summary>
public sealed class TheWaiterPreOrderOnScreen : IQuestion<RestaurantOrder>
{
    public async Task<RestaurantOrder> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var screen = new WaiterPreOrderPage(page);
        await screen.DishCards.First
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = Settings.Current.NavigationTimeoutMs });

        var dishes = new List<CartLine>();
        foreach (var card in await screen.DishCards.AllAsync())
        {
            var name = SampleDataFactory.WithoutPrefix((await WaiterPreOrderPage.DishName(card).InnerTextAsync()).Trim());
            var quantity = (await WaiterPreOrderPage.DishQuantity(card).InnerTextAsync()).Replace("Quantity:", "", StringComparison.Ordinal).Trim();
            dishes.Add(new CartLine(name, int.Parse(quantity, CultureInfo.InvariantCulture)));
        }

        return new RestaurantOrder(dishes, null, await page.Locator("body").InnerTextAsync());
    }
}
