using System.Text.Json.Serialization;

namespace MsGraph.GraphModels
{
    public class GraphUserIdentity
    {
        [JsonPropertyName("signInType")]
        public string SignInType { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("issuerAssignedId")]
        public string IssuerAssignedId { get; set; }
    }
}