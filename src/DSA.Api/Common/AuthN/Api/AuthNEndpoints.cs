namespace DSA.Api.Common.AuthN.Api
{
    internal static class AuthNEndpoints
    {
        public static void MapAuthNEndpoints(this WebApplication app)
        {
            app.MapTokenEndpoints();
        }
    }
}
