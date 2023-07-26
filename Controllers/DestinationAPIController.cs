using System;
using System.Diagnostics.Metrics;
using System.Net;
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
		protected APIResponse _response;
		private readonly IMapper _mapper;
        private readonly IDestinationRepository _dbDestination;

        public DestinationAPIController(IDestinationRepository dbDestination,ApplicationDBContext db, ILogger<DestinationAPIController> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
			_mapper = mapper;
			_dbDestination = dbDestination;
			_response = new();
        }

		// GET ALL THE DESTINATIONS - RETURN TYPE AS DTO, MAPPED WITH AUTOMAPPER
        [HttpGet]
		public async Task<ActionResult<APIResponse>> GetDestinations()
		{
			_logger.LogInformation("Getting all destinations");

			try
			{
				IEnumerable<Destination> destinationList = await _dbDestination.GetAll();

				_response.Response = _mapper.Map<List<DestinationDTO>>(destinationList);
				_response.StatusCode = HttpStatusCode.OK;

				return Ok(_response);
			} catch(Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				return _response;
			}

		}

		// GET A SINGLE DESTINATION BY ID
        [HttpGet("{id:int}", Name ="GetDestination")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult <APIResponse>> GetDestinationById(int id)
        {
			try
			{
				if (id == 0)
				{
					_logger.LogError("Get Destination Error" + id);
					_response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                    
				}
				var destination = await _dbDestination.Get(d => d.Id == id);

				if (destination == null)
				{
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Response = _mapper.Map<DestinationDTO>(destination);
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

		// CREATE/ADD A DESTINATION

		[HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AddDestination([FromBody]DestinationCreateDTO createDTO)
		{
			try
			{
				if (createDTO == null)
				{
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                }

				var check = await _dbDestination.Get(d => d.Name.ToLower() == createDTO.Name.ToLower());

				if (check != default)
				{
					ModelState.AddModelError("Custom Error", "Destination Name already exists");
					return BadRequest(ModelState);
				}

				var model = _mapper.Map<Destination>(createDTO);


				await _dbDestination.Create(model);
                _response.Response = _mapper.Map<DestinationDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetDestination", new { id = model.Id }, _response);
			}
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return _response;
            }
        }


		// DELETE A DESTINATION
        [HttpDelete("{id:int}", Name = "DeleteDestination")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteDestination(int id)
		{
			try
			{
				if (id == 0)
				{
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                }
				var destination = await _dbDestination.Get(d => d.Id == id);
				if (destination == null)
				{
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

				await _dbDestination.Remove(destination);

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

		// UPDATE A DESTINATION - FULL UPDATE(NOT A PATCH)
		[HttpPut("{id:int}", Name ="UpdateDestination")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateDestination(int id, [FromBody] DestinationUpdateDTO updateDTO)
		{
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                }
                var model = _mapper.Map<Destination>(updateDTO);

                await _dbDestination.Update(model);
            
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

