namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>How submitting the group pre-order turned out: whether it was sent, and what the submitter was told.
/// <see cref="Status"/> is only known when the submission went through the service.</summary>
public sealed record SubmissionOutcome(bool Sent, int? Status, string Message, string Evidence);

/// <summary>The outcome of a submission through the service, read from the scenario's last response.</summary>
public sealed class TheSubmissionOutcome : IQuestion<SubmissionOutcome>
{
    public Task<SubmissionOutcome> AnsweredByAsync(Actor actor)
    {
        var response = actor.TheLastResponse();
        return Task.FromResult(new SubmissionOutcome(
            response.Succeeded, response.Status, response.ErrorMessage, $"{response.Status}: {response.Body}"));
    }
}
