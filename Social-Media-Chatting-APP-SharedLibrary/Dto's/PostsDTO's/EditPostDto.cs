namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

/// <summary>
/// Only the text content of a post can be edited.
/// Media assets are locked after creation — cannot be added, replaced, or removed.
/// </summary>
public record EditPostDto(
    string? Content,
    string? QuoteContent
);
