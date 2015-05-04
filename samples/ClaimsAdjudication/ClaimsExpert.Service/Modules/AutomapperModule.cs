using System;
using Autofac;
using AutoMapper;
using NRules.Samples.ClaimsExpert.Contract;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Service.Modules
{
    public class AutomapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Mapper.CreateMap<Claim, ClaimDto>()
                .ForMember(dest => dest.PatientFirstName, x => x.MapFrom(src => src.Patient.Name.FirstName))
                .ForMember(dest => dest.PatientMiddleName, x => x.MapFrom(src => src.Patient.Name.MiddleName))
                .ForMember(dest => dest.PatientLastName, x => x.MapFrom(src => src.Patient.Name.LastName));

            Mapper.CreateMap<ClaimAlert, ClaimAlertDto>();

            Mapper.CreateMap<ClaimStatus, AdjudicationStatus>()
                .ConvertUsing(src =>
                {
                    var result = AdjudicationStatus.Open;
                    Enum.TryParse(src.ToString(), true, out result);
                    return result;
                });

            Mapper.CreateMap<ClaimType, string>()
                .ConvertUsing(src => src.ToString());
        }
    }
}