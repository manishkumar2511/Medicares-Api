using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Auth;

public class AuthGroup : Group
{
    public AuthGroup()
    {
        base.Configure(
            "auth",
            ep =>
            {
                ep.Description(x => x.Produces(401)
                .WithTags(AuthApiRoutes.TagName));
            });
    }

    public static class AuthApiRoutes
    {
        public const string TagName = "Auth";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string Verify2FACode = "verify-2fa";
        public const string Resend2FACode = "resend-2fa";
        public const string GetStarted = "get-started";
    }

    public static class AuthConsts
    {
        public const string LoginSummary = "User Login";
        public const string LoginDescription = "Authenticate user and return JWT token";

        public const string ResendMfaSummary = "Resend MFA Code";
        public const string ResendMfaDescription = "Resend the MFA code to the user's email";

        public const string VerifyMFASummary = "Verify MFA Code";
        public const string VerifyMFADescription = "Verify the 2FA code and return JWT token";

        public const string UserLogoutSummary = "User Logout";
        public const string UserLogOutDescription = "Logout the current user";

        public const string OwnerRegistrationSummary = "Owner Registration";
        public const string OwnerRegistrationDescription = "Create a new Owner account and initial User";
    }

    // Messages
    public static class AuthMessages
    {
        public const string LoginSuccessful = "Login successful";
        public const string AuthenticationSuccess = "Authentication successful";
        public const string RegistrationSuccessful = "Registration successful";
        public const string RegistrationFailed = "Registration failed";
        public const string RegistrationLoginFailed = "Registration successful but login failed: ";
        public const string FailedToSendMFA = "Failed to send MFA code";
        public const string MFACodeSent = "MFA code sent";
        public const string LogOutFailed = "Logout failed";
        public const string LogOutSuccess = "Logout successful";
        public const string OwnerEmailAlreadyExists = "Owner associated with this email already exists";
    }
}
