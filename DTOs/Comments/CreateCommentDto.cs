using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Comments;

public class CreateCommentDto
{
    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid EventId { get; set; }
}
