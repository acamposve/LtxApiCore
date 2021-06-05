using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain
{

     public class BillingAddress
    {
        [Key]
        public int id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string ContactFax { get; set; }
    }
}