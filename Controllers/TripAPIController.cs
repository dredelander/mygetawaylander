using System;
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
        //private readonly ApplicationDBContext _db;
        private readonly ITripRepository _dbTrip;

        public TripAPIController(ITripRepository dbTrip, ILogger<TripAPIController> logger, IMapper mapper)
        {
            _dbTrip = dbTrip;
            _logger = logger;
            _mapper = mapper;
        }

        // GET ALL THE TRIPS - RETURN TYPE AS THE NORMAL DTO, MAPPED WITH AUTOMAPPER
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDTO>>> GetTrips()
        {
            _logger.LogInformation("Getting all trips");
            //IEnumerable<Trip> tripList = await _db.Trips.ToListAsync();
            IEnumerable<Trip> tripList = await _dbTrip.GetAll();
            return Ok(_mapper.Map<List<TripDTO>>(tripList));
        }

        // GET A SINGLE TRIP BY ID
        [HttpGet("{id:int}", Name = "GetTrip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TripDTO>> GetTripById(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Get Trip Error" + id);
                return BadRequest();
            }
            //var trip = await _db.Trips.FirstOrDefaultAsync(d => d.Id == id);
            var trip = await _dbTrip.Get(d => d.Id == id);

            if (trip == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<TripDTO>(trip));
        }


        // CREATE/ADD A TRIP

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TripDTO>> AddTrip([FromBody] TripCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            var check = await _dbTrip.Get(d => d.Nickname.ToLower() == createDTO.Nickname.ToLower());

            if (check != default)
            {
                ModelState.AddModelError("Custom Error", "Trip Name already exists");
                return BadRequest(ModelState);
            }


            var model = _mapper.Map<Trip>(createDTO);


            //_db.Trips.AddAsync(model);
            //_db.SaveChangesAsync();

            await _dbTrip.Create(model);

            return CreatedAtRoute("Gettrips", new { id = model.Id }, model);
        }

        // DELETE A TRIP
        [HttpDelete("{id:int}", Name = "DeleteTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var trip = await _dbTrip.Get(d => d.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            await _dbTrip.Remove(trip);
            
            return NoContent();
        }

        // UPDATE A TRIP - FULL UPDATE(NOT A PATCH)
        [HttpPut("{id:int}", Name = "UpdateTrip")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTrip(int id, [FromBody] TripUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }


            var model = _mapper.Map<Trip>(updateDTO);

            await _dbTrip.Update(model);

            return NoContent();
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

