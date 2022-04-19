using System.Text.Json.Serialization;

namespace MsGraph.GraphModels
{
    public class GraphUserPasswordprofile
    {
        [JsonPropertyName("forceChangePasswordNextSignIn")]
        public bool ForceChangePasswordNextSignIn { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}