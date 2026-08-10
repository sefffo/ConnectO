using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

public record CreateMediaAssetDto(
    string MediaUrl,
    string FileName,
    string PublicId,
    ResourceType ResourceType
    );