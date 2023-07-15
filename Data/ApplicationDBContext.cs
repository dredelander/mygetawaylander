using System;
using cr2Project.Models;
using cr2Project.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace cr2Project.Data
{
	public class ApplicationDBContext : DbContext
	{
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base (options)
        {
        }
        public DbSet<Trip> Trips { get; set; }
		public DbSet<Destination> Destinations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Destination>().HasData(
                new Destination { Id = 1, Name = "Summer in Quito",City = "Quito", Country = "Ecuador" },
                new Destination { Id = 2, Name = "Magic of Cuenca", City = "Cuenca", Country = "Ecuador" },
                new Destination { Id = 3, Name = "Enchanted West", City = "Oregon", Country = "USA" },
                new Destination { Id = 4, Name = "Michigan's Sahara", City = "Silver Lake", Country = "USA" },
                new Destination { Id = 5, Name = "Themepark Bonanza", City = "Orlando", Country = "Ecuador" }
                );

            modelBuilder.Entity<Trip>().HasData(
                new Trip { Id = 1, Nickname = "Dream Vacay", BudgetMaximun = 2500, BudgetMinimun = 1500, NumberOfGuest = 2 },
                new Trip { Id = 2, Nickname = "Cruise Pregame", BudgetMaximun = 5500, BudgetMinimun = 4500, NumberOfGuest = 3 },
                new Trip { Id = 3, Nickname = "Boys are back in Town", BudgetMaximun = 1500, BudgetMinimun = 1000, NumberOfGuest = 5 }
                );
        }
    }
}

