namespace AshenWar.Application.ValueObjects;

public sealed record SubmitOrdersResult(bool Success, string? Error)
{
    public static SubmitOrdersResult Ok() => new(true, null);
    public static SubmitOrdersResult Fail(string error) => new(false, error);
}