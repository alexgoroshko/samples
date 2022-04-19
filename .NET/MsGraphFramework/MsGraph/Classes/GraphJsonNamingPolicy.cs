using System.Text.Json;

namespace MsGraph.Classes
{
    public class GraphJsonNamingPolicy : JsonNamingPolicy
    {

        private readonly GraphConfig _config;

        public GraphJsonNamingPolicy(GraphConfig config)
        {
            _config = config;
        }

        public override string ConvertName(string name)
        {
            //This works both ways: when serializing and when deserializing.
            //"name" is the property name in c# class, return value is property name in json
            if (name.StartsWith(GraphConstants.Extn))
            {
                return _config.CustomAttributeFullName(name);
            }
            return name;
        }
    }
}