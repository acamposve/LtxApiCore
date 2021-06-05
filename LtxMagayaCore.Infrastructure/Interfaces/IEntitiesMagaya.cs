using ServiceMagaya;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Infrastructure.Interfaces
{
    public interface IEntitiesMagaya
    {
        string ObtenerEntidades(string dato);
        string CrearEntidad(string request);
        SetParentEntityResponse SetAgent(string cliente);
    }
}
