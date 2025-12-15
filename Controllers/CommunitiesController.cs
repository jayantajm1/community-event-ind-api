using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.Models;
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
    [ProducesResponseType(typeof(IEnumerable<Community>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Community>>> GetAllCommunities()
    {
        try
        {
            var communities = await _communityService.GetAllCommunitiesAsync();
            return Ok(communities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving communities");
            return StatusCode(500, new { message = "An error occurred while retrieving communities" });
        }
    }

    /// <summary>
    /// Get community by ID
    /// </summary>
    /// <param name="id">Community ID</param>
    /// <returns>Community details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Community), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Community>> GetCommunityById(Guid id)
    {
        try
        {
            var community = await _communityService.GetCommunityByIdAsync(id);
            if (community == null)
            {
                return NotFound(new { message = "Community not found" });
            }

            return Ok(community);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving community {CommunityId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving community" });
        }
    }

    /// <summary>
    /// Get communities for the current user
    /// </summary>
    /// <returns>User's communities</returns>
    [HttpGet("my-communities")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Community>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Community>>> GetMyCommunities()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var communities = await _communityService.GetUserCommunitiesAsync(userId);
            return Ok(communities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user communities");
            return StatusCode(500, new { message = "An error occurred while retrieving communities" });
        }
    }

    /// <summary>
    /// Create a new community
    /// </summary>
    /// <param name="community">Community information</param>
    /// <returns>Created community</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Community), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Community>> CreateCommunity([FromBody] Community community)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Set the creator as the community admin
            community.CreatedBy = userId;
            community.CreatedAt = DateTime.UtcNow;

            var createdCommunity = await _communityService.CreateCommunityAsync(community);
            return CreatedAtAction(nameof(GetCommunityById), new { id = createdCommunity.Id }, createdCommunity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating community");
            return StatusCode(500, new { message = "An error occurred while creating community" });
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
    [ProducesResponseType(typeof(Community), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Community>> UpdateCommunity(Guid id, [FromBody] Community community)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var existingCommunity = await _communityService.GetCommunityByIdAsync(id);
            if (existingCommunity == null)
            {
                return NotFound(new { message = "Community not found" });
            }

            // Check if user is the creator or admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (existingCommunity.CreatedBy != userId && userRole != "Admin")
            {
                return Forbid();
            }

            community.UpdatedAt = DateTime.UtcNow;
            var updatedCommunity = await _communityService.UpdateCommunityAsync(id, community);
            return Ok(updatedCommunity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating community {CommunityId}", id);
            return StatusCode(500, new { message = "An error occurred while updating community" });
        }
    }

    /// <summary>
    /// Delete a community
    /// </summary>
    /// <param name="id">Community ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCommunity(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var existingCommunity = await _communityService.GetCommunityByIdAsync(id);
            if (existingCommunity == null)
            {
                return NotFound(new { message = "Community not found" });
            }

            // Check if user is the creator or admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (existingCommunity.CreatedBy != userId && userRole != "Admin")
            {
                return Forbid();
            }

            await _communityService.DeleteCommunityAsync(id);
            return Ok(new { message = "Community deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting community {CommunityId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting community" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
