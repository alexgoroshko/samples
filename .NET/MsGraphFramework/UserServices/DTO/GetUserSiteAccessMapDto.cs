using System;

namespace UserServices.DTO
{
  public class GetUserSiteAccessMapDto
  {
    public string UserId { get; set; }
        public int SiteId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SiteName { get; set; }
        public string UserName { get; set; }
  }
}