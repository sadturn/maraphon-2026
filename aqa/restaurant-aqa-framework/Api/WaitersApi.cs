using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Api.Dtos.Waiters;

namespace Restaurant_Aqa_Framework.Api;

/// <summary>Both routes have no gateway auth: anyone can make a registered user a waiter of a location, or take
/// that away again. That is how a scenario gets a waiter of its own (POST /register always yields a customer).
/// The waiter role arrives with the next sign-in, as a Cognito group claim in the id token.</summary>
public sealed class WaitersApi(IAPIRequestContext api) : ApiClientBase(api)
{
    public Task<IAPIResponse> AddWaiterAsync(WaiterRequest request) =>
        Api.PostAsync(Url("/waiters"), new APIRequestContextOptions
        {
            DataString = Serialize(request),
            Headers = Headers(),
        });

    public Task<IAPIResponse> RemoveWaiterAsync(WaiterRequest request) =>
        DeleteAsync(Url("/waiters"), new APIRequestContextOptions
        {
            DataString = Serialize(request),
            Headers = Headers(),
        });
}
