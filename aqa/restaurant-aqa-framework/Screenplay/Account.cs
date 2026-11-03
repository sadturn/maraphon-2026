namespace Restaurant_Aqa_Framework.Screenplay;

/// <summary>Someone this scenario registered and signed in — remembered under who they are ("owner", "waiter",
/// "another customer") so a later step can act as them through the service or sign in with them on screen.</summary>
public sealed record Account(string Email, string Password, string IdToken);
