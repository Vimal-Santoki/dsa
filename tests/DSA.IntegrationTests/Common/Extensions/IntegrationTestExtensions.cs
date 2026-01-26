using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DSA.IntegrationTests.Common.Extensions
{
    internal static class IntegrationTestExtensions
    {

        // Define a private response class inside the extension so no one else needs to know about it
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via JSON Deserialization")]
        private sealed class TokenResponse
        {
            public string? AccessToken { get; set; }
        }

        public static async Task AuthenticateAsync(this HttpClient client, string username = "admin", string password = "password")
        {
            var loginDto = new { Username = username, Password = password };
            var response = await client.PostAsJsonAsync("/connect/token", loginDto);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (result?.AccessToken != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.AccessToken);
            }
        }
    }
}
