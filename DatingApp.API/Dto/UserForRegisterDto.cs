using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dto
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Gender { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        [Required]
        public string DateOfBirth { get; set; }
        
        [Required]
        public string KnownAs { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [StringLength(12,MinimumLength=4,ErrorMessage="Password should be between 4 to 12 charactor.")]
        public string Password { get; set; }

        [Required]
        [StringLength(12,MinimumLength=4,ErrorMessage="Password should be between 4 to 12 charactor.")]
        public string ConfirmPassword { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }

    }
}