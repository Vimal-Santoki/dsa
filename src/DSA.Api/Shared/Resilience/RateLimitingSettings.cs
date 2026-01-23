namespace DSA.Api.Resilience;

internal sealed class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";

    public int PermitLimit { get; set; } = 100;
    public int WindowInSeconds { get; set; } = 60;
    public int SegmentsPerWindow { get; set; } = 6;
}
