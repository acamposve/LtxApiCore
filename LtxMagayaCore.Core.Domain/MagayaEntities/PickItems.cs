using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LtxMagayaCore.Core.Domain
{
    public class PickItems
    {
        [Key]
        public Guid GUID { get; set; }
        public string PickItem { get; set; }
    }


}