namespace DSA.Api.Common.AuthN.Dto
{
    internal record TokenResponse(string AccessToken, int ExpiresIn);
}
