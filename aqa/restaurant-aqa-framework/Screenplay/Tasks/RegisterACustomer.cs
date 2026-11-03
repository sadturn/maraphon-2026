using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Auth;
using Restaurant_Aqa_Framework.Api.Seeding;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

/// <summary>Registers a fresh customer through the service, signs them in and remembers the <see cref="Account"/> under
/// <paramref name="who"/>. Accounts cannot be deleted through the API, so each one is unique and prefixed instead.</summary>
public sealed class RegisterACustomer(string who) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var auth = new AuthApi(actor.UsingTheApi());
        var response = actor.TheLastResponse();
        var customer = SampleDataFactory.RegisterRequest();

        await response.CaptureAsync(await auth.RegisterAsync(customer));
        response.ThrowUnlessSucceeded($"Registering {who}");

        await response.CaptureAsync(await auth.SignInAsync(new SignInRequest(customer.Email, customer.Password)));
        response.ThrowUnlessSucceeded($"Signing in {who}");

        actor.Remember(who, new Account(customer.Email, customer.Password, response.Json<SignInResponse>().IdToken));
    }
}
