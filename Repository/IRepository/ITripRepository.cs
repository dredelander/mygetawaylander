using System;
using System.Linq.Expressions;
using cr2Project.Models;

namespace cr2Project.Repository
{
	public interface ITripRepository
	{
		Task<List<Trip>> GetAll(Expression<Func<Trip,bool>> filter = null);

		Task<Trip> Get(Expression<Func<Trip, bool>> filter = null, bool tracked = true);

		Task Create(Trip entity);

        Task Update(Trip entity);

        Task Remove(Trip entity);

		Task Save();


	}
}

