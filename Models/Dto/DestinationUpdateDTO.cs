using System;
using System.ComponentModel.DataAnnotations;

namespace cr2Project.Models.Dto
{
	public class DestinationUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool OceanDestination { get; set; }
        public bool LakeDestination { get; set; }
        public bool MountainDestination { get; set; }
        public bool CityDestination { get; set; }
        public bool RemoteDestination { get; set; }
        public bool FamilyDestination { get; set; }
    }
}

