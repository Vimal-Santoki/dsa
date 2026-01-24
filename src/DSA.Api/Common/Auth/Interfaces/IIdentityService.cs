using DSA.Api.Common.Auth.Dto;

namespace DSA.Api.Common.Auth.Interfaces
{
    internal interface IIdentityService
    {
        Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
