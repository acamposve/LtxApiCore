using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain
{
    public class Client
    {
        [Key]
        public Guid GUID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public Address Address { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string ParentName { get; set; }
        public PaymentTerms PaymentTerms { get; set; }
        public string IsPrepaid { get; set; }
        public string MobilePhone { get; set; }
        public string Phone { get; set; }
        public string EntityID { get; set; }
        public CustomFields CustomFields { get; set; }
        public string Fax { get; set; }
        public string ContactFirstName { get; set; }
        public string TransactionDueDays { get; set; }

    }
}