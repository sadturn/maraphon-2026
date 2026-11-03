using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>What someone who looked at a group pre-order got to see.</summary>
public enum GroupPreOrderState
{
    /// <summary>The participants, their dishes and the totals.</summary>
    Shown,

    /// <summary>Allowed to look, but there is nothing to show yet.</summary>
    Empty,

    NotFound,

    /// <summary>Signed in, but not the owner of this group.</summary>
    Refused,

    /// <summary>Not signed in (a guest has no account, so a guest is not signed in either).</summary>
    SignInRequired,

    /// <summary>Anything else; <see cref="GroupPreOrderView.Evidence"/> says what.</summary>
    Failed,
}

/// <summary>A participant as the owner sees them; <see cref="Name"/> is the guest's name (the owner's is not compared).</summary>
public sealed record ParticipantView(string Name, bool IsOwner, IReadOnlyList<CartLine> Dishes, decimal Subtotal, bool IsSubmitted);

/// <summary>The group pre-order as one look at it found it, the same shape whether it was read from the service or off
/// the screen. <see cref="Evidence"/> is the response body or the screen's text, for assertion messages.</summary>
public sealed record GroupPreOrderView(
    GroupPreOrderState State,
    IReadOnlyList<ParticipantView> Participants,
    decimal GrandTotal,
    string Evidence)
{
    public IReadOnlyList<ParticipantView> Guests => Participants.Where(p => !p.IsOwner).ToList();

    public ParticipantView? Owner => Participants.FirstOrDefault(p => p.IsOwner);
}
