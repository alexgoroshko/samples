using System.Text.Json.Serialization;

namespace MsGraph.GraphModels
{

    // ReSharper disable ClassNeverInstantiated.Global


    /// <summary>
    /// Represents /application/{id}/extensionProperties response json
    /// </summary>
    public class GraphCustomAttributes
    {
        [JsonPropertyName("odatacontext")]
        public string OdataContext { get; set; }

        [JsonPropertyName("value")]
        public GraphCustomAttribute[] Items { get; set; }
    }
}
