using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Events;
using CommunityEventsApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    /// <summary>
    /// Get all events with optional filters
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <returns>List of events</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<EventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<EventDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<EventDto>>>> GetAllEvents([FromQuery] EventFilterDto filter)
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync(filter);
            return Ok(HttpApiResponse<IEnumerable<EventDto>>.Success(events, "Events retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return StatusCode(500, HttpApiResponse<IEnumerable<EventDto>>.InternalServerError("An error occurred while retrieving events"));
        }
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Event details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<EventDto>>> GetEventById(Guid id)
    {
        try
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
            {
                return NotFound(HttpApiResponse<EventDto>.NotFound("Event not found"));
            }

            return Ok(HttpApiResponse<EventDto>.Success(eventDto, "Event retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {EventId}", id);
            return StatusCode(500, HttpApiResponse<EventDto>.InternalServerError("An error occurred while retrieving event"));
        }
    }

    /// <summary>
    /// Get nearby events based on location
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="radiusKm">Search radius in kilometers (default: 10)</param>
    /// <returns>List of nearby events</returns>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<EventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<EventDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<EventDto>>>> GetNearbyEvents(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10)
    {
        try
        {
            var events = await _eventService.GetNearbyEventsAsync(latitude, longitude, radiusKm);
            return Ok(HttpApiResponse<IEnumerable<EventDto>>.Success(events, "Nearby events retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nearby events");
            return StatusCode(500, HttpApiResponse<IEnumerable<EventDto>>.InternalServerError("An error occurred while retrieving nearby events"));
        }
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    /// <param name="createDto">Event information</param>
    /// <returns>Created event</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<EventDto>>> CreateEvent([FromBody] CreateEventDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<EventDto>.Unauthorized("Invalid user"));
            }

            var createdEvent = await _eventService.CreateEventAsync(createDto, userId);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, new HttpApiResponse<EventDto>(System.Net.HttpStatusCode.Created, "Event created successfully", createdEvent));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Event creation failed: {Message}", ex.Message);
            return BadRequest(HttpApiResponse<EventDto>.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, HttpApiResponse<EventDto>.InternalServerError("An error occurred while creating event"));
        }
    }

    /// <summary>
    /// Update an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="updateDto">Updated event information</param>
    /// <returns>Updated event</returns>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<EventDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<EventDto>>> UpdateEvent(Guid id, [FromBody] CreateEventDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<EventDto>.Unauthorized("Invalid user"));
            }

            var updatedEvent = await _eventService.UpdateEventAsync(id, updateDto, userId);
            return Ok(HttpApiResponse<EventDto>.Success(updatedEvent, "Event updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized event update attempt: {Message}", ex.Message);
            return StatusCode(403, HttpApiResponse<EventDto>.Forbidden(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(HttpApiResponse<EventDto>.NotFound(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            return StatusCode(500, HttpApiResponse<EventDto>.InternalServerError("An error occurred while updating event"));
        }
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> DeleteEvent(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<object>.Unauthorized("Invalid user"));
            }

            await _eventService.DeleteEventAsync(id, userId);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Event deleted successfully", null));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized event deletion attempt: {Message}", ex.Message);
            return StatusCode(403, HttpApiResponse<object>.Forbidden(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(HttpApiResponse<object>.NotFound(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while deleting event"));
        }
    }

    /// <summary>
    /// Register current user for an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/register")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> RegisterForEvent(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<object>.Unauthorized("Invalid user"));
            }

            var result = await _eventService.RegisterForEventAsync(id, userId);
            if (!result)
            {
                return BadRequest(HttpApiResponse<object>.BadRequest("Unable to register for event"));
            }

            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Successfully registered for event", null));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(HttpApiResponse<object>.NotFound(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(HttpApiResponse<object>.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering for event {EventId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while registering for event"));
        }
    }

    /// <summary>
    /// Unregister current user from an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/unregister")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> UnregisterFromEvent(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<object>.Unauthorized("Invalid user"));
            }

            var result = await _eventService.UnregisterFromEventAsync(id, userId);
            if (!result)
            {
                return BadRequest(HttpApiResponse<object>.BadRequest("Unable to unregister from event"));
            }

            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Successfully unregistered from event", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering from event {EventId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while unregistering from event"));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
