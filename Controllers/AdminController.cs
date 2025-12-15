using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Events;
using CommunityEventsApi.DTOs.Users;
using CommunityEventsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEventService _eventService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUserService userService,
        IEventService eventService,
        ILogger<AdminController> logger)
    {
        _userService = userService;
        _eventService = eventService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            // This would need to be implemented in the UserService
            // For now, returning a placeholder message
            return Ok(new { message = "Admin: Get all users - implementation pending in UserService" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }

    /// <summary>
    /// Get all events (Admin only)
    /// </summary>
    /// <returns>List of all events</returns>
    [HttpGet("events")]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync(new EventFilterDto());
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all events");
            return StatusCode(500, new { message = "An error occurred while retrieving events" });
        }
    }

    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "User deleted successfully by admin" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting user" });
        }
    }

    /// <summary>
    /// Update user role (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="roleDto">New role information</param>
    /// <returns>Success message</returns>
    [HttpPut("users/{id}/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto roleDto)
    {
        try
        {
            // This would need to be implemented in the UserService
            // For now, returning a placeholder message
            _logger.LogInformation("Admin updating user {UserId} role to {Role}", id, roleDto.Role);
            return Ok(new { message = $"User role update to {roleDto.Role} - implementation pending in UserService" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role for {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating user role" });
        }
    }

    /// <summary>
    /// Close/complete an event (Admin only)
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpPost("events/{id}/close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseEvent(Guid id)
    {
        try
        {
            // This would need to be implemented in the EventService
            // For now, returning a placeholder message
            _logger.LogInformation("Admin closing event {EventId}", id);
            return Ok(new { message = "Event closed - implementation pending in EventService" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing event {EventId}", id);
            return StatusCode(500, new { message = "An error occurred while closing event" });
        }
    }

    /// <summary>
    /// Get platform statistics (Admin only)
    /// </summary>
    /// <returns>Platform statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            // This would aggregate data from various services
            // For now, returning a placeholder
            return Ok(new
            {
                message = "Platform statistics",
                note = "Implementation pending - would include user count, event count, registration count, etc."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics");
            return StatusCode(500, new { message = "An error occurred while retrieving statistics" });
        }
    }
}
