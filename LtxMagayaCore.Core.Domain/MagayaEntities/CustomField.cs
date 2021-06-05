using System;
using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain
{


    public class CustomField
    {


        public CustomFieldDefinition CustomFieldDefinition { get; set; }
        public string Value { get; set; }

    }
}