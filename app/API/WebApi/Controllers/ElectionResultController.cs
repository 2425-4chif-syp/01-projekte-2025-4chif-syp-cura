using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectionResultController : ControllerBase
    {
        public record ElectionResultToPostDto(int CityId, int PartyId, int NumberOfVotes);

        IUnitOfWork _unitOfWork;
        public ElectionResultController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [ProducesResponseType(typeof(WinnerPartyDto), StatusCodes.Status200OK)]
        [HttpGet("GetWinner")]
        public async Task<IActionResult> GetWinner()
        {
            WinnerPartyDto? winner = await _unitOfWork.ElectionResultRepository.GetWinner();
            return Ok(winner);
        }

        [ProducesResponseType(typeof(List<CityResultDto>), StatusCodes.Status200OK)]
        [HttpGet("getResultByCityId/{cityId}")]
        public async Task<IActionResult> GetResultByCityId(int cityId)
        {
            List<CityResultDto> cityResult = await _unitOfWork.ElectionResultRepository.GetResultByCityId(cityId);
            return Ok(cityResult);
        }

        [ProducesResponseType(typeof(ElectionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            ElectionResult? electionResult = await _unitOfWork.ElectionResultRepository.GetByIdAsync(id);
            if (electionResult == null)
            {
                return NotFound();
            }
            return Ok(electionResult);
        }

        [ProducesResponseType(typeof(ElectionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ElectionResultToPostDto newResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ElectionResult electionResult = new ElectionResult()
            {
                City_Id = newResult.CityId,
                Party_Id = newResult.PartyId,
                NrOfVotes = newResult.NumberOfVotes
            };

            _unitOfWork.ElectionResultRepository.AddElectionResult(electionResult);

            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new
            {
                id = newResult
            }, electionResult);
        }
    }
}
