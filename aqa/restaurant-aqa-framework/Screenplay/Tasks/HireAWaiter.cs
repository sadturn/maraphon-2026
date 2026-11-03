using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Auth;
using Restaurant_Aqa_Framework.Api.Dtos.Locations;
using Restaurant_Aqa_Framework.Api.Dtos.Waiters;
using Restaurant_Aqa_Framework.Api.Seeding;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Gives a location a waiter of the scenario's own: registers a customer and makes them a waiter there. The
/// location set up for the scenario has no other waiter, so every booking at it is assigned to this one. The waiter's
/// sign-in becomes the scenario's <see cref="WaiterSession"/>, which also removes what the scenario set up, and the
/// account is remembered as <see cref="Who"/>. No standing waiter account is needed.</summary>
public sealed class HireAWaiter(string? atLocation = null) : ITask
{
    public const string Who = "waiter";

    public async Task PerformAsAsync(Actor actor)
    {
        var location = actor.Recall<LocationResponse>(atLocation);
        var api = actor.UsingTheApi();
        var auth = new AuthApi(api);
        var waiters = new WaitersApi(api);
        var response = actor.TheLastResponse();
        var person = SampleDataFactory.RegisterRequest();

        await response.CaptureAsync(await auth.RegisterAsync(person));
        response.ThrowUnlessSucceeded("Registering the waiter");

        var hiring = new WaiterRequest(person.Email, location.Id);
        await response.CaptureAsync(await waiters.AddWaiterAsync(hiring));
        response.ThrowUnlessSucceeded("Making the new account a waiter");
        actor.Recall<CleanupScope>().TrackDelete($"waiter:{person.Email}", () => waiters.RemoveWaiterAsync(hiring));

        // Signed in only now: the waiter role is a group claim in the id token, so it has to be issued after the hiring.
        await response.CaptureAsync(await auth.SignInAsync(new SignInRequest(person.Email, person.Password)));
        response.ThrowUnlessSucceeded("Signing in the waiter");

        var idToken = response.Json<SignInResponse>().IdToken;
        actor.Remember(new WaiterSession(idToken));
        actor.Remember(Who, new Account(person.Email, person.Password, idToken));
    }
}
