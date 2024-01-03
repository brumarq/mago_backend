using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.DTOs.Device;
using Application.DTOs.Misc;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Controllers
{
    [Route("deviceMS/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationsService _authorizationService;

        public UnitController(IUnitService unitService, IAuthenticationService authenticationService, IAuthorizationsService authorizationService)
        {
            _unitService = unitService;
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
        }


        [HttpGet("{unitId}")]
        [Authorize("All")]
        public async Task<ActionResult<UnitDTO>> GetUnitById(int unitId)
        {      
            try
            {
                var unit = await _unitService.GetUnitByIdAsync(unitId);
                return (unit == null) ? NotFound() : Ok(unit);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
    
}