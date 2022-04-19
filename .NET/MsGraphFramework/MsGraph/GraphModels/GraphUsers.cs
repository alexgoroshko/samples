using System.Text.Json.Serialization;

namespace MsGraph.GraphModels
{


    
    // ReSharper disable ClassNeverInstantiated.Global
    public class GraphUsers
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string OdataNextLink { get; set; }

        [JsonPropertyName("value")]
        public GraphUser[] Items { get; set; }

        public int Iterations { get; set; }
    }
}
