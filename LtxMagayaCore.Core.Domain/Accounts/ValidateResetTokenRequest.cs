using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}