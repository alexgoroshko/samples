namespace UserServices
{
    public static class UserConstants
    {
        public static readonly string ExtensionClaimPrefix = "extension_";

        public static string ExtensionClaimName(string shortName)
        {
            return ExtensionClaimPrefix + shortName;
        }
    }
}
