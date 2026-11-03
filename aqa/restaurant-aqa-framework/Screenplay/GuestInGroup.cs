namespace Restaurant_Aqa_Framework.Screenplay;

/// <summary>A guest who joined a group pre-order by invitation, remembered under the guest's name. A guest has no
/// account: <paramref name="SessionToken"/> is what lets them change their own cart.</summary>
public sealed record GuestInGroup(string Name, string SessionId, string SessionToken);
