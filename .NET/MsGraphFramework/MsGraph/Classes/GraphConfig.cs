using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using MsGraph.GraphModels.Attributes;

namespace MsGraph.Classes
{

    /// <summary>
    /// Encapsulates tenant-specific constants and provides methods for calculating various values which derive from these constants
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GraphConfig
    {

        /// <summary>
        /// This is hack to get around automapper's inability do inject dependencies into profiles. Do not use it anywhere except within an automapper's profile.
        /// </summary>
        public static GraphConfig The;

        #region Json properties
        public string TenantId { get; set; }
        public string GraphAppClientId { get; set; }
        public string GraphAppClientSecret { get; set; }
        public DateTime GraphAppClientSecretExpirationDate { get; set; }
        public string TenantDomain { get; set; }
        public string B2CExtensionAppClientId { get; set; }
        public string B2CExtensionAppObjectId { get; set; }
        #endregion Json properties
        
        #region Calculated

        private string B2CExtensionAppClientIdWithoutDashes => B2CExtensionAppClientId.Replace("-", "");
        private string CustomAttributePrefix => $"extension_{B2CExtensionAppClientIdWithoutDashes}_";

        /// <summary>
        /// Converts short custom attribute name ("ExtnMyProp") (with or without "Extn") to full b2c name ("extension_xxxx...xxx_MyProp")
        /// </summary>
        /// <param name="customAttributeShortName"></param>
        /// <returns></returns>
        public string CustomAttributeFullName(string customAttributeShortName)
        {
            if (customAttributeShortName.StartsWith(GraphConstants.Extn)) customAttributeShortName = customAttributeShortName.Replace(GraphConstants.Extn, "");
            return $"{CustomAttributePrefix}{customAttributeShortName}";
        }

        /// <summary>
        /// Converts full b2c name ("extension_xxxx...xxx_MyProp") into short name ("MyProp") (WITHOUT Extn)
        /// </summary>
        /// <param name="customAttributeFullName"></param>
        /// <returns></returns>
        public string CustomAttributeShortName(string customAttributeFullName)
        {
            return customAttributeFullName.Replace(CustomAttributePrefix, "");
        }


        /// <summary>
        /// Returns top-level (no recursion) json field list for <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<string> B2CPropertyList<T>()
        {
            var type = typeof(T);
            foreach (var prop in type.GetProperties())
            {

                //skipping properties marked with [SkipGraphProperty]
                if (prop.GetCustomAttribute(typeof(SkipGraphPropertyAttribute)) != null) continue;

                //see if json property name is specified explicitely with [JsonPropertyName]
                var attr = prop.GetCustomAttribute(typeof(JsonPropertyNameAttribute));
                if (attr != null) yield return ((JsonPropertyNameAttribute) attr).Name;
                else
                {
                    var name = prop.Name;

                    //is it a custom attribute? get its full name then
                    if (name.StartsWith(GraphConstants.Extn)) yield return CustomAttributeFullName(name);

                    //none of the above? just return property name
                    else yield return name;
                }
            }
        }

        #endregion 

    }
}
