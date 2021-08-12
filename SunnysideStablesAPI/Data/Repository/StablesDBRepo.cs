using Microsoft.EntityFrameworkCore;
using SunnysideStablesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Data.Repository
{
    public class StablesDBRepo : IStablesRepo
    {
        private readonly AppDbContext _context;

        public StablesDBRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Horse>> GetHorses(bool includeOwners=true, int pageIndex=0, int pageSize=3)
        {
            var horses= await _context.Horse
                          .Include(o => o.HorseOwner)
                          .ThenInclude(o => o.Owner)
                          .OrderBy(h => h.Name)
                          .Skip(pageIndex * pageSize)
                          .Take(pageSize).ToListAsync();
            return horses;
        }

        public async Task<int> GetHorseCount()
        {
            return await _context.Horse.CountAsync();
        }

        public void Add(Horse horseToAdd)
        {
            _context.Add(horseToAdd);
        }


        public async Task<bool> Commit()
        {
            var rowsChanged = await _context.SaveChangesAsync();
            return rowsChanged > 0;
        }

        public void AddHorseOwners(List<HorseOwner> horseOwners)
        {
            horseOwners.ForEach(o => _context.Add(o));
        }

        public async Task<List<Owner>> GetOwners()
        {
            return await _context.Owners.ToListAsync();          
        }

        public async Task<Horse> GetHorseById(int id)
        {
            return await _context.Horse.FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}
