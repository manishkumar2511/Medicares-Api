namespace Medicares.Domain.Shared.Constant
{
    public static class ApplicationConsts
    {
        public const string Country = "India";
        public const string PlatformOwnerId = "11111111-1111-1111-1111-111111111111"; 

        public static class ConfigKeys
        {
            public const string Credentials = "Credentials";
            public const string JwtSettings = "JwtSettings";
        }

        public static class SignalR
        {
            public const string AccessTokenQuery = "access_token";
            public const string HubPath = "/hubs";
        }

        public static class Policies
        {
            public const string RequireMfa = "RequireMfa";
        }

        public static class Claims
        {
            public const string Mfa = "amr";
        }

        public static class ClaimValues
        {
            public const string True = "mfa";
        }
    }
}
