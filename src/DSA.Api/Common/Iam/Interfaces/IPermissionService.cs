using DSA.Api.Common.Iam.Models;

namespace DSA.Api.Common.Iam.Interfaces
{
    internal interface IPermissionService
    {
        Task<PolicyDocument?> GetPolicyAsync(string principalId, CancellationToken cancellationToken = default);
    }
}
