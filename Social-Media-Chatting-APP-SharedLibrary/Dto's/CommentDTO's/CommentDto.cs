
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;

public record CommentDto(
    Guid Id,
    string? Content,
    List<MediaAssetDto>? MediaAssets,
    Guid? ParentCommentId,
    AuthorDto Author,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int LikeCount,
    int RepliesCount
    );