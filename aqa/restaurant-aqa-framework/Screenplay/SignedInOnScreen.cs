namespace Restaurant_Aqa_Framework.Screenplay;

/// <summary>Who is signed in on the scenario's browser (<see langword="null"/> once they signed out), so a scenario
/// where two people take turns on the same screen — the owner, then the waiter — signs the first one out.</summary>
public sealed record SignedInOnScreen(string? Email);
