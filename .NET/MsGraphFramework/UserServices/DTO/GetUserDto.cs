using UserServices.DTO.Interfaces;

namespace UserServices.DTO
{
    public class GetUserDto : ICompanyRelatedEntityDto
    {
        public string Id { get; set; }
        public string EmailLogin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool? AccountEnabled { get; set; }
        public string PreferredLanguage { get; set; }
        
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public bool GdprTermsAccepted { get; set; }
        public int CompanyId { get; set; }
        public bool IsCompanyContactPerson { get; set; }
        public string CompanyName { get; set; }
        /// <summary>
        /// Two letter ISO region name
        /// </summary>
        public string CountryCode { get; set; }
    }
}
