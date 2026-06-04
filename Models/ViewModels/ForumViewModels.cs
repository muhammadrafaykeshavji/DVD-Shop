using System.ComponentModel.DataAnnotations;

namespace E_project_DVD_Shop.Models.ViewModels;

public class ForumPostInput
{
    public int? ParentPostId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}

public class ForumChatInput
{
    [Required, StringLength(4000)]
    public string Content { get; set; } = string.Empty;
}

public class ForumQuestionInput
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(4000)]
    public string Content { get; set; } = string.Empty;
}

public class ForumAnswerInput
{
    public int QuestionId { get; set; }

    [Required, StringLength(4000)]
    public string Content { get; set; } = string.Empty;
}
