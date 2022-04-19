using System.Text.Json.Serialization;

namespace MsGraph.GraphModels
{
    public class GraphCustomAttribute
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deletedDateTime")]
        public object DeletedDateTime { get; set; }

        [JsonPropertyName("appDisplayName")]
        public string AppDisplayName { get; set; }

        [JsonPropertyName("dataType")]
        public string DataType { get; set; }

        [JsonPropertyName("isSyncedFromOnPremises")]
        public bool IsSyncedFromOnPremises { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("targetObjects")]
        public string[] TargetObjects { get; set; }
    }
}