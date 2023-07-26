using System;
using System.ComponentModel.DataAnnotations;

namespace cr2Project.Models.Dto
{
	public class TripUpdateDTO
	{
        [Required]
        public int Id { get; set; }
        [MaxLength(40)]
        public string Nickname { get; set; }

        public int NumberOfGuest { get; set; }

        //public int NumberOfDestinations { get; set; }
        public ICollection<Destination>? Destinations { get; set; } 

        public DateTime UpdatedDate { get; }

        public double BudgetMinimun { get; set; }

        public double BudgetMaximun { get; set; }
        public TripUpdateDTO()
		{
            UpdatedDate = DateTime.UtcNow;
		}
	}
}

