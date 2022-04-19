using System.ComponentModel.DataAnnotations;

namespace UserServices.DTO
{
    public class CreateUserDto : IncomingUserDto
    {
        [Required]
        [EmailAddress]
        public string EmailLogin { get; set; }
    }
}
