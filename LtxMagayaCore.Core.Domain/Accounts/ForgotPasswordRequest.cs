using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}