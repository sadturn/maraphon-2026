namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>POST /guest/sessions — a guest has no account: <paramref name="SessionToken"/> is the bearer token
/// for every call on that guest's own cart.</summary>
public sealed record GuestSessionResponse(string SessionId, string SessionToken);
