using System;
using cr2Project.Models;
using System.Linq.Expressions;

namespace cr2Project.Repository.IRepository
{
	public interface IDestinationRepository
	{

        Task<List<Destination>> GetAll(Expression<Func<Destination, bool>> filter = null, string? includeTrips = null);

        Task<Destination> Get(Expression<Func<Destination, bool >> filter = null, bool tracked = true, string? includeTrips = null);

        Task Create(Destination entity);

        Task Remove(Destination entity);

        Task Update(Destination entity);

        Task Save();
    }
}

