using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.StepDefinitions;

/// <summary>The owner removes a guest from the group pre-order. Removing is the stage-specific step; it remembers the
/// group as the owner sees it afterwards (the service answers a removal with the updated list), so the Then steps of
/// <see cref="GroupPreOrderStepDefinitions"/> check what is left. Refusals are read from the outcome of the call.</summary>
[Binding]
public sealed class GroupPreOrderRemovalStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    private const string SecondCustomer = "second customer";
    private const string OnlyTheOwner = "Only the reservation owner can perform this action.";
    private const string NotInThisGroup = "Guest session does not belong to this group reservation.";

    // A table as big as the first one, so the second booking is never refused for its size.
    private const int TableSeats = 8;

    [Given("a second customer has booked another table for {int} guests")]
    public Task GivenASecondCustomerHasBookedAnotherTable(int guests) =>
        Actor.AttemptsToAsync(
            new AddTable(TableSeats), new RegisterACustomer(SecondCustomer), new BookATableForAGroup(guests, by: SecondCustomer));

    [Given("{word} has joined the second customer's group")]
    public Task GivenGuestHasJoinedTheSecondGroup(string guest) => Actor.AttemptsToAsync(new JoinTheGroup(guest, bookedBy: SecondCustomer));

    [Given("the owner has removed {word} from the group")]
    public async Task GivenOwnerHasRemoved(string guest)
    {
        await Actor.AttemptsToAsync(new RemoveTheGuest(Viewer.Owner, guest));
        Actor.TheLastResponse().ThrowUnlessSucceeded($"Removing {guest}");
    }

    // ---- removing ----

    [When("{viewer} removes {word} from the group")]
    [Scope(Tag = "api")]
    public async Task WhenViewerRemovesThroughTheService(Viewer viewer, string guest)
    {
        await Actor.AttemptsToAsync(new RemoveTheGuest(viewer, guest));
        Actor.Remember(await Actor.AsksForAsync(new TheGroupPreOrderFromTheService()));
    }

    [When("the owner removes {word} from the group")]
    [Scope(Tag = "ui")]
    public async Task WhenOwnerRemovesOnScreen(string guest)
    {
        await Actor.AttemptsToAsync(new RemoveTheGuestOnScreen(guest));
        Actor.Remember(await Actor.AsksForAsync(new TheGroupPreOrderOnScreen()));
    }

    [When("the owner removes a guest who does not exist")]
    public Task WhenOwnerRemovesAGuestWhoDoesNotExist() => Actor.AttemptsToAsync(new RemoveTheGuest(Viewer.Owner, guestName: null));

    // ---- refusals ----

    [Then("the owner is told the guest does not belong to this group")]
    public async Task ThenOwnerIsToldTheGuestIsNotInThisGroup()
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());
        await Assert.That(outcome.Status).IsEqualTo(400).Because(outcome.Body);
        await Assert.That(outcome.ErrorMessage).IsEqualTo(NotInThisGroup).Because(outcome.Body);
    }

    [Then("the owner is told the guest could not be found")]
    public async Task ThenOwnerIsToldTheGuestIsMissing()
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());
        await Assert.That(outcome.Status).IsEqualTo(404).Because(outcome.Body);
        await Assert.That(outcome.ErrorMessage).Contains("not found").Because(outcome.Body);
    }

    [Then("they are refused because only the owner may remove guests")]
    public async Task ThenTheyAreRefusedRemoving()
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());
        await Assert.That(outcome.Status).IsEqualTo(403).Because(outcome.Body);
        await Assert.That(outcome.ErrorMessage).IsEqualTo(OnlyTheOwner).Because(outcome.Body);
    }

    [Then("they are asked to sign in before removing guests")]
    public async Task ThenTheyAreAskedToSignInBeforeRemoving()
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());
        await Assert.That(outcome.Status).IsEqualTo(401).Because(outcome.Body);
    }

    [Then("{word} is still in the group")]
    public async Task ThenGuestIsStillInTheGroup(string guest) =>
        await Assert.That(await Actor.AsksForAsync(new TheGuestsInTheGroup())).Contains(guest);
}
