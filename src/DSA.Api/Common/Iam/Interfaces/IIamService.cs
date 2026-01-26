namespace DSA.Api.Common.Iam.Interfaces
{
    internal interface IIamService
    {
        Task<bool> IsAuthorizedAsync(string userId, string action, string resource);
    }
}
