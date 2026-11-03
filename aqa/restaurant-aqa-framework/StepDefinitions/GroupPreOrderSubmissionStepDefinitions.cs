using System.Globalization;
using System.Text.RegularExpressions;
using Restaurant_Aqa_Framework.Api.Dtos.Orders;
using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.StepDefinitions;

/// <summary>Submitting the group pre-order and what the restaurant receives. The restaurant, the booking and the guests
/// come from <see cref="GroupPreOrderStepDefinitions"/>. Submitting and the waiter opening the order are the
/// stage-specific steps; both remember their answer, so the Then steps work on either stage.</summary>
[Binding]
public sealed partial class GroupPreOrderSubmissionStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    private const string OnlyTheOwner = "Only the reservation owner can perform this action.";

    [Given("the owner has pre-ordered {int} {word}")]
    public Task GivenOwnerHasPreOrdered(int quantity, string dish) =>
        Actor.AttemptsToAsync(new PreOrderForTheOwner([new CartLine(dish, quantity)]));

    [Given("the owner has submitted the group pre-order")]
    public Task GivenOwnerHasSubmitted() =>
        Actor.AttemptsToAsync(new SubmitTheGroupPreOrder(Viewer.Owner, mustSucceed: true));

    // ---- submitting ----

    [When("{viewer} submits the group pre-order")]
    [Scope(Tag = "api")]
    public Task WhenViewerSubmitsThroughTheService(Viewer viewer) => SubmitThroughTheServiceAsync(viewer, SubmissionTarget.TheScenariosGroup);

    [When("the owner submits the group pre-order")]
    [Scope(Tag = "ui")]
    public async Task WhenOwnerSubmitsOnScreen()
    {
        await Actor.AttemptsToAsync(new SubmitTheGroupPreOrderOnScreen());
        Actor.Remember(await Actor.AsksForAsync(new TheSubmissionOutcomeOnScreen()));
    }

    [When("the owner submits a group that does not exist")]
    public Task WhenOwnerSubmitsAMissingGroup() => SubmitThroughTheServiceAsync(Viewer.Owner, SubmissionTarget.AGroupThatDoesNotExist);

    [When("the owner submits the group for a booking that does not exist")]
    public Task WhenOwnerSubmitsForAMissingBooking() => SubmitThroughTheServiceAsync(Viewer.Owner, SubmissionTarget.ABookingThatDoesNotExist);

    // ---- the restaurant's side ----

    [When("the waiter opens the booking's pre-order")]
    [Scope(Tag = "api")]
    public async Task WhenWaiterOpensThePreOrderThroughTheService() =>
        Actor.Remember(await Actor.AsksForAsync(new TheRestaurantsOrder()));

    [When("the waiter opens the booking's pre-order")]
    [Scope(Tag = "ui")]
    public async Task WhenWaiterOpensThePreOrderOnScreen()
    {
        await Actor.AttemptsToAsync(new OpenTheWaiterPreOrder());
        Actor.Remember(await Actor.AsksForAsync(new TheWaiterPreOrderOnScreen()));
    }

    [Then("the restaurant receives these dishes")]
    public async Task ThenRestaurantReceivesTheseDishes(DataTable dishes)
    {
        var order = await Actor.AsksForAsync(new TheRestaurantsOrder());
        await Assert.That(Describe(order.Dishes)).IsEqualTo(Describe(Lines(dishes))).Because(order.Evidence);
    }

    [Then("the waiter sees these dishes")]
    public async Task ThenWaiterSeesTheseDishes(DataTable dishes)
    {
        var order = Recall<RestaurantOrder>();
        await Assert.That(Describe(order.Dishes)).IsEqualTo(Describe(Lines(dishes))).Because(order.Evidence);
    }

    [Then("the restaurant's order is marked as submitted by the customer")]
    public async Task ThenOrderIsSubmittedByTheCustomer()
    {
        var order = await Actor.AsksForAsync(new TheRestaurantsOrder());
        await Assert.That(order.Status).IsEqualTo(OrderStatus.SubmittedByCustomer).Because(order.Evidence);
    }

    [Then("the group pre-order is marked as submitted")]
    public async Task ThenGroupIsSubmitted() =>
        await Assert.That(await Actor.AsksForAsync(new TheGroupStatus())).IsEqualTo("Submitted");

    [Then("the group pre-order is still open")]
    public async Task ThenGroupIsStillOpen() =>
        await Assert.That(await Actor.AsksForAsync(new TheGroupStatus())).IsEqualTo("Open");

    // ---- how the submission went ----

    [Then("the pre-order is sent to the restaurant")]
    public async Task ThenPreOrderIsSent()
    {
        var outcome = Recall<SubmissionOutcome>();
        await Assert.That(outcome.Sent).IsTrue().Because(outcome.Evidence);
    }

    [Then("the owner is told {string}")]
    public async Task ThenOwnerIsTold(string message)
    {
        var outcome = Recall<SubmissionOutcome>();
        await Assert.That(outcome.Sent).IsFalse().Because(outcome.Evidence);
        await Assert.That(outcome.Message).IsEqualTo(message).Because(outcome.Evidence);
    }

    /// <summary>The message names the reservation ("Order for reservation &lt;id&gt; was not found."), so only its shape is fixed.</summary>
    [Then("the owner is told their own pre-order could not be found")]
    public async Task ThenOwnerIsToldTheirPreOrderIsMissing()
    {
        var outcome = Recall<SubmissionOutcome>();
        await Assert.That(outcome.Sent).IsFalse().Because(outcome.Evidence);
        await Assert.That(OrderNotFoundRegex().IsMatch(outcome.Message)).IsTrue().Because($"the owner was told '{outcome.Message}'");
    }

    [Then("the owner is told the {submission_target} could not be found")]
    public async Task ThenOwnerIsToldTargetIsMissing(SubmissionTarget target)
    {
        var outcome = Recall<SubmissionOutcome>();
        await Assert.That(outcome.Status).IsEqualTo(404).Because(outcome.Evidence);
        await Assert.That(outcome.Message).StartsWith(target == SubmissionTarget.AGroupThatDoesNotExist ? "Group reservation" : "Reservation")
            .Because(outcome.Evidence);
    }

    [Then("they are refused because only the owner may submit it")]
    public async Task ThenTheyAreRefusedSubmitting()
    {
        var outcome = Recall<SubmissionOutcome>();
        await Assert.That(outcome.Status).IsEqualTo(403).Because(outcome.Evidence);
        await Assert.That(outcome.Message).IsEqualTo(OnlyTheOwner).Because(outcome.Evidence);
    }

    [Then("they are asked to sign in before submitting")]
    public async Task ThenTheyAreAskedToSignInBeforeSubmitting()
    {
        var outcome = Recall<SubmissionOutcome>();
        await Assert.That(outcome.Status).IsEqualTo(401).Because(outcome.Evidence);
    }

    [Then("the owner can no longer submit the group pre-order")]
    public async Task ThenOwnerCanNoLongerSubmit() =>
        await Assert.That(await Actor.AsksForAsync(new TheSubmitButtonIsShown())).IsFalse();

    private async Task SubmitThroughTheServiceAsync(Viewer viewer, SubmissionTarget target)
    {
        await Actor.AttemptsToAsync(new SubmitTheGroupPreOrder(viewer, target));
        Actor.Remember(await Actor.AsksForAsync(new TheSubmissionOutcome()));
    }

    private static List<CartLine> Lines(DataTable dishes) =>
        dishes.Rows.Select(d => new CartLine(d["dish"], int.Parse(d["quantity"], CultureInfo.InvariantCulture))).ToList();

    private static string Describe(IEnumerable<CartLine> lines) =>
        string.Join(", ", lines.OrderBy(l => l.Dish, StringComparer.Ordinal));

    [GeneratedRegex(@"^Order for reservation \S+ was not found\.$")]
    private static partial Regex OrderNotFoundRegex();
}
