using System;
namespace cr2Project.Models.Dto
{
	public class TripDTO
	{
        public int Id { get; set; }

        public string Nickname { get; set; }

        public int NumberOfGuest { get; set; }

        public int NumberOfDestinations { get
            {
                return Destinations?.Count() ?? 0;
            } }

        public ICollection<Destination>? Destinations { get; set; } 


        public double BudgetMinimun { get; set; }

        public double BudgetMaximun { get; set; }
        public TripDTO()
		{
		}
	}
}

