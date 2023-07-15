using System;
using System.Linq;
using System.Linq.Expressions;
using cr2Project.Data;
using cr2Project.Models;
using Microsoft.EntityFrameworkCore;

namespace cr2Project.Repository
{
    public class TripRepository : ITripRepository
    {
        private readonly ApplicationDBContext _db;

        public TripRepository(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task Create(Trip entity)
        {
            entity.Destinations.ToList().ForEach(x => _db.Attach(x));
            await _db.Trips.AddAsync(entity);
            await Save();

        }

        public async Task<Trip> Get(Expression<Func<Trip, bool>> filter = null, bool tracked = true)
        {


            IQueryable<Trip> query = _db.Trips;

            if(!tracked)
            {
                query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();

        }

        public async Task<List<Trip>> GetAll(Expression<Func<Trip, bool>> filter = null)
        {
            IQueryable<Trip> query = _db.Trips.Include(x => x.Destinations);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task Remove(Trip entity)
        {
            _db.Trips.Remove(entity);
            await Save();
        }

        public async Task Update(Trip entity)
        {
            entity.Destinations.ToList().ForEach(x =>  _db.Attach(x));

            _db.Trips.Update(entity);
            await Save();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}

