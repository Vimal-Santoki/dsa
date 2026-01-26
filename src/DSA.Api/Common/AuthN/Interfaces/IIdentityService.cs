using DSA.Api.Common.AuthN.Dto;

namespace DSA.Api.Common.AuthN.Interfaces
{
    internal interface IIdentityService
    {
        Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
