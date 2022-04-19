using System;
using System.Collections.Generic;
using System.Text;

namespace MsGraph.GraphModels
{
    // ReSharper disable InconsistentNaming
    public class GraphTokenResponse
    {
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }
    }




}
