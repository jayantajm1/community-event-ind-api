using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Comments;

public class UpdateCommentDto
{
    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;
}
