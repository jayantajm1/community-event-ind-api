using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Events;
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
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents([FromQuery] EventFilterDto filter)
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync(filter);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return StatusCode(500, new { message = "An error occurred while retrieving events" });
        }
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Event details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetEventById(Guid id)
    {
        try
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            return Ok(eventDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving event" });
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
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetNearbyEvents(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10)
    {
        try
        {
            var events = await _eventService.GetNearbyEventsAsync(latitude, longitude, radiusKm);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving nearby events");
            return StatusCode(500, new { message = "An error occurred while retrieving nearby events" });
        }
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    /// <param name="createDto">Event information</param>
    /// <returns>Created event</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var createdEvent = await _eventService.CreateEventAsync(createDto, userId);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Event creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, new { message = "An error occurred while creating event" });
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
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EventDto>> UpdateEvent(Guid id, [FromBody] CreateEventDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var updatedEvent = await _eventService.UpdateEventAsync(id, updateDto, userId);
            return Ok(updatedEvent);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized event update attempt: {Message}", ex.Message);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while updating event" });
        }
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            await _eventService.DeleteEventAsync(id, userId);
            return Ok(new { message = "Event deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized event deletion attempt: {Message}", ex.Message);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting event" });
        }
    }

    /// <summary>
    /// Register current user for an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/register")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterForEvent(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await _eventService.RegisterForEventAsync(id, userId);
            if (!result)
            {
                return BadRequest(new { message = "Unable to register for event. Event may be full or you may already be registered." });
            }

            return Ok(new { message = "Successfully registered for event" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering for event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while registering for event" });
        }
    }

    /// <summary>
    /// Unregister current user from an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/unregister")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnregisterFromEvent(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await _eventService.UnregisterFromEventAsync(id, userId);
            if (!result)
            {
                return BadRequest(new { message = "Unable to unregister from event. You may not be registered." });
            }

            return Ok(new { message = "Successfully unregistered from event" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering from event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while unregistering from event" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
