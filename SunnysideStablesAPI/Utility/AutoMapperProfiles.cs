using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;

namespace SunnysideStablesAPI.Utility
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Horse, HorseDto>()
                .ForMember(dest => dest.Owners, opt => opt.MapFrom(x => x.HorseOwner.Select(y => y.Owner).ToList()))
                .ReverseMap();

            CreateMap<Owner, OwnerDto>();
        }
    }
}
