using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.Posts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetFeed;

public class GetFeedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<GetFeedQuery, Result<FeedDto>>
{
    public async Task<Result<FeedDto>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        return default;
    }
}