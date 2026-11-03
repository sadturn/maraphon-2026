using System.Globalization;
using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.StepDefinitions;

/// <summary>A group pre-order: the owner's booking, the guests who join it with their carts, and what the owner — or
/// someone who is not the owner — gets to see. Looking is the one stage-specific step: through the service on `@api`,
/// on the owner's screen on `@ui`. Both remember a <see cref="GroupPreOrderView"/>, so every Then step works on either.</summary>
[Binding]
public sealed class GroupPreOrderStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    // A table for the largest group a scenario books, so a booking is never refused for its size.
    private const int TableSeats = 8;

    // ---- the restaurant and the booking ----

    [Given("a restaurant whose menu has")]
    public async Task GivenARestaurantWhoseMenuHas(DataTable menu)
    {
        await Actor.AttemptsToAsync(new SetUpLocation(), new HireAWaiter(), new AddTable(TableSeats));
        foreach (var dish in menu.Rows)
            await Actor.AttemptsToAsync(new AddDish(called: dish["dish"], price: decimal.Parse(dish["price"], CultureInfo.InvariantCulture)));
    }

    [Given("a customer has booked a table for {int} guests")]
    public Task GivenACustomerHasBookedATableFor(int guests) =>
        Actor.AttemptsToAsync(new RegisterACustomer(TheBearerTokenOf.Owner), new BookATableForAGroup(guests));

    // ---- the guests ----

    [Given("these guests have joined the group")]
    public async Task GivenTheseGuestsHaveJoinedTheGroup(DataTable guests)
    {
        foreach (var guest in guests.Rows)
        {
            var name = guest["guest"];
            var cart = CartLine.ParseAll(guest["cart"]);

            await Actor.AttemptsToAsync(new JoinTheGroup(name));
            if (cart.Count > 0)
                await Actor.AttemptsToAsync(ChangeTheCart.PutIn(name, cart));
            if (IsYes(guest["cart submitted"]))
                await Actor.AttemptsToAsync(new SubmitTheCart(name));
        }
    }

    [When("{word} puts {int} {word} in the cart")]
    public Task WhenGuestPutsInTheCart(string guest, int quantity, string dish) =>
        Actor.AttemptsToAsync(ChangeTheCart.PutIn(guest, [new CartLine(dish, quantity)]));

    [When("{word} takes {int} {word} out of the cart")]
    public Task WhenGuestTakesOutOfTheCart(string guest, int quantity, string dish) =>
        Actor.AttemptsToAsync(ChangeTheCart.TakeOut(guest, [new CartLine(dish, quantity)]));

    // ---- looking at it ----

    [When("{viewer} looks at the group pre-order")]
    [Scope(Tag = "api")]
    public async Task WhenViewerLooksThroughTheService(Viewer viewer)
    {
        await Actor.AttemptsToAsync(new ReviewTheGroupPreOrder(viewer));
        Actor.Remember(await Actor.AsksForAsync(new TheGroupPreOrderFromTheService()));
    }

    [When("{viewer} looks at the group pre-order")]
    [Scope(Tag = "ui")]
    public async Task WhenViewerLooksOnScreen(Viewer viewer)
    {
        await Actor.AttemptsToAsync(new OpenTheGroupPreOrder(viewer));
        Actor.Remember(await Actor.AsksForAsync(new TheGroupPreOrderOnScreen()));
    }

    [When("the owner looks at a group pre-order that does not exist")]
    [Scope(Tag = "api")]
    public async Task WhenOwnerLooksAtAMissingOneThroughTheService()
    {
        await Actor.AttemptsToAsync(new ReviewTheGroupPreOrder(Viewer.Owner, ofABookingThatDoesNotExist: true));
        Actor.Remember(await Actor.AsksForAsync(new TheGroupPreOrderFromTheService()));
    }

    [When("the owner looks at a group pre-order that does not exist")]
    [Scope(Tag = "ui")]
    public async Task WhenOwnerLooksAtAMissingOneOnScreen()
    {
        await Actor.AttemptsToAsync(new OpenTheGroupPreOrder(Viewer.Owner, ofABookingThatDoesNotExist: true));
        Actor.Remember(await Actor.AsksForAsync(new TheGroupPreOrderOnScreen()));
    }

    // ---- what they see ----

    /// <summary>Compares the guests only: the owner is a participant too once their own session exists (the owner's
    /// screen opens it), and is not what these rules are about.</summary>
    [Then("the owner sees these guests")]
    public async Task ThenOwnerSeesTheseGuests(DataTable guests)
    {
        var view = Recall<GroupPreOrderView>();
        await Assert.That(view.State).IsEqualTo(GroupPreOrderState.Shown).Because(view.Evidence);

        var expected = guests.Rows
            .Select(g => Describe(g["guest"], CartLine.ParseAll(g["dishes"]), decimal.Parse(g["subtotal"], CultureInfo.InvariantCulture), IsYes(g["cart submitted"])))
            .Order(StringComparer.Ordinal);
        var actual = view.Guests
            .Select(g => Describe(g.Name, g.Dishes, g.Subtotal, g.IsSubmitted))
            .Order(StringComparer.Ordinal);

        await Assert.That(string.Join("\n", actual)).IsEqualTo(string.Join("\n", expected)).Because(view.Evidence);
    }

    [Then("the grand total is {money}")]
    public async Task ThenGrandTotalIs(decimal total)
    {
        var view = Recall<GroupPreOrderView>();
        await Assert.That(view.GrandTotal).IsEqualTo(total).Because(view.Evidence);
    }

    /// <summary>The owner's screen shows the consolidated order as its grand total, which leaves out unsubmitted carts.</summary>
    [Then("the grand total of the submitted carts is {money}")]
    [Scope(Tag = "ui")]
    public Task ThenGrandTotalOfSubmittedCartsIs(decimal total) => ThenGrandTotalIs(total);

    [Then("the grand total is the sum of the participants' subtotals")]
    public async Task ThenGrandTotalIsTheSumOfSubtotals()
    {
        var view = Recall<GroupPreOrderView>();
        await Assert.That(view.GrandTotal).IsEqualTo(view.Participants.Sum(p => p.Subtotal)).Because(view.Evidence);
    }

    [Then("the group pre-order is empty")]
    public async Task ThenGroupPreOrderIsEmpty()
    {
        var view = Recall<GroupPreOrderView>();
        await Assert.That(view.State).IsEqualTo(GroupPreOrderState.Empty).Because(view.Evidence);
        await Assert.That(view.GrandTotal).IsEqualTo(0m).Because(view.Evidence);
    }

    [Then("they are refused because only the owner may see it")]
    public Task ThenTheyAreRefused() => ThenStateIs(GroupPreOrderState.Refused);

    [Then("they are asked to sign in first")]
    public Task ThenTheyAreAskedToSignIn() => ThenStateIs(GroupPreOrderState.SignInRequired);

    [Then("the owner is told the group pre-order could not be found")]
    public Task ThenOwnerIsToldItCouldNotBeFound() => ThenStateIs(GroupPreOrderState.NotFound);

    private async Task ThenStateIs(GroupPreOrderState expected)
    {
        var view = Recall<GroupPreOrderView>();
        await Assert.That(view.State).IsEqualTo(expected).Because(view.Evidence);
    }

    private static bool IsYes(string cell) => cell.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);

    private static string Describe(string guest, IEnumerable<CartLine> dishes, decimal subtotal, bool submitted) =>
        $"{guest} | {string.Join(", ", dishes.OrderBy(d => d.Dish, StringComparer.Ordinal))} | {Money.Format(subtotal)} | {(submitted ? "submitted" : "still choosing")}";
}
