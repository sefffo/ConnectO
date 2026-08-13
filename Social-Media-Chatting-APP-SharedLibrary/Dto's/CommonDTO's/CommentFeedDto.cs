using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

public record CommentFeedDto(
    List<CommentDto> Comments,
    DateTime? NextCursor,
    bool HasNextPage
);