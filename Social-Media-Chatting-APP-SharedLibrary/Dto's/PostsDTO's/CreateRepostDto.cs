using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

public record CreateRepostDto(
    Guid OriginalPostId,
    string? QuoteContent,
    PostType PostType);