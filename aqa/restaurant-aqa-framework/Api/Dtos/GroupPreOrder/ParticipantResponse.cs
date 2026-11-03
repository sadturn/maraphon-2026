using System.Text.Json.Serialization;

namespace Restaurant_Aqa_Framework.Api.Dtos.GroupPreOrder;

public sealed record ParticipantResponse(
    string? Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] ParticipantType Type,
    string Name,
    IReadOnlyList<ParticipantItemResponse> Items,
    decimal Total,
    bool IsSubmitted);
