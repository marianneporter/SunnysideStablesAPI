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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToTitleCase()))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Owner, OwnerDto>()
                  .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.ToTitleCase()))
                  .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.ToTitleCase()));
        }
    }
}  