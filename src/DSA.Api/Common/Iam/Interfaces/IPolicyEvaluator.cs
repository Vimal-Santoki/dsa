using DSA.Api.Common.Iam.Models;

namespace DSA.Api.Common.Iam.Interfaces
{
    internal interface IPolicyEvaluator
    {
        bool Evaluate(PolicyDocument policyDocument, string action, string resource);
    }
}
