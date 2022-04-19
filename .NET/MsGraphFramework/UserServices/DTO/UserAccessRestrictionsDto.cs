using System.Collections.Generic;
using UserServices.DTO.Interfaces;

namespace UserServices.DTO
{
    public class UserAccessRestrictionsDto : ICompanyRelatedEntityDto, IPreferencesDto
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public bool GdprTermsAccepted { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        /// <summary>
        /// Two letter ISO region name
        /// </summary>
        public string CountryCode { get; set; }
        
        public IEnumerable<string> Preferences { get; set; }
    }
}
