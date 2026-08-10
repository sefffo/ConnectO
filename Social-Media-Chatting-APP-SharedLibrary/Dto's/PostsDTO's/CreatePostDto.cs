using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

public record CreatePostDto(
    string? Content,
    List<CreateMediaAssetDto>? MediaAsset,
    PostType PostType,
    Guid?OriginalPostId,
    string? QuoteContent
    );