using System;
using System.Linq;
using System.Text.Json.Serialization;
using MsGraph.Classes;
using MsGraph.GraphModels.Attributes;

namespace MsGraph.GraphModels
{


    //ALL PROPERTIES HERE https://docs.microsoft.com/en-us/graph/api/resources/user?view=graph-rest-1.0
    
    //_todo ideally, to make the lib reusable, custom attributes should not be part of this. 

    public class GraphUser
    {
        
        #region Properties
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("passwordProfile")]
        public GraphUserPasswordprofile PasswordProfile { get; set; }

        [JsonPropertyName("passwordPolicies")]
        public string PasswordPolicies { get; set; }

        [JsonPropertyName("accountEnabled")]
        public bool? AccountEnabled { get; set; }

        [JsonPropertyName("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTime? CreatedDateTime { get; set; }

        [JsonPropertyName("lastPasswordChangeDateTime")]
        public DateTime? LastPasswordChangeDateTime { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("identities")]
        public GraphUserIdentity[] Identities { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }
        
        #endregion Properties

        #region CustomAttributes
        //Custom attribute names must start with "Extn" followed by the "short name".
        //During [de]serialization names are replaced with b2c internal names (see GraphConfig and GraphJson)


        public string ExtnEnvName { get; set; }
        public string ExtnRole { get; set; }
        public int? ExtnCompanyId { get; set; }
        public bool? ExtnIsCompanyContactPerson { get; set; }
        public bool? ExtnGdprTermsAccepted { get; set; }
        
        public string ExtnCountryCode { get; set; }

        #endregion CustomAttributes

        #region Calculated

        [JsonIgnore]
        [SkipGraphProperty]
        public bool IsB2CUser
        {
            get
            {
                var b2CIdentity = Identities?.FirstOrDefault(i => i.SignInType == GraphConstants.SignInTypeEmailAddress)?.IssuerAssignedId;
                return b2CIdentity != null;
            }
        }

        #endregion Calculated

    }


















}
