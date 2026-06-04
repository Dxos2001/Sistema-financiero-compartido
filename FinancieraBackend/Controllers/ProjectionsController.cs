using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectionsController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionsController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<ProjectionResponseDTO>> GetProjection(int groupId)
        {
            try
            {
                var result = await _projectionService.GenerateSixMonthProjectionAsync(groupId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, new { message = ex.Message }); // Service Unavailable if API Key is missing
            }
        }
    }
}
