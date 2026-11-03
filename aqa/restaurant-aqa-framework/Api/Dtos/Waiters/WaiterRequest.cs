namespace Restaurant_Aqa_Framework.Api.Dtos.Waiters;

/// <summary>Body of POST and DELETE /waiters: which registered user works at which location.</summary>
public sealed record WaiterRequest(string Email, string LocationId);
