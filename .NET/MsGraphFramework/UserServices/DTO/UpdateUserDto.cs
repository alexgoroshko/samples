using System.ComponentModel.DataAnnotations;

namespace UserServices.DTO
{
    public class UpdateUserDto : IncomingUserDto
    {
        [Required]
        [MinLength(36)]
        [MaxLength(36)]
        public string Id { get; set; }

        [EmailAddress]
        public string EmailLogin { get; set; }
    }
}
