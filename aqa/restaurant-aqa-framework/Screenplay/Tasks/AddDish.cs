using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Dishes;
using Restaurant_Aqa_Framework.Api.Dtos.Locations;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Screenplay.Questions;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Adds a sample dish of the given category to a location: the one named, or else the one this scenario
/// set up most recently. A dish <paramref name="called"/> something gets that name (behind the test-data prefix) and is
/// remembered under it; a <paramref name="price"/> replaces the random one where a scenario adds up totals.</summary>
public sealed class AddDish(
    DishType type = DishType.MainCourse, string? toLocation = null, string? called = null, decimal? price = null) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var location = actor.Recall<LocationResponse>(toLocation);
        var api = new DishesApi(actor.UsingTheApi());
        var response = actor.TheLastResponse();

        var request = SampleDataFactory.CreateDishRequest([location.Id], type);
        if (called is not null)
            request = request with { Name = SampleDataFactory.DishName(called) };
        if (price is not null)
            request = request with { Price = price.Value };

        await response.CaptureAsync(await api.CreateDishAsync(request));
        if (!response.Succeeded)
            throw new InvalidOperationException($"Adding the dish failed with {response.Status}: {response.Body}");

        var dish = response.Json<DishResponse>();
        if (called is null)
            actor.Remember(dish);
        else
            actor.Remember(called, dish);

        // The waiter is asked for only when the cleanup runs, so a scenario can hire its own waiter after this.
        actor.Recall<CleanupScope>().TrackDelete(
            $"dish:{dish.Id}",
            async () => await api.DeleteDishAsync(dish.Id, force: true, bearerToken: (await actor.AsksForAsync(new TheWaiterSession())).IdToken));
    }
}
