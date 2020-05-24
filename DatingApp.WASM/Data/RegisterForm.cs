using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.WASM.Data
{
    public class RegisterForm
    {
        [Required]
        public string Gender { get; set; } = "Male";

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

        [Required(ErrorMessage = "Please repeat the password")]
        [Compare("Password", ErrorMessage = "The two passwords don't match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
    }
}