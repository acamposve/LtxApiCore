using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Core.Domain.MagayaEntities
{
    public class estados
    {
        [Key]
        public int id_estado { get; set; }
        public string estado { get; set; }
        public string iso { get; set; }
    }
}
