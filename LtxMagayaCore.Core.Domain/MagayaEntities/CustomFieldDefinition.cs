using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain
{

    public class CustomFieldDefinition
    {


        public string Type { get; set; }
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string InternalUse { get; set; }
        public string BuildReports { get; set; }
        public string Inactive { get; set; }
        public string IsCalculated { get; set; }
        public string IsReadOnly { get; set; }
        public LookupItems LookupItems { get; set; }
        public List<PickItems> PickItems { get; set; }
    }

}