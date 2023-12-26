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

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }


        [HttpGet("{unitId}")]
        public async Task<ActionResult<UnitDTO>> GetDeviceByIdAsync(int unitId)
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
        
        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private bool HasPermission(string permission)
        {
            return User.HasClaim(c => c.Type == "permissions" && c.Value == permission);
        }
    }
    
}