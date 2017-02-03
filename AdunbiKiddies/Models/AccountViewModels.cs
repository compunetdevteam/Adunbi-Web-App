using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdunbiKiddies.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Your First name is required")]
        [StringLength(40, ErrorMessage = "Your First name is too long")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [Required(ErrorMessage = "Your Middle name is required")]
        [StringLength(40, ErrorMessage = "Your Middle name is too long")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Your last name is required")]
        [StringLength(40, ErrorMessage = "Your last name is too long")]
        public string LastName { get; set; }

        public string Username
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Name { get; set; }

        [Display(Name = "Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Your Address is required")]
        [StringLength(50, ErrorMessage = "Your Address name is too long")]
        public string Address { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class DailySales
    {
        [Display(Name = "Please Select Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
