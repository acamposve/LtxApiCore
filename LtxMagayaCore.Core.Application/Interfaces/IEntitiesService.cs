using LtxMagayaCore.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Core.Application.Interfaces
{
    public interface IEntitiesService
    {
        string CrearEntidad(Account account);
        string  ObtenerEndidades(string request);
    }
}
