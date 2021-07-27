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
                           opt => opt.MapFrom(x => x.HorseOwner.Select(y => $"{y.Owner.FirstName} {y.Owner.LastName}").ToList()))
         //       .ForMember(dest=>dest.DOB, opt => opt.MapFrom(d=> d.DOB.ToString("yyyy-MM-dd")))
                .ReverseMap();

            CreateMap<Owner, OwnerDto>();
        }
    }
}
