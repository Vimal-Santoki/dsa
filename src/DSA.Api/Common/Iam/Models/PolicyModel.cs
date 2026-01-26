namespace DSA.Api.Common.Iam.Models
{
    internal sealed record PolicyDocument(string Version, List<Statements> Statements);
    internal sealed record Statements(Effect Effect, List<string> Actions, List<string> Resources);
    internal enum Effect { Allow, Deny }
}
