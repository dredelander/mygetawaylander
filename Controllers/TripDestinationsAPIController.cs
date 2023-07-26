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
using System.Net;

namespace cr2Project.Controllers
{
    [Route("api/TripDestinationsAPI")]
    [ApiController]
    public class TripDestinationsAPIController : ControllerBase
	{

        public ILogger<TripDestinationsAPIController> _logger { get; }
        private readonly ApplicationDBContext _db;
        private readonly ITripDestinationsRepository _dbTripDest;
        protected APIResponse _response;

        public TripDestinationsAPIController( ApplicationDBContext db, ITripDestinationsRepository dbTripDest, ILogger<TripDestinationsAPIController> logger)
        {
            _db = db;
            _logger = logger;
            _dbTripDest = dbTripDest;
            _response = new();
        }

        //GET ALL DESTINATIONS FOR A TRIP
        [HttpGet("{tripId:int}/destinations/")]
        public async Task<ActionResult<APIResponse>> GetTripDestinations(int tripId)
        {
            try
            {
                _logger.LogInformation("Getting all destinations");

                IEnumerable<Destination> tripDestList = await _dbTripDest.GetTripDestinations(x => x.Id == tripId);
                _response.Response = tripDestList;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }

        }

        // DELETE A DESTINATION FROM A TRIP
        [HttpDelete("{tripId:int}/deletefromtrip/{destId:int}", Name = "DeleteDestinationFromTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteDestinationFromTrip(int tripId, int destId)
        {
            if (tripId == 0 | destId == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            bool result = await _dbTripDest.RemoveDestination(filterTrip: x => x.Id == tripId,filterDest: x => x.Id == destId);

            if (result)
            {
                _response.Response = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound();
            }
        }

        // ADD A DESTINATION FROM TO A TRIP
        [HttpPost("{tripId:int}/addtotrip/{destId:int}", Name = "AddDestinationToTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> AddDestinationToTrip(int tripId, int destId)
        {
            try
            {

                if (tripId == 0 | destId == 0)
                {
                    return BadRequest();
                }

                await _dbTripDest.AddDestination(filterTrip: x => x.Id == tripId, filterDest: x => x.Id == destId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = new List<string>() { "The Destination has been added to your trip!" };

                return Ok(_response);
            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }


        }
    }
}

