using LtxMagayaCore.Infrastructure.Interfaces;
using ServiceMagaya;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Infrastructure.Services
{
    public class EntitiesMagaya : IEntitiesMagaya
    {
        public CSSoapServiceSoapClient _client = new();
        public string ObtenerEntidades(string dato)
        {
            StartSessionRequest _request = new()
            {
                user = "webadmin",
                pass = "we*LTC134679$$"
            };
            var _session = _client.StartSession(_request);


            GetEntitiesRequest _Getrequest = new GetEntitiesRequest();

            _Getrequest.access_key = _session.access_key;
            _Getrequest.flags = 0x00000000;

            _Getrequest.start_with = dato;
            var response = _client.GetEntities(_Getrequest);
            return response.entity_list_xml;

        }
        public string CrearEntidad(string request)
        {



            StartSessionRequest _request = new()
            {
                user = "webadmin",
                pass = "we*LTC134679$$"
            };
            var _session = _client.StartSession(_request);




            SetEntityRequest ser = new();

            ser.access_key = _session.access_key;
            ser.entity_xml = request;
            var retorno = _client.SetEntity(ser);




            EndSessionRequest _endSession = new()
            {
                access_key = _session.access_key
            };
            _client.EndSession(_endSession);
            return retorno.error_desc;
        }





        public SetParentEntityResponse SetAgent(string cliente)
        {

            StartSessionRequest _request = new()
            {
                user = "webadmin",
                pass = "we*LTC134679$$"
            };
            var _session = _client.StartSession(_request);

            SetParentEntityRequest request = new SetParentEntityRequest();
            request.access_key = _session.access_key;
            request.entity_guid = cliente;
            request.parent_guid = "0EFAE28C-CFAD-4AFA-B0AE-0D48B08EC7D3";
            var retorno = _client.SetParentEntity(request);




            EndSessionRequest _endSession = new()
            {
                access_key = _session.access_key
            };
            _client.EndSession(_endSession);


            return retorno;


        }
    }
}
