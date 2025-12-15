using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityEventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all comments for a specific event
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <returns>List of comments</returns>
    [HttpGet("event/{eventId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByEvent(Guid eventId)
    {
        try
        {
            var comments = await _commentService.GetCommentsByEventAsync(eventId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for event {EventId}", eventId);
            return StatusCode(500, new { message = "An error occurred while retrieving comments" });
        }
    }

    /// <summary>
    /// Get a specific comment by ID
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Comment details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> GetCommentById(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comment {CommentId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving comment" });
        }
    }

    /// <summary>
    /// Create a new comment on an event
    /// </summary>
    /// <param name="createDto">Comment information</param>
    /// <returns>Created comment</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var createdComment = await _commentService.CreateCommentAsync(createDto, userId);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Comment creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return StatusCode(500, new { message = "An error occurred while creating comment" });
        }
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <param name="updateDto">Updated comment content</param>
    /// <returns>Updated comment</returns>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var updatedComment = await _commentService.UpdateCommentAsync(id, updateDto.Content, userId);
            return Ok(updatedComment);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized comment update attempt: {Message}", ex.Message);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", id);
            return StatusCode(500, new { message = "An error occurred while updating comment" });
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            await _commentService.DeleteCommentAsync(id, userId);
            return Ok(new { message = "Comment deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized comment deletion attempt: {Message}", ex.Message);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting comment" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
