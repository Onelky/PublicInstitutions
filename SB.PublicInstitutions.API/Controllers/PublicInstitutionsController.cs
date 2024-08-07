using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace SB.PublicInstitutions.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PublicInstitutionsController(IPublicInstitutionsService publicInstitutionsService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var publicInstitutions = await publicInstitutionsService.GetAll();
            return Ok(publicInstitutions);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PublicInstitutionDto institutionDto)
        {
            var publicInstitution = await publicInstitutionsService.Create(institutionDto);
            return Ok(publicInstitution);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PublicInstitutionDto institutionDto)
        {
            var publicInstitution = await publicInstitutionsService.Update(id, institutionDto);
            return Ok(publicInstitution);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await publicInstitutionsService.Delete(id);
            if(isDeleted) return Ok();
            return BadRequest();
        }

    }
}
