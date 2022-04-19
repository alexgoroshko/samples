using System.Collections.Generic;

namespace UserServices.DTO.Interfaces
{
  public interface IPreferencesDto
  {
     IEnumerable<string> Preferences { get; set; }
  }
}