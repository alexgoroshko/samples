using System;
using System.Text.Json.Serialization;

namespace MsGraph.GraphModels
{
    // ReSharper disable ClassNeverInstantiated.Global


    /// <summary>
    /// Represents graph api error response json
    /// </summary>
    public class GraphError
    {
        [JsonPropertyName("error")]
        public GraphErrorError Error { get; set; }
    }

    public class GraphErrorError
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("innerError")]
        public GraphInnerError InnerError { get; set; }
    }

    public class GraphInnerError
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("requestid")]
        public string RequestId { get; set; }

        [JsonPropertyName("clientrequestid")]
        public string ClientRequestId { get; set; }
    }

}
