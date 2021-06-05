using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Core.Domain.MagayaEntities
{
    public class ciudades
    { 
        [Key]
        public int id_ciudad { get; set; }
        public int id_estado { get; set; }
        public string ciudad { get; set; }
        public byte capital { get; set; }
    }
}
