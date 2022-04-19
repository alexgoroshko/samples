using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserServices.Enum;

namespace UserServices.DTO
{
    public abstract class IncomingUserDto : IValidatableObject
    {
        public string FirstName { get; set; }

        [MinLength(2)]
        public string LastName { get; set; }

        [MaxLength(63)]
        public string PhoneNumber { get; set; }
        public string PreferredLanguage { get; set; }
        public int? CompanyId { get; set; } 
        public bool? IsCompanyContactPerson { get; set; }
        public string Role { get; set; }
        /// <summary>
        /// Two letter ISO region name
        /// </summary>
        public string CountryCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            UserRole userRole = default;
            
            if (Role != null && !System.Enum.TryParse(Role, false, out userRole))
            {
                results.Add(new ValidationResult($"Invalid user role: {Role}"));
            }

            if (userRole is UserRole.LocalCountryAdmin or UserRole.LocalCountryEmployee && string.IsNullOrEmpty(CountryCode))
            {
                results.Add(new ValidationResult($"CountryCode is required for {Role} role"));
            }
            
            return results;
        }


    }
}
