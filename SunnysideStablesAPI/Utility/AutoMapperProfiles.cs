using AutoMapper;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using System;
using System.Linq;


namespace SunnysideStablesAPI.Utility
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Horse, HorseDto>()
                .ForMember(dest => dest.Owners,
                           opt => opt.MapFrom(x => x.HorseOwner.Select(y => $"{y.Owner.FirstName} {y.Owner.LastName}").ToList()))
                .ReverseMap();

            CreateMap<HorseAddUpdateDto, Horse>()
                .ForMember(dest => dest.Heightcm, opt => opt.MapFrom(src => Utility.HandsToCm(src.HeightHands)));


            CreateMap<Owner, OwnerDto>();
        }
    }
}  