using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DAL.Repositories
{
    public class LookupRepository(CampusDbContext db) : ILookupRepository
    {
        private readonly CampusDbContext _db = db;

        public Task<List<Campus>> GetCampusesAsync() =>
            _db.Campuses.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        public Task<List<Category>> GetCategoriesAsync() =>
            _db.Categories.AsNoTracking().OrderBy(c => c.Id).ToListAsync();

        public Task<List<Team>> GetTeamsAsync() =>
            _db.Teams.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

        public async Task<Campus?> GetCampusAsync(int id) =>
            await _db.Campuses.FindAsync(id);

        public async Task<Category?> GetCategoryAsync(int id) =>
            await _db.Categories.FindAsync(id);

        public async Task<Team?> GetTeamAsync(int id) =>
            await _db.Teams.FindAsync(id);
    }
}
