using Application.ApplicationServices.Interfaces;
using Application.DTOs.Misc;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// Unit controller
    /// </summary>
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        /// <summary>
        /// Unit controller consturctor
        /// </summary>
        /// <param name="unitService"></param>
        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }


        /// <summary>
        /// Retrieves a specific unit by ID. Accessible by Admin.
        /// </summary>
        /// <param name="unitId">The ID of the unit to retrieve.</param>
        /// <returns>Returns the requested unit.</returns>
        /// <response code="200">Returns the requested unit.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Unit not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{unitId}")]
        [Authorize("All")]
        public async Task<ActionResult<UnitDTO>> GetUnitById(int unitId)
        {      
            try
            {
                var unit = await _unitService.GetUnitByIdAsync(unitId);
                return (unit == null) ? NotFound() : Ok(unit);
            }
            catch (CustomException ce)
            {
                return StatusCode((int)ce.StatusCode, ce.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
    
}