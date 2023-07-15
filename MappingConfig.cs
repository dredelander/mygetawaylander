using System;
using AutoMapper;
using cr2Project.Models;
using cr2Project.Models.Dto;

namespace cr2Project
{
	public class MappingConfig : Profile
	{
		public MappingConfig()
		{
			CreateMap<Destination, DestinationDTO>();
            CreateMap<DestinationDTO, Destination>();

            CreateMap<Destination, DestinationCreateDTO>().ReverseMap();
            CreateMap<Destination, DestinationUpdateDTO>().ReverseMap();

			CreateMap<Trip, TripDTO>();
			CreateMap<TripDTO, Trip>();

            CreateMap<Trip, TripCreateDTO>().ReverseMap();
            CreateMap<Trip, TripUpdateDTO>().ReverseMap();
        }

	}
}

