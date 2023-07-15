using System;
using System.Diagnostics.Metrics;
using AutoMapper;
using cr2Project.Data;
using cr2Project.Models;
using cr2Project.Models.Dto;
using cr2Project.Repository;
using cr2Project.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cr2Project.Controllers
{

	[Route("api/DestinationAPI")]
	[ApiController]
	public class DestinationAPIController : ControllerBase
	{
        public ILogger<DestinationAPIController> _logger { get; }
		private readonly ApplicationDBContext _db;
		private readonly IMapper _mapper;
        private readonly IDestinationRepository _dbDestination;

        public DestinationAPIController(IDestinationRepository dbDestination,ApplicationDBContext db, ILogger<DestinationAPIController> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
			_mapper = mapper;
			_dbDestination = dbDestination;
        }

		// GET ALL THE DESTINATIONS - RETURN TYPE AS DTO, MAPPED WITH AUTOMAPPER
        [HttpGet]
		public async Task<ActionResult<IEnumerable<DestinationDTO>>> GetDestinations()
		{
			_logger.LogInformation("Getting all destinations");

			//IEnumerable<Destination> destinationList = await _db.Destinations.ToListAsync();
			IEnumerable<Destination> destinationList = await _dbDestination.GetAll();

            return Ok(_mapper.Map<List<DestinationDTO>>(destinationList));
		}

		// GET A SINGLE DESTINATION BY ID
        [HttpGet("{id:int}", Name ="GetDestination")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult <DestinationDTO>> GetDestinationById(int id)
        {
			if(id == 0)
			{
				_logger.LogError("Get Destination Error" + id);
				return BadRequest();
			}
            //var destination = await _db.Destinations.FirstOrDefaultAsync(d => d.Id == id);
            var destination = await _dbDestination.Get(d => d.Id == id);

            if (destination == null)
			{
				return NotFound();
			}

			
            return Ok(_mapper.Map<DestinationDTO>(destination));
        }

		// CREATE/ADD A DESTINATION

		[HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DestinationDTO>> AddDestination([FromBody]DestinationCreateDTO createDTO)
		{
			if(createDTO == null)
			{
				return BadRequest(createDTO);
			}

			//var check = await _db.Destinations.FirstOrDefaultAsync(d => d.Name.ToLower() == createDTO.Name.ToLower());
            var check = await _dbDestination.Get(d => d.Name.ToLower() == createDTO.Name.ToLower());

            if ( check != default)
			{
				ModelState.AddModelError("Custom Error", "Destination Name already exists");
				return BadRequest(ModelState);
			}

			var model = _mapper.Map<Destination>(createDTO);
			

			await _dbDestination.Create(model);
			

			return CreatedAtRoute("GetDestination",new { id = model.Id }, model);
		}


		// DELETE A DESTINATION
        [HttpDelete("{id:int}", Name = "DeleteDestination")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDestination(int id)
		{
			if(id == 0)
			{
				return BadRequest();
			}
			var destination = await _dbDestination.Get(d => d.Id == id);
			if(destination == null)
			{
				return NotFound();
			}

			await _dbDestination.Remove(destination);
			
			return NoContent();
		}

		// UPDATE A DESTINATION - FULL UPDATE(NOT A PATCH)
		[HttpPut("{id:int}", Name ="UpdateDestination")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateDestination(int id, [FromBody] DestinationUpdateDTO updateDTO)
		{
			if(updateDTO == null || id != updateDTO.Id)
			{
				return BadRequest();
			}


			var model = _mapper.Map<Destination>(updateDTO);

			_db.Destinations.Update(model);
			await _db.SaveChangesAsync();


            return NoContent();
        }


		// PATCH (PARTIAL UPDATE) OF A DESTINATION
        [HttpPatch("{id:int}", Name = "UpdatePartialDestination")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialDestination(int id, [FromBody] JsonPatchDocument<DestinationUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id ==0)
            {
                return BadRequest();
            }
            //var destination = await _db.Destinations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            var destination = await _dbDestination.Get(d => d.Id == id, tracked: false);

            DestinationUpdateDTO destinationDTO = _mapper.Map<DestinationUpdateDTO>(destination);


			if(destination == null)
			{
				return BadRequest();
			}

			patchDTO.ApplyTo(destinationDTO, ModelState);

			Destination model = _mapper.Map<Destination>(destinationDTO);

			await _dbDestination.Update(model);

			//await _db.SaveChangesAsync();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
            return NoContent();
        }
    }
}



//[Route("api/TripDestinationsAPI")]
//// UPDATE A DESTINATION - FULL UPDATE(NOT A PATCH)

//[HttpGet("{tripId:int}/destinations/")]
//public async Task<IActionResult> GetTripDestinations(int tripId)

//_db.Trips.Include( x => x.Destinations).Find(id).Destinations



//[HttpPut("{tripId:int}/destinations/{destId:int}", Name = "AddDestinationsToTrip")]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//[ProducesResponseType(StatusCodes.Status204NoContent)]
//public async Task<IActionResult> UpdateDestination(int id, [FromBody] DestinationUpdateDTO updateDTO)
//{
//    if (updateDTO == null || id != updateDTO.Id)
//    {
//        return BadRequest();
//    }


//    var model = _mapper.Map<Destination>(updateDTO);

//    _db.Destinations.Update(model);
//    await _db.SaveChangesAsync();


//    return NoContent();
//}