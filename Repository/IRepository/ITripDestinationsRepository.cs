using System;
using cr2Project.Models;
using System.Linq.Expressions;

namespace cr2Project.Repository.IRepository
{
	public interface ITripDestinationsRepository
	{

        Task<List<Destination>> GetTripDestinations(Expression<Func<Trip, bool>> filter = null);

        Task AddDestination(Expression<Func<Trip, bool>> filterTrip = null, Expression<Func<Destination, bool>> filterDest = null);

        Task<bool> RemoveDestination(Expression<Func<Trip, bool>> filterTrip = null, Expression<Func<Destination, bool>> filterDest = null);

        Task Save();
    }
}

