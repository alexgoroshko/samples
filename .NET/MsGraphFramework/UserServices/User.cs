using System;

namespace UserServices
{
  public class User 
    {

        /*
         * when adding new fields make sure they are
         1     1) mapped in *UserDto classes
         *     2) mapped in GraphUser
         *     3) handled by Automapper (there are 2 separate profiles for the above
         */

        public string Id { get; set; }

        //see comment about this in AzureMapperProfile
        public bool? AccountEnabled { get; set; }
        public string EmailLogin { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string PreferredLanguage { get; set; }
        public DateTime? LastPasswordChangeDateTime { get; set; }
        public string DisplayName { get; set; }
        
        public string EnvName { get; set; }
        public string Role { get; set; }
        public bool? GdprTermsAccepted { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsCompanyContactPerson { get; set; }
        /// <summary>
        /// Two letter ISO region name
        /// </summary>
        public string CountryCode { get; set; }
    }
}
