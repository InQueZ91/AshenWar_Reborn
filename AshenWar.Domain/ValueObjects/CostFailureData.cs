namespace Domain.ValueObjects;

public sealed record CostFailureData(string ResourceType, int Required, int Available);