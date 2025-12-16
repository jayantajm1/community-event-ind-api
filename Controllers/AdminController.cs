using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Events;
using CommunityEventsApi.DTOs.Users;
using CommunityEventsApi.Models;
using CommunityEventsApi.Utils;
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
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<UserDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(HttpApiResponse<IEnumerable<UserDto>>.Success(users, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, HttpApiResponse<IEnumerable<UserDto>>.InternalServerError("An error occurred while retrieving users"));
        }
    }

    /// <summary>
    /// Get all events (Admin only)
    /// </summary>
    /// <returns>List of all events</returns>
    [HttpGet("events")]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<EventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<EventDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<EventDto>>>> GetAllEvents()
    {
        try
        {
            var events = await _eventService.GetAllEventsAsync(new EventFilterDto());
            return Ok(HttpApiResponse<IEnumerable<EventDto>>.Success(events, "Events retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all events");
            return StatusCode(500, HttpApiResponse<IEnumerable<EventDto>>.InternalServerError("An error occurred while retrieving events"));
        }
    }

    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("users/{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "User deleted successfully by admin", null));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(HttpApiResponse<object>.NotFound(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while deleting user"));
        }
    }

    /// <summary>
    /// Update user role (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="roleDto">New role information</param>
    /// <returns>Success message</returns>
    [HttpPut("users/{id}/role")]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDto roleDto)
    {
        try
        {
            // This would need to be implemented in the UserService
            // For now, returning a placeholder message
            _logger.LogInformation("Admin updating user {UserId} role to {Role}", id, roleDto.Role);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, $"User role update to {roleDto.Role} - implementation pending in UserService", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role for {UserId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while updating user role"));
        }
    }

    /// <summary>
    /// Close/complete an event (Admin only)
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success message</returns>
    [HttpPost("events/{id}/close")]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> CloseEvent(Guid id)
    {
        try
        {
            // This would need to be implemented in the EventService
            // For now, returning a placeholder message
            _logger.LogInformation("Admin closing event {EventId}", id);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Event closed - implementation pending in EventService", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing event {EventId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while closing event"));
        }
    }

    /// <summary>
    /// Get platform statistics (Admin only)
    /// </summary>
    /// <returns>Platform statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> GetStatistics()
    {
        try
        {
            // This would aggregate data from various services
            // For now, returning a placeholder
            var stats = new
            {
                message = "Platform statistics",
                note = "Implementation pending - would include user count, event count, registration count, etc."
            };
            return Ok(HttpApiResponse<object>.Success(stats, "Statistics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics");
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while retrieving statistics"));
        }
    }
}
