namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

public record PostFeedDto(
    List<PostDto> Posts,
    DateTime? NextCursor,
    bool HasNextPage
);