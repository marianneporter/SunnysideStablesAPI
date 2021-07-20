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

        Task<int> GetHorseCount();
    }
}
