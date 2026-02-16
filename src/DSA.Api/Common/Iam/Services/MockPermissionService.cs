using DSA.Api.Common.Iam.Constants;
using DSA.Api.Common.Iam.Interfaces;
using DSA.Api.Common.Iam.Models;

namespace DSA.Api.Common.Iam.Services
{
    internal class MockPermissionService : IPermissionService
    {
        public async Task<PolicyDocument?> GetPolicyAsync(string principalId, CancellationToken cancellationToken = default)
        {
            if (principalId == null) return null;

            if (principalId == "user-123")
            {
                // Admin Policy: Allow Everything
                return new PolicyDocument("2026", new()
                {
                    // Admin Policy: Allow Everything
                    new Statements(Effect.Allow, ["*"], ["*"])
                });
            }

            return new PolicyDocument("2026", new()
            {
                // Read-Only Policy: Allow only read actions on specific resources
                new Statements(Effect.Allow,
                    [AppPermissions.Sorting.List],
                    ["*"]
                ),
                // Deny write actions
                new Statements(Effect.Deny,
                    [AppPermissions.Sorting.Execute],
                    ["*"]
                )
            });
        }
    }
}
