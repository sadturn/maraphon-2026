using System.Globalization;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.StepDefinitions;

/// <summary>
/// Turns the words a step captured into the typed value its step definition wants. Each named
/// transformation is also a custom Cucumber-expression parameter type — a step written with
/// <c>{viewer}</c> only matches the people listed here, so a typo in a feature file is an
/// undefined step rather than a runtime parse error. A transformation's regex must contain its
/// own capture group (Reqnroll passes that group's text to the method).
/// </summary>
[Binding]
public static class StepArgumentTransformations
{
    [StepArgumentTransformation(Name = "viewer", Regex = "(the owner|another customer|the waiter|a guest of the group|a visitor who is not signed in)")]
    public static Viewer ToViewer(string value) => value switch
    {
        "the owner" => Viewer.Owner,
        "another customer" => Viewer.AnotherCustomer,
        "the waiter" => Viewer.Waiter,
        "a guest of the group" => Viewer.GuestOfTheGroup,
        "a visitor who is not signed in" => Viewer.VisitorNotSignedIn,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    [StepArgumentTransformation(Name = "submission_target", Regex = "(group|booking)")]
    public static SubmissionTarget ToSubmissionTarget(string value) =>
        value == "group" ? SubmissionTarget.AGroupThatDoesNotExist : SubmissionTarget.ABookingThatDoesNotExist;

    /// <summary>An amount as a feature file writes it: always two decimals, no currency sign ("23.00").</summary>
    [StepArgumentTransformation(Name = "money", Regex = @"(\d+\.\d{2})")]
    public static decimal ToMoney(string value) => decimal.Parse(value, CultureInfo.InvariantCulture);
}
