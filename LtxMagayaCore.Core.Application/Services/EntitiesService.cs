using LtxMagayaCore.Core.Application.Interfaces;
using LtxMagayaCore.Core.Domain;
using LtxMagayaCore.Core.Domain.MagayaEntities;
using LtxMagayaCore.Infrastructure.Data;
using LtxMagayaCore.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LtxMagayaCore.Core.Application.Services
{
    public class EntitiesService : IEntitiesService
    {
        private readonly IEntitiesMagaya _entitiesMagaya;
        private readonly DataContext _context;



        public EntitiesService(IEntitiesMagaya entitiesMagaya, DataContext context)
        {
            _entitiesMagaya = entitiesMagaya;
            _context = context;
        }
        public string CrearEntidad(Account account) {


            Client _client = new Client();
            _client.Name = account.FirstName + " " + account.LastName;




            _client.Phone = account.AccountPhone;
            _client.Email = account.Email;

            var ciudadBD = _context.ciudades.FirstOrDefault(x => x.id_ciudad == int.Parse(account.State));
            var estadoBD = _context.estados.FirstOrDefault(x => x.id_estado == ciudadBD.id_estado);


            var ciudad = ciudadBD.ciudad;
            var estado = estadoBD.estado;
            var direccion = account.Address;
            //     var nombre = "IVETH" + " " + "LORDS";
            var sector = account.Sector;

            NuevasEntidades ne = new NuevasEntidades();
            var address = ne.NuevaDireccion(_client, ciudad, estado, _client.Name, direccion);
            var lookupitems = ne.lookUp("ForwardingAgent");

            var cfd = ne.DefinicionCampoPersonalizado("Lookup", "agent", "Agent", lookupitems);
            var cf = ne.CampoPersonalizado("LTX", cfd);
            var cfd2 = ne.DefinicionCampoPersonalizado("PickList", "sector", "SECTOR", null);
            var cf2 = ne.CampoPersonalizado(sector, cfd2);



            var cfd3 = ne.DefinicionCampoPersonalizado("Logical", "online_creation", "ONLINE_CREATION", null);
            var cf3 = ne.CampoPersonalizado("true", cfd3);

            List<CustomField> lcf = new List<CustomField>
                {
                    cf,
                    cf2,
                    cf3
                };

            var NuevaEntidad = ne.NuevaEntidad("", null, "Client", _client.Name, address, _client, lcf);


            var objeto = GetXMLFromObject(NuevaEntidad);
            string valor = objeto.Trim();





            var response = _entitiesMagaya.CrearEntidad(valor);




            var clientemagaya = _entitiesMagaya.ObtenerEntidades(_client.Name);
            XNamespace ns = "http://www.magaya.com/XMLSchema/V1";
            var xmlConsignatarios = XDocument.Parse(clientemagaya);
            var listaC = (from p in xmlConsignatarios.Descendants(ns + "Client") select p).ToList();



            var clienteguid = listaC[0].FirstAttribute.Value;
            account.GUID = clienteguid;
            _context.Accounts.Update(account);
            _context.SaveChanges();
            _entitiesMagaya.SetAgent(clienteguid);

            return response;


        }
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }


        private static string GetXMLFromObject(object o)
        {
            StringWriter sw = new Utf8StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType(), "http://www.magaya.com/XMLSchema/V1");
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, "http://www.magaya.com/XMLSchema/V1");
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o, namespaces);
            }
            catch (Exception ex)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }

        public string ObtenerEndidades(string request)
        {
            return _entitiesMagaya.ObtenerEntidades(request);
        }
    }
}
