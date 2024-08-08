using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.PublicInstitutions.Domain.Entities;
using Shared;

namespace SB.PublicInstitutions.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PublicInstitutionsController(IPublicInstitutionsService publicInstitutionsService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicInstitution>>> GetAll()
        {
            var publicInstitutions = await publicInstitutionsService.GetAll();
            return Ok(publicInstitutions);
        }
        
        [HttpPost]
        public async Task<ActionResult<PublicInstitution>> Create([FromBody] PublicInstitutionCreateDto institutionDto)
        {
            var publicInstitution = await publicInstitutionsService.Create(institutionDto);
            return Ok(publicInstitution);
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<PublicInstitution>> Update(Guid id, [FromBody] PublicInstitutionUpdateDto institutionDto)
        {
            var publicInstitution = await publicInstitutionsService.Update(id, institutionDto);
            return Ok(publicInstitution);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var isDeleted = await publicInstitutionsService.Delete(id);
            if(isDeleted) return Ok();
            return BadRequest();
        }

    }
}
