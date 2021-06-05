using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Core.Domain
{
    public class Entity
    {


        public string Type { get; set; }
        public string Name { get; set; }

        public Address Address { get; set; }
        public Address BillingAddress { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }

        public string ParentName { get; set; }
        public bool IsPrepaid { get; set; }
        //public OtherAddresses OtherAddresses { get; set; }
        public List<CustomField> CustomFields { get; set; }
        // [XmlAttribute]

        // public string GUID { get; set; }
    }
}
