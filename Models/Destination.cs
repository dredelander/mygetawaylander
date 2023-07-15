using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace cr2Project.Models
{
	public class Destination
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Name { get; set; }

		[Required]
        [MaxLength(50)]
        public string City { get; set; }
		[Required]
		[MaxLength(50)]
		public string Country { get; set; }

		public bool OceanDestination { get; set; }
		public bool LakeDestination { get; set; }
		public bool MountainDestination { get; set; }
		public bool CityDestination { get; set; }
		public bool RemoteDestination { get; set; }
		public bool FamilyDestination { get; set; }

		[JsonIgnore]
        public ICollection<Trip>? Trips { get; set; }

        public Destination()
		{
			OceanDestination = false;
			LakeDestination = false;
			MountainDestination = false;
			CityDestination = false;
			RemoteDestination = false;
			FamilyDestination = false;
		}

	}
}

