﻿using Application.ApplicationServices.Interfaces;
using Application.DTOs.DeviceMetrics;
using Application.DTOs.Metrics;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

///<Summary>
/// Field controller class
///</Summary>
[Route("orchestrate/fields/[controller]")]
[ApiController]
public class FieldController : ControllerBase
{
    private readonly IFieldService _fieldService;

    ///<Summary>
    /// Field controller constructor
    ///</Summary>
    public FieldController(IFieldService fieldService)
    {
        _fieldService = fieldService;
    }

    /// <summary>
    /// Create a field | Permissions: Admin
    /// </summary>
    /// <param name="createFieldDTO">Body for creation of a field</param>
    /// <returns>List of device metrics</returns>
    /// <response code="201">Creates a field successfully</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="403">Forbidden access.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("")]
    [Authorize("Admin")]
    public async Task<ActionResult<FieldDTO>> CreateField([FromBody] CreateFieldDTO createFieldDTO)
    {
        try
        {
            var createField = await _fieldService.CreateFieldAsync(createFieldDTO);

            return Created("", createField);
        }
        catch (CustomException ce)
        {
            return StatusCode((int)ce.StatusCode, ce.Message);
        }
        catch (HttpRequestException re)
        {
            return StatusCode((int)re.StatusCode!, re?.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }
}