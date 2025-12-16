using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.Models;
using CommunityEventsApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunitiesController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly ILogger<CommunitiesController> _logger;

    public CommunitiesController(ICommunityService communityService, ILogger<CommunitiesController> logger)
    {
        _communityService = communityService;
        _logger = logger;
    }

    /// <summary>
    /// Get all communities
    /// </summary>
    /// <returns>List of communities</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<Community>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<Community>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<Community>>>> GetAllCommunities()
    {
        try
        {
            var communities = await _communityService.GetAllCommunitiesAsync();
            return Ok(new HttpApiResponse<IEnumerable<Community>>(System.Net.HttpStatusCode.OK, "Communities retrieved successfully", communities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving communities");
            return StatusCode(500, new HttpApiResponse<IEnumerable<Community>>(System.Net.HttpStatusCode.InternalServerError, "An error occurred while retrieving communities", null));
        }
    }

    /// <summary>
    /// Get community by ID
    /// </summary>
    /// <param name="id">Community ID</param>
    /// <returns>Community details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<Community>>> GetCommunityById(Guid id)
    {
        try
        {
            var community = await _communityService.GetCommunityByIdAsync(id);
            if (community == null)
            {
                return NotFound(HttpApiResponse<Community>.NotFound("Community not found"));
            }

            return Ok(HttpApiResponse<Community>.Success(community, "Community retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving community {CommunityId}", id);
            return StatusCode(500, HttpApiResponse<Community>.InternalServerError("An error occurred while retrieving community"));
        }
    }

    /// <summary>
    /// Get communities for the current user
    /// </summary>
    /// <returns>User's communities</returns>
    [HttpGet("my-communities")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<Community>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<Community>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<Community>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<Community>>>> GetMyCommunities()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new HttpApiResponse<IEnumerable<Community>>(System.Net.HttpStatusCode.Unauthorized, "Invalid user", null));
            }

            var communities = await _communityService.GetUserCommunitiesAsync(userId);
            return Ok(new HttpApiResponse<IEnumerable<Community>>(System.Net.HttpStatusCode.OK, "User communities retrieved successfully", communities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user communities");
            return StatusCode(500, new HttpApiResponse<IEnumerable<Community>>(System.Net.HttpStatusCode.InternalServerError, "An error occurred while retrieving communities", null));
        }
    }

    /// <summary>
    /// Create a new community
    /// </summary>
    /// <param name="community">Community information</param>
    /// <returns>Created community</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<Community>>> CreateCommunity([FromBody] Community community)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<Community>.Unauthorized("Invalid user"));
            }

            // Set the creator as the community admin
            community.CreatedBy = userId;
            community.CreatedAt = DateTime.UtcNow;

            var createdCommunity = await _communityService.CreateCommunityAsync(community);
            return CreatedAtAction(nameof(GetCommunityById), new { id = createdCommunity.Id }, new HttpApiResponse<Community>(System.Net.HttpStatusCode.Created, "Community created successfully", createdCommunity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating community");
            return StatusCode(500, HttpApiResponse<Community>.InternalServerError("An error occurred while creating community"));
        }
    }

    /// <summary>
    /// Update a community
    /// </summary>
    /// <param name="id">Community ID</param>
    /// <param name="community">Updated community information</param>
    /// <returns>Updated community</returns>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<Community>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<Community>>> UpdateCommunity(Guid id, [FromBody] Community community)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<Community>.Unauthorized("Invalid user"));
            }

            var existingCommunity = await _communityService.GetCommunityByIdAsync(id);
            if (existingCommunity == null)
            {
                return NotFound(HttpApiResponse<Community>.NotFound("Community not found"));
            }

            // Check if user is the creator or admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (existingCommunity.CreatedBy != userId && userRole != "Admin")
            {
                return StatusCode(403, HttpApiResponse<Community>.Forbidden("Access denied"));
            }

            community.UpdatedAt = DateTime.UtcNow;
            var updatedCommunity = await _communityService.UpdateCommunityAsync(id, community);
            return Ok(HttpApiResponse<Community>.Success(updatedCommunity, "Community updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating community {CommunityId}", id);
            return StatusCode(500, HttpApiResponse<Community>.InternalServerError("An error occurred while updating community"));
        }
    }

    /// <summary>
    /// Delete a community
    /// </summary>
    /// <param name="id">Community ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> DeleteCommunity(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<object>.Unauthorized("Invalid user"));
            }

            var existingCommunity = await _communityService.GetCommunityByIdAsync(id);
            if (existingCommunity == null)
            {
                return NotFound(HttpApiResponse<object>.NotFound("Community not found"));
            }

            // Check if user is the creator or admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (existingCommunity.CreatedBy != userId && userRole != "Admin")
            {
                return StatusCode(403, HttpApiResponse<object>.Forbidden("Access denied"));
            }

            await _communityService.DeleteCommunityAsync(id);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Community deleted successfully", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting community {CommunityId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while deleting community"));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
