using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DTOs.Comments;
using CommunityEventsApi.Utils;
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
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<CommentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<IEnumerable<CommentDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<IEnumerable<CommentDto>>>> GetCommentsByEvent(Guid eventId)
    {
        try
        {
            var comments = await _commentService.GetCommentsByEventAsync(eventId);
            return Ok(new HttpApiResponse<IEnumerable<CommentDto>>(System.Net.HttpStatusCode.OK, "Comments retrieved successfully", comments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for event {EventId}", eventId);
            return StatusCode(500, new HttpApiResponse<IEnumerable<CommentDto>>(System.Net.HttpStatusCode.InternalServerError, "An error occurred while retrieving comments", null));
        }
    }

    /// <summary>
    /// Get a specific comment by ID
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Comment details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<CommentDto>>> GetCommentById(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(HttpApiResponse<CommentDto>.NotFound("Comment not found"));
            }

            return Ok(HttpApiResponse<CommentDto>.Success(comment, "Comment retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comment {CommentId}", id);
            return StatusCode(500, HttpApiResponse<CommentDto>.InternalServerError("An error occurred while retrieving comment"));
        }
    }

    /// <summary>
    /// Create a new comment on an event
    /// </summary>
    /// <param name="createDto">Comment information</param>
    /// <returns>Created comment</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<CommentDto>>> CreateComment([FromBody] CreateCommentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<CommentDto>.Unauthorized("Invalid user"));
            }

            var createdComment = await _commentService.CreateCommentAsync(createDto, userId);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, new HttpApiResponse<CommentDto>(System.Net.HttpStatusCode.Created, "Comment created successfully", createdComment));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Comment creation failed: {Message}", ex.Message);
            return BadRequest(HttpApiResponse<CommentDto>.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return StatusCode(500, HttpApiResponse<CommentDto>.InternalServerError("An error occurred while creating comment"));
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
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<CommentDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<CommentDto>>> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<CommentDto>.Unauthorized("Invalid user"));
            }

            var updatedComment = await _commentService.UpdateCommentAsync(id, updateDto.Content, userId);
            return Ok(HttpApiResponse<CommentDto>.Success(updatedComment, "Comment updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized comment update attempt: {Message}", ex.Message);
            return StatusCode(403, HttpApiResponse<CommentDto>.Forbidden(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(HttpApiResponse<CommentDto>.NotFound(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", id);
            return StatusCode(500, HttpApiResponse<CommentDto>.InternalServerError("An error occurred while updating comment"));
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HttpApiResponse<object>>> DeleteComment(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(HttpApiResponse<object>.Unauthorized("Invalid user"));
            }

            await _commentService.DeleteCommentAsync(id, userId);
            return Ok(new HttpApiResponse<object>(System.Net.HttpStatusCode.OK, "Comment deleted successfully", null));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized comment deletion attempt: {Message}", ex.Message);
            return StatusCode(403, HttpApiResponse<object>.Forbidden(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(HttpApiResponse<object>.NotFound(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", id);
            return StatusCode(500, HttpApiResponse<object>.InternalServerError("An error occurred while deleting comment"));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
