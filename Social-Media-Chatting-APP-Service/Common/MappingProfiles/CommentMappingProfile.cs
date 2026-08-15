using AutoMapper;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;

namespace Social_Media_Chatting_APP_Service.Common.MappingProfiles;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        // Comment → CommentDto
        // Note: LikeCount, RepliesCount, IsLikedByMe are computed manually in handlers via Zip loop
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.MediaAsset, opt => opt.MapFrom(src => src.MediaAsset))
            .ForMember(dest => dest.LikeCount, opt => opt.Ignore())
            .ForMember(dest => dest.RepliesCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsLikedByMe, opt => opt.Ignore());
    }
}
