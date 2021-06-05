using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Core.Domain.MagayaEntities
{
    public class NuevasEntidades
    {
        public Address NuevaDireccion(Client Cons, string ciudad, string estado, string nombre, string addressOrigin)
        {
            Address address = new Address
            {
                Street = addressOrigin,
                City = ciudad.ToUpper(),
                State = estado.ToUpper(),
                Country = "VENEZUELA",
                ContactName = nombre.ToUpper(),
                ContactPhone = Cons.Phone,
                ContactEmail = Cons.Email.ToUpper(),
            };
            return address;

        }
        public LookupItems lookUp(string tipo)
        {
            LookupItems li1 = new LookupItems
            {
                LookupItem = tipo
            };
            return li1;

        }
        public CustomFieldDefinition DefinicionCampoPersonalizado(string tipo, string nombreInterno, string nombreMostrar, LookupItems li)
        {
            CustomFieldDefinition cfd = new CustomFieldDefinition
            {
                Type = tipo,
                InternalName = nombreInterno,
                DisplayName = nombreMostrar,
                InternalUse = "false",
                BuildReports = "true",
                Inactive = "false",
                IsCalculated = "false",
                IsReadOnly = "false"
            };
            if (li != null)
                cfd.LookupItems = li;

            return cfd;
        }
        public CustomField CampoPersonalizado(string valor, CustomFieldDefinition cfd)
        {
            CustomField cf = new CustomField
            {
                CustomFieldDefinition = cfd,
                Value = valor
            };
            return cf;
        }



        public Entity NuevaEntidad(string guid, OtherAddresses oa, string tipo, string nombre, Address address, Client Cons, List<CustomField> lcf)
        {

            Entity NuevaEntidad = new Entity();

            //if (guid != null)
            //{


            //    NuevaEntidad.GUID = guid;





            //}




            NuevaEntidad.Type = "Client";
            NuevaEntidad.Name = nombre.ToUpper();

            NuevaEntidad.Address = address;
            NuevaEntidad.BillingAddress = address;
            NuevaEntidad.Email = Cons.Email.ToUpper();
            NuevaEntidad.Phone = Cons.Phone;
            NuevaEntidad.ContactFirstName = Cons.Name.ToUpper();


            NuevaEntidad.IsPrepaid = false;
            NuevaEntidad.ParentName = "LTX C.A";
            NuevaEntidad.CustomFields = lcf;




            return NuevaEntidad;

        }


    }
}
