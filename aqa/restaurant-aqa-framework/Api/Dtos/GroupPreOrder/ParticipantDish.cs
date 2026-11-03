namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

/// <summary>The part of a dish a participant's cart line needs; the backend sends the whole dish.</summary>
public sealed record ParticipantDish(string Id, string Name, decimal Price);
