using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorldProject.ViewModels
{
    public class ForgotPassViewModel
    {
        [Required]
        [Display(Name = "Enter your email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
