using Flurl;

namespace MsGraph.Classes
{
    public static class GraphConstants
    {
        public static readonly string Extn = "Extn";

        public static readonly string SignInTypeEmailAddress = "emailAddress";

        public static readonly string DisablePasswordExpiration = "DisablePasswordExpiration";

        private static readonly string GraphBaseUrl = "https://graph.microsoft.com/v1.0/";
        //private static readonly string GraphBaseUrl = "https://graph.microsoft.com/beta/";
        public static string GraphUsersUrl => Url.Combine(GraphBaseUrl, "/users");
        public static string GraphApplicationsUrl => Url.Combine(GraphBaseUrl, "/applications");



    }
}
