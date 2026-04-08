using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        public CityController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [ProducesResponseType(typeof(List<CityWithVotesDto>), StatusCodes.Status200OK)]
        [HttpGet()]
        public async Task<IActionResult> GetAllWithVotes()
        {
            List<CityWithVotesDto> citiesWithVotes = await _unitOfWork.CityRepository.GetAllWithVotes();
            return Ok(citiesWithVotes);
        }

        [ProducesResponseType(typeof(City), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            City? city = await _unitOfWork.CityRepository.GetByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city);
        }

        

    }
}
