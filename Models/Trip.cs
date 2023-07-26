using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace cr2Project.Models
{
	public class Trip
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
		public int NumberOfGuest { get; set; }

        public int? NumberOfDestinations { get; set; }

        public ICollection<Destination>? Destinations { get; set; } 

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public double BudgetMinimun { get; set; }

        [Required]
        public double BudgetMaximun { get; set; }

        public Trip()
		{

            
		}
	}
}

