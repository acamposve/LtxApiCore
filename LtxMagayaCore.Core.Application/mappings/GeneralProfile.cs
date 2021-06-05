using AutoMapper;
using LtxMagayaCore.Core.Domain;
using LtxMagayaCore.Core.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Core.Application.mappings
{
    public class GeneralProfile : Profile
    {
        public static Mapper InitializeAutomapper()
        {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(src => src.Password))

                                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))

                .ForMember(dest => dest.Id, opt => opt.Ignore())

.ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
.ForMember(dest => dest.Verified, opt => opt.Ignore())
.ForMember(dest => dest.ResetToken, opt => opt.Ignore())
.ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
.ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
.ForMember(dest => dest.Created, opt => opt.Ignore())
.ForMember(dest => dest.Updated, opt => opt.Ignore())
.ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.AccountPhone, opt => opt.Ignore())
 .ForMember(dest => dest.GUID, opt => opt.Ignore());

            });
            config.AssertConfigurationIsValid();
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
