using AutoMapper;
using LtxMagayaCore.Core.Domain;
using LtxMagayaCore.Core.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LtxMagayaCore.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            CreateMap<Account, AccountResponse>();

            CreateMap<Account, AuthenticateResponse>();

            CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(src => src.Password))



                .ForMember(dest => dest.Id, opt => opt.Ignore())

.ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
.ForMember(dest => dest.Verified, opt => opt.Ignore())
.ForMember(dest => dest.ResetToken, opt => opt.Ignore())
.ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
.ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
.ForMember(dest => dest.Created, opt => opt.Ignore())
.ForMember(dest => dest.Updated, opt => opt.Ignore())
.ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

            CreateMap<CreateRequest, Account>();

            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));
        }
    }
}
