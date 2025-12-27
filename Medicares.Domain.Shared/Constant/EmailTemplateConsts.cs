namespace Medicares.Domain.Shared.Constant
{
    public static class EmailTemplateConsts
    {
        public const string OwnerWelcomeSubject = "Welcome to {0}";

        public const string OwnerWelcomeTitle = "Owner Account Created";

        public const string OwnerWelcomeMainText =
            "Your owner account has been successfully created. " +
            "You can now sign in using your registered email and password to start setting up and managing your stores from the dashboard.";

        public const string OwnerWelcomeSecondaryText =
            "If you need any assistance, our support team is always here to help.";

        public const string OwnerWelcomeButtonText = "Login Here";
    }
}
