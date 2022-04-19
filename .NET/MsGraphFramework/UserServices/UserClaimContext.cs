using System;
using UserServices.Enum;

namespace UserServices
{
  public class UserClaimContext
  {
    public string UserId { get; set; }
    public UserRole Role { get; set; }
    
    public bool? GdprTermsAccepted { get; set; }
    
    public int CompanyId { get; set; }
    
    public bool IsCompanyContactPerson { get; set; }
    
    public string CountryCode { get; set; }
    
    public Guid UserIdGuid { get; set; }
  }
}