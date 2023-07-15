using System;
using System.Collections.Generic;
using AutoMapper;
using cr2Project.Data;
using cr2Project.Models;
using cr2Project.Models.Dto;
using cr2Project.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using cr2Project.Repository;

namespace cr2Project.Controllers
{
    [Route("api/TripDestinationsAPI")]
    [ApiController]
    public class TripDestinationsAPIController : ControllerBase
	{

        public ILogger<TripDestinationsAPIController> _logger { get; }
        private readonly ApplicationDBContext _db;
        private readonly ITripDestinationsRepository _dbTripDest;

        public TripDestinationsAPIController( ApplicationDBContext db, ITripDestinationsRepository dbTripDest, ILogger<TripDestinationsAPIController> logger)
        {
            _db = db;
            _logger = logger;
            _dbTripDest = dbTripDest;
        }

        //GET ALL DESTINATIONS FOR A TRIP
        [HttpGet("{tripId:int}/destinations/")]
        public async Task<ActionResult<IEnumerable<Destination>>> GetTripDestinations(int tripId)
        {
            _logger.LogInformation("Getting all destinations");

            IEnumerable<Destination> tripDestList = await _dbTripDest.GetTripDestinations(x => x.Id == tripId);

            return Ok(tripDestList);

    
        }

        // DELETE A DESTINATION FROM A TRIP
        [HttpDelete("{tripId:int}/deletefromtrip/{destId:int}", Name = "DeleteDestinationFromTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDestinationFromTrip(int tripId, int destId)
        {
            if (tripId == 0 | destId == 0)
            {
                return BadRequest();
            }
            bool result = await _dbTripDest.RemoveDestination(filterTrip: x => x.Id == tripId,filterDest: x => x.Id == destId);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        // ADD A DESTINATION FROM TO A TRIP
        [HttpPost("{tripId:int}/addtotrip/{destId:int}", Name = "AddDestinationToTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AddDestinationToTrip(int tripId, int destId)
        {
            if (tripId == 0 | destId == 0)
            {
                return BadRequest();
            }

            await _dbTripDest.AddDestination(filterTrip: x => x.Id == tripId, filterDest: x => x.Id == destId);

            return NoContent();

        }
    }
}

