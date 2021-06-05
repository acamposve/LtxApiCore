using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain.Accounts
{
    public class RegisterRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }


        public string identificationNumber { get; set; }
        public string address { get; set; }
        public string sector { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
    }
}