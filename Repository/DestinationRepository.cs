using System;
using System.Linq.Expressions;
using System.Linq;
using cr2Project.Data;
using cr2Project.Models;
using cr2Project.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace cr2Project.Repository
{
    public class DestinationRepository : IDestinationRepository
    {
        private readonly ApplicationDBContext _db;

        public DestinationRepository(ApplicationDBContext db)
        {
            _db = db;
        }


        public async Task Create(Destination entity)
        {
            await _db.Destinations.AddAsync(entity);
            await Save();
        }

        public async Task<Destination> Get(Expression<Func<Destination, bool>> filter = null, bool tracked = true)
        {
            IQueryable<Destination> query = _db.Destinations;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Destination>> GetAll(Expression<Func<Destination, bool>> filter = null)
        {
            IQueryable<Destination> query = _db.Destinations;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task Remove(Destination entity)
        {
            _db.Destinations.Remove(entity);
            await Save();
        }

        public async Task Update (Destination entity)
        {
            _db.Destinations.Update(entity);
            await Save();
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}

