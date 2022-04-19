using System;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MsGraph.GraphModels;

namespace MsGraph.Classes
{


    /// <summary>
    /// Provides runtime resolution of property names for b2c custom attributes by applying <see cref="GraphJsonNamingPolicy"/>
    /// </summary>
    public class GraphJson
    {
        private readonly GraphConfig _graphConfig;

        public GraphJson(GraphConfig graphConfig)
        {
            _graphConfig = graphConfig;
        }
        
        public T Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json,  new JsonSerializerOptions {  PropertyNamingPolicy = new GraphJsonNamingPolicy(_graphConfig) });
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Unable to deserialize", nameof(json), e);
            }
        }


        /// <summary>
        /// Produces json optimized for consuming by graph api (including patch) and also for viewing
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string Serialize(object o)
        {
            try
            {
                //ignoring nulls is good for patch
                //indents are good for human eye
                return JsonSerializer.Serialize(o, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new GraphJsonNamingPolicy(_graphConfig),
                    IgnoreNullValues = true,
                    WriteIndented = true
                });
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Unable to serialize: [{o.GetType()}]", e);
            }
        }


    }

}
