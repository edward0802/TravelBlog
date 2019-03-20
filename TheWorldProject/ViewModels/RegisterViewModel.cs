using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorldProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User's Email")]
        public string Email { get; set; }
        [Required]
        [MinLength(5)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords are different")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.*\s).{8,24}$", ErrorMessage = "Password must be at least 8 characters and contain digits, upper and lower case and non alphanumeric symbols")]
        public string ConfirmPassword { get; set; }

    }
}
