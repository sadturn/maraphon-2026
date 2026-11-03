using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

namespace Restaurant_Aqa_Framework.Api;

/// <summary>The group pre-order of a reservation for several guests. Two kinds of caller use it: the owner (a
/// signed-in customer, Cognito id token) and the guests, who have no account — a guest's calls carry the session
/// token it got on joining, and the backend checks that token itself (the gateway lets those routes through).</summary>
public sealed class GroupPreOrderApi(IAPIRequestContext api) : ApiClientBase(api)
{
    // ---- the owner ----

    public Task<IAPIResponse> GetInviteTokenAsync(string reservationId, string? bearerToken = null) =>
        GetAsync(Url($"/reservations/{reservationId}/invite-token"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetOwnerSessionIdAsync(string reservationId, string? bearerToken = null) =>
        GetAsync(Url($"/reservations/{reservationId}/guest-session-id"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetParticipantsAsync(string groupReservationId, string? bearerToken = null) =>
        GetAsync(Url($"/guest/groups/{groupReservationId}/sessions"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> SubmitAsync(string groupReservationId, string reservationId, string? bearerToken = null) =>
        Api.PostAsync(Url($"/group-reservations/{groupReservationId}/submit"), new APIRequestContextOptions
        {
            DataString = Serialize(new SubmitGroupPreOrderRequest(reservationId)),
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetStatusAsync(string groupReservationId) =>
        GetAsync(Url($"/guest/groups/{groupReservationId}/status"));

    // ---- a guest ----

    public Task<IAPIResponse> GetInviteAsync(string inviteToken) => GetAsync(Url($"/guest/invites/{inviteToken}"));

    public Task<IAPIResponse> JoinAsync(JoinGroupRequest request) =>
        Api.PostAsync(Url("/guest/sessions"), new APIRequestContextOptions
        {
            DataString = Serialize(request),
            Headers = Headers(),
        });

    public Task<IAPIResponse> AddToCartAsync(string guestSessionId, GuestCartItemRequest item, string sessionToken) =>
        Api.PostAsync(Url($"/guest/sessions/{guestSessionId}/dishes"), new APIRequestContextOptions
        {
            DataString = Serialize(item),
            Headers = Headers(sessionToken),
        });

    // Not retried like other DELETEs: it takes a quantity away, so repeating it after a 500 could take it twice.
    public Task<IAPIResponse> RemoveFromCartAsync(string guestSessionId, GuestCartItemRequest item, string sessionToken) =>
        Api.DeleteAsync(Url($"/guest/sessions/{guestSessionId}/dishes"), new APIRequestContextOptions
        {
            DataString = Serialize(item),
            Headers = Headers(sessionToken),
        });

    public Task<IAPIResponse> SubmitCartAsync(string guestSessionId, string sessionToken) =>
        Api.PostAsync(Url($"/guest/sessions/{guestSessionId}/submit"), new APIRequestContextOptions
        {
            Headers = Headers(sessionToken),
        });
}
