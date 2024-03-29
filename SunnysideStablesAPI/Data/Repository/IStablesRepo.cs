﻿using SunnysideStablesAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Data.Repository
{
    public interface IStablesRepo
    {
        Task<HorseListData> GetHorses(bool includeOwners=false,
                                    int pageIndex=0,
                                    int pageSize=3,
                                    string searchParam=null);

        Task<List<Owner>> GetOwners();

        Task<int> GetHorseCount();

        Task<Horse> GetHorseById(int id);

        void Add(Horse horseToAdd);

        void AddHorseOwner(HorseOwner horseOwner);

        void DeleteHorseOwner(HorseOwner horseOwner);

        Task<bool> Commit();
        void AddHorseOwners(List<HorseOwner> horseOwners);
    }
}
