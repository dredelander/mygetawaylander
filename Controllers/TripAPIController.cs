using System;
using System.Net;
using AutoMapper;
using cr2Project.Data;
using cr2Project.Models;
using cr2Project.Models.Dto;
using cr2Project.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cr2Project.Controllers
{
    [Route("api/TripAPI")]
    [ApiController]
    public class TripAPIController : ControllerBase
	{
        public ILogger<TripAPIController> _logger { get; }
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly ITripRepository _dbTrip;

        public TripAPIController(ITripRepository dbTrip, ILogger<TripAPIController> logger, IMapper mapper)
        {
            _dbTrip = dbTrip;
            _logger = logger;
            _mapper = mapper;
            this._response = new();
        }

        // GET ALL THE TRIPS - RETURN TYPE AS THE NORMAL DTO, MAPPED WITH AUTOMAPPER
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetTrips()
        {
            _logger.LogInformation("Getting all trips");
            try
            {


                IEnumerable<Trip> tripList = await _dbTrip.GetAll();
                _response.Response = _mapper.Map<List<TripDTO>>(tripList);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }

        }

        // GET A SINGLE TRIP BY ID
        [HttpGet("{id:int}", Name = "GetTrip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetTripById(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Get Trip Error" + id);
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var trip = await _dbTrip.Get(d => d.Id == id);

                if (trip == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Response = _mapper.Map<TripDTO>(trip);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }

        }


        // CREATE/ADD A TRIP

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AddTrip([FromBody] TripCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    
                    return BadRequest(_response);
                }

                var check = await _dbTrip.Get(d => d.Nickname.ToLower() == createDTO.Nickname.ToLower());

                if (check != default)
                {
                    ModelState.AddModelError("Custom Error", "Trip Name already exists");
                    return BadRequest(ModelState);
                }

                var model = _mapper.Map<Trip>(createDTO);

                await _dbTrip.Create(model);
                _response.Response = _mapper.Map<TripDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetTrip", new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
        }

        // DELETE A TRIP
        [HttpDelete("{id:int}", Name = "DeleteTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteTrip(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                }
                var trip = await _dbTrip.Get(d => d.Id == id);
                if (trip == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _dbTrip.Remove(trip);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
        }

        // UPDATE A TRIP - FULL UPDATE(NOT A PATCH)
        [HttpPut("{id:int}", Name = "UpdateTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateTrip(int id, [FromBody] TripUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }


                var model = _mapper.Map<Trip>(updateDTO);

                await _dbTrip.Update(model);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
        }

        // PATCH (PARTIAL UPDATE) OF A TRIP
        [HttpPatch("{id:int}", Name = "UpdatePartialTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialTrip(int id, [FromBody] JsonPatchDocument<TripUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var trip = await _dbTrip.Get(d => d.Id == id, tracked: false);

            TripUpdateDTO tripDTO = _mapper.Map<TripUpdateDTO>(trip);

            if (trip == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(tripDTO, ModelState);

            Trip model = _mapper.Map<Trip>(tripDTO);

            await _dbTrip.Update(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}

