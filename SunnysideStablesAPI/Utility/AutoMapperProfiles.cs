using AutoMapper;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using System.Linq;


namespace SunnysideStablesAPI.Utility
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Horse, HorseDto>()
                .ForMember(dest => dest.Owners,
                           opt => opt.MapFrom(x => x.HorseOwner.Select(y => y.Owner).ToList()))
                .ReverseMap();

            CreateMap<HorseAddUpdateDto, Horse>()
                .ForMember(dest => dest.Heightcm, opt => opt.MapFrom(src => Utility.HandsToCm(src.HeightHands)))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Owner, OwnerDto>();
        }
    }
}  