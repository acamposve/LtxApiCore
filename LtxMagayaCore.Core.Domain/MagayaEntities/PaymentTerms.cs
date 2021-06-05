using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain
{

    public class PaymentTerms
    {
        [Key]
        public Guid GUID { get; set; }
        public string Description { get; set; }
        public string NetDueDays { get; set; }
        public string DiscountPercentage { get; set; }
        public string DiscountPaidDays { get; set; }

    }
}