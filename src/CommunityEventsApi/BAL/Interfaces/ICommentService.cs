using CommunityEventsApi.DTOs.Comments;

namespace CommunityEventsApi.BAL.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetCommentsByEventAsync(Guid eventId);
    Task<CommentDto?> GetCommentByIdAsync(Guid id);
    Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto, Guid userId);
    Task<CommentDto> UpdateCommentAsync(Guid id, string content, Guid userId);
    Task DeleteCommentAsync(Guid id, Guid userId);
}
