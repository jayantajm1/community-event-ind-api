using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Users;
using CommunityEventsApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    /// <returns>User profile information</returns>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<UserDto>>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<UserDto>.Unauthorized("Invalid user"));
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(HttpApiResponse<UserDto>.NotFound("User not found"));
            }

            return Ok(HttpApiResponse<UserDto>.Success(user, "Profile retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, HttpApiResponse<UserDto>.InternalServerError("An error occurred while retrieving profile"));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<UserDto>>> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(HttpApiResponse<UserDto>.NotFound("User not found"));
            }

            return Ok(HttpApiResponse<UserDto>.Success(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, HttpApiResponse<UserDto>.InternalServerError("An error occurred while retrieving user"));
        }
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    /// <param name="updateDto">Updated profile information</param>
    /// <returns>Updated user information</returns>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<UserDto>.Unauthorized("Invalid user"));
            }

            var updatedUser = await _userService.UpdateProfileAsync(userId, updateDto);
            return Ok(HttpApiResponse<UserDto>.Success(updatedUser, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, HttpApiResponse<UserDto>.InternalServerError("An error occurred while updating profile"));
        }
    }

    /// <summary>
    /// Update user by ID (Admin/Self only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateDto">Updated profile information</param>
    /// <returns>Updated user information</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateProfileDto updateDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow admin or the user themselves to update
            if (currentUserId != id && userRole != "Admin")
            {
                return StatusCode(403, HttpApiResponse<UserDto>.Forbidden("Access denied"));
            }

            var updatedUser = await _userService.UpdateProfileAsync(id, updateDto);
            return Ok(HttpApiResponse<UserDto>.Success(updatedUser, "User updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, HttpApiResponse<UserDto>.InternalServerError("An error occurred while updating user"));
        }
    }

    /// <summary>
    /// Delete user account
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> DeleteUser(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow admin or the user themselves to delete
            if (currentUserId != id && userRole != "Admin")
            {
                return StatusCode(403, HttpApiResponse<object>.Forbidden("Access denied"));
            }

            await _userService.DeleteUserAsync(id);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "User deleted successfully", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while deleting user"));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
