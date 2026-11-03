using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

/// <summary>Whether the owner's screen still offers Submit Pre-Order. Ask once the screen has settled (after the outcome
/// of a submission): the answer is read straight away, not waited for.</summary>
public sealed class TheSubmitButtonIsShown : IQuestion<bool>
{
    public async Task<bool> AnsweredByAsync(Actor actor) =>
        await new GroupOrderPage(actor.LookingAtThePage()).SubmitButton.CountAsync() > 0;
}
