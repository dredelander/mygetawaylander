using System;
using System.ComponentModel.DataAnnotations;

namespace cr2Project.Models.Dto
{
	public class TripCreateDTO
	{
        [MaxLength(40)]
        public string Nickname { get; set; }

        [Required]
        public int NumberOfGuest { get; set; }

        //public int NumberOfDestinations { get; set; }

        public ICollection<Destination>? Destinations { get; set; }

        public DateTime CreatedDate { get;}

        [Required]
        public double BudgetMinimun { get; set; }

        [Required]
        public double BudgetMaximun { get; set; }

        public TripCreateDTO()
		{
            CreatedDate = DateTime.UtcNow;
		}
	}
}

