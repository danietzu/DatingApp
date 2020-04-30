using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(maximumLength: 8,
                      MinimumLength = 4,
                      ErrorMessage = "You must specify a username between 4 and 8 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(maximumLength: 8,
                      MinimumLength = 4,
                      ErrorMessage = "You must specify a password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}