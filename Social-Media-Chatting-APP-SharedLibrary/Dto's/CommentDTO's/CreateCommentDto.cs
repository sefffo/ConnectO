using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;


namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;

public record CreateCommentDto(
    Guid PostId,
    string? Content,
    List<CreateMediaAssetDto>? MediaAsset,
    Guid? ParentCommentId
);