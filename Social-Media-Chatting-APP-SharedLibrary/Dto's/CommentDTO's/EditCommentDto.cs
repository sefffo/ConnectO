namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;

/// <summary>
/// Only the text content of a comment can be edited.
/// Media asset is locked after creation — cannot be changed or removed.
/// </summary>
public record EditCommentDto(
    string? Content
);
