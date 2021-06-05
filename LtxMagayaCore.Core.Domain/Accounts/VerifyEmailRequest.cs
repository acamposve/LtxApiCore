using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}