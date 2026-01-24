namespace DSA.Api.Common.Auth.Dto
{
    internal record TokenResponse(string AccessToken, int ExpiresIn);
}
