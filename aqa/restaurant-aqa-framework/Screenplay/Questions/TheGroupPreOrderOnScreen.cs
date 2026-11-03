using System.Globalization;
using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>The group pre-order as the owner's screen shows it, once the screen has settled. Its Grand Total is the
/// consolidated order — submitted carts only. The screen does not tell a refusal from any other failure to load, so
/// "Couldn't load the group pre-order" counts as <see cref="GroupPreOrderState.Refused"/>; being sent to the login form
/// is <see cref="GroupPreOrderState.SignInRequired"/>.</summary>
public sealed class TheGroupPreOrderOnScreen : IQuestion<GroupPreOrderView>
{
    public async Task<GroupPreOrderView> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var screen = new GroupOrderPage(page);
        var signIn = new LoginPage(page).SubmitButton;

        // The screen makes several calls in a row before it settles, each possibly waking a cold backend, so it is given
        // as long as a navigation rather than the default action timeout.
        await screen.SyncStatus.Or(screen.NotFound).Or(screen.LoadFailed).Or(signIn).First
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = Settings.Current.NavigationTimeoutMs });

        var evidence = await page.Locator("body").InnerTextAsync();
        if (await signIn.IsVisibleAsync())
            return new GroupPreOrderView(GroupPreOrderState.SignInRequired, [], 0, evidence);
        if (await screen.NotFound.IsVisibleAsync())
            return new GroupPreOrderView(GroupPreOrderState.NotFound, [], 0, evidence);
        if (await screen.LoadFailed.IsVisibleAsync())
            return new GroupPreOrderView(GroupPreOrderState.Refused, [], 0, evidence);
        if (await screen.NothingAddedYet.IsVisibleAsync())
            return new GroupPreOrderView(GroupPreOrderState.Empty, [], 0, evidence);

        var participants = new List<ParticipantView>();
        for (var i = 0; i < await screen.ParticipantCards.CountAsync(); i++)
            participants.Add(await ReadAsync(screen.ParticipantCard(i)));

        return new GroupPreOrderView(
            GroupPreOrderState.Shown, participants, Money.Parse(await screen.GrandTotal.InnerTextAsync()), evidence);
    }

    private static async Task<ParticipantView> ReadAsync(ParticipantCardComponent card)
    {
        var title = (await card.Title.InnerTextAsync()).Trim();
        var isOwner = title == ParticipantCardComponent.OwnTitle || title.StartsWith(ParticipantCardComponent.OwnerTitlePrefix, StringComparison.Ordinal);
        var name = title.StartsWith(ParticipantCardComponent.GuestTitlePrefix, StringComparison.Ordinal)
            ? title[ParticipantCardComponent.GuestTitlePrefix.Length..]
            : title;

        var dishes = new List<CartLine>();
        foreach (var line in await card.DishLines.AllAsync())
        {
            var dish = SampleDataFactory.WithoutPrefix((await ParticipantCardComponent.DishName(line).InnerTextAsync()).Trim());
            var quantity = (await ParticipantCardComponent.DishQuantity(line).InnerTextAsync()).Trim().TrimStart('×').Trim();
            dishes.Add(new CartLine(dish, int.Parse(quantity, CultureInfo.InvariantCulture)));
        }

        return new ParticipantView(
            name,
            isOwner,
            dishes,
            Money.Parse(await card.Subtotal.InnerTextAsync()),
            await card.SubmittedChip.CountAsync() > 0);
    }
}
