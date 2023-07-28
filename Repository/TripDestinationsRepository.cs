using System;
using System.Linq;
using System.Linq.Expressions;
using cr2Project.Data;
using cr2Project.Models;
using cr2Project.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace cr2Project.Repository
{
	public class TripDestinationsRepository : ITripDestinationsRepository
	{
        private readonly ApplicationDBContext _db;
        public TripDestinationsRepository(ApplicationDBContext db)
		{
            _db = db;
            //_db.Destinations.Include(d => d.Trips).ToList();
        }

        public async Task AddDestination(Expression<Func<Trip, bool>> filterTrip = null, Expression<Func<Destination, bool>> filterDest = null)
        {
            IQueryable<Trip> query = _db.Trips.Include(x => x.Destinations);

            if (filterTrip != null)
            {
                query = query.Where(filterTrip);
            }

            var trip = await query.FirstOrDefaultAsync();

            IQueryable<Destination> queryDest = _db.Destinations;

            if (filterDest != null)
            {
                queryDest = queryDest.Where(filterDest);
            }
            if (trip != null)
            {
                var destinations = trip.Destinations;
                var destinationToAdd = await queryDest.FirstOrDefaultAsync();
                destinations.Add(destinationToAdd);
                await Save();
                //return true;
            }
            else
            {
                //return false;
            }
        }

        public async Task<List<Destination>> GetTripDestinations(Expression<Func<Trip, bool>> filter = null)
        {

            IQueryable<Trip> query = _db.Trips.Include(x => x.Destinations);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var trip = await query.FirstOrDefaultAsync();


            var destinations =  trip.Destinations.ToList();
            
            return destinations;
        }

        public async Task<bool> RemoveDestination(Expression<Func<Trip, bool>> filterTrip = null, Expression<Func<Destination, bool>> filterDest = null)
        {

            IQueryable<Trip> query = _db.Trips.Include(x => x.Destinations);

            if (filterTrip != null)
            {
                query = query.Where(filterTrip);
            }

            var trip = await query.FirstOrDefaultAsync();

            IQueryable<Destination> queryDest = _db.Destinations;

            if (filterDest != null)
            {
                queryDest = queryDest.Where(filterDest);
            }
            if (trip != null)
            {
                var destinations = trip.Destinations;
                var destinationToRemove = await queryDest.FirstOrDefaultAsync();
                destinations.Remove(destinationToRemove);
                await Save();
                return true;
            }
            else
            {
                return false;
            }

            


        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}

