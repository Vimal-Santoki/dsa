using DSA.Api.Common.Iam.Interfaces;
using DSA.Api.Common.Iam.Models;

namespace DSA.Api.Common.Iam.Services
{
    internal class PolicyEvaluator : IPolicyEvaluator
    {
        public bool Evaluate(PolicyDocument policyDocument, string action, string resource)
        {

            if (policyDocument is null || policyDocument.Statements is null) return false;

            var isAllowed = false;

            foreach (var statement in policyDocument.Statements)
            {
                var actionMatches = IsMatch(statement.Actions, action);
                var resourceMatches = IsMatch(statement.Resources, resource);

                if (actionMatches && resourceMatches)
                {
                    if (statement.Effect == Effect.Deny)
                    {
                        return false; // Explicit deny
                    }

                    if (statement.Effect== Effect.Allow)
                    {
                        isAllowed = true; // Mark as allowed, but continue checking for denies
                    }
                }
            }

            return isAllowed;
        }

        static bool IsMatch(List<string> configuredValue, string requestedValue)
        {
            if (configuredValue is null || requestedValue is null) return false;

            foreach (var value in configuredValue)
            {
                if (value== "*" || value.Equals(requestedValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
