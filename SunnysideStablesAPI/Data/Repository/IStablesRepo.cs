using SunnysideStablesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Data.Repository
{
    public interface IStablesRepo
    {
        Task<List<Horse>> GetHorses(bool includeOwners=false, int pageIndex=0, int pageSize=3);

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
