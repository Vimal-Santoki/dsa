using DSA.Api.Common.Iam.Interfaces;

namespace DSA.Api.Common.Iam.Services
{
    internal class IamService : IIamService
    {
        readonly IPermissionService _permissionService;
        readonly IPolicyEvaluator _policyEvaluator;
        public IamService(IPermissionService permissionService, IPolicyEvaluator policyEvaluator)
        {
            _permissionService = permissionService;
            _policyEvaluator = policyEvaluator;
        }
        public async Task<bool> IsAuthorizedAsync(string userId, string action, string resource)
        {
            // 1. Get the Policy
            var policy = await _permissionService.GetPolicyAsync(userId);
            if (policy is null) return false;

            // 2. Evaluate
            return _policyEvaluator.Evaluate(policy, action, resource);
        }
    }
}
