using AutoMapper;
using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.DTOs.Comments;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.BAL.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByEventAsync(Guid eventId)
    {
        var comments = await _commentRepository.GetCommentsByEventAsync(eventId);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return comment == null ? null : _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto, Guid userId)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = createDto.Content,
            EventId = createDto.EventId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> UpdateCommentAsync(Guid id, string content, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this comment");
        }

        comment.Content = content;
        comment.UpdatedAt = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment);
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task DeleteCommentAsync(Guid id, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this comment");
        }

        await _commentRepository.DeleteAsync(comment);
    }
}
