using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartyController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        public PartyController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }


        [ProducesResponseType(typeof(List<Party>), StatusCodes.Status200OK)]
        [HttpGet()]
        public async Task<IActionResult> GetAll(int? categoryId)
        {
            List<Party> parties = await _unitOfWork.PartyRepository.GetAll();
            return Ok(parties);
        }


        
    }
}
