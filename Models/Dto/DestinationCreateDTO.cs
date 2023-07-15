using System;
using System.ComponentModel.DataAnnotations;

namespace cr2Project.Models.Dto
{
	public class DestinationCreateDTO
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        //public int? TripId { get; set; }


    }
}

