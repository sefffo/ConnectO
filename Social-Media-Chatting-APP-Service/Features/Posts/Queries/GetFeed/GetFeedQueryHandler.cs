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
        var postRepo = unitOfWork.GetRepository<Post, Guid>();
        var friendshipRepo = unitOfWork.GetRepository<Social_Media_Chatting_APP_Domain.Entities.Friendship, Guid>();
        // get the direct friends for the user 
        var directFriends =
            await friendshipRepo.FindAllAsync(f =>
                (f.RequestId.ToString() == request.UserId || f.AddresseeId.ToString() == request.UserId) &&
                f.Status == FriendshipStatus.Accepted);
        // lw hwa da el ba3et w msh el user hat el req lw el 3ks hat el addressee
        var directFriendsIds =
            directFriends.Select(f => f.RequestId.ToString() == request.UserId ? f.AddresseeId : f.RequestId).ToList();

        var directFriendsIdsString = directFriendsIds.Select(id => id.ToString()).ToList();
        directFriendsIdsString.Add(request.UserId);

        //var fofFriends = await friendshipRepo.FindAllAsync(f.=);

        //get the blocked friends 
        var blockedFriends = await friendshipRepo.FindAllAsync(f =>
            (f.AddresseeId.ToString() == request.UserId || f.RequestId.ToString() == request.UserId) &&
            f.Status == FriendshipStatus.Blocked);
        var blockedIds = blockedFriends
            .Select(f => f.RequestId.ToString() == request.UserId ? f.AddresseeId : f.RequestId).ToList();
        var blockedIdsString = blockedIds.Select(id => id.ToString()).ToList();

        // get the feed posts 
        // first the direct friends posts 
        // get the friends of friednds ids first 

        var discoverIds = await friendshipRepo.FindAllAsync(f =>
            (directFriendsIds.Contains(f.RequestId) || directFriendsIds.Contains(f.AddresseeId)) &&
            f.Status == FriendshipStatus.Accepted);

        var discoverIdsString = discoverIds.SelectMany(f => new[] { f.RequestId.ToString(), f.AddresseeId.ToString() })
            .Distinct().ToList();
        var directFeedSpec = new FeedPostSpecification(directFriendsIdsString, request.FriendsCursor, request.Limit);
        var excludedIds = directFriendsIdsString.Concat(blockedIdsString).Concat<string>(new[] { request.UserId })
            .ToList();
        var fofFriendsSpec =
            new DiscoverPostSpecification(discoverIdsString, excludedIds, request.DiscoverPage, request.Limit);

        var directFriendsPosts = await postRepo.FindAllAsync(directFeedSpec);
        var fofFriendsPosts = await postRepo.FindAllAsync(fofFriendsSpec);

        // did we get more than we asked for?
        var hasNextPage = directFriendsPosts.Count() > request.Limit;
        // if yes, trim that extra post off — don't send it to the client
        if (hasNextPage)
            directFriendsPosts = directFriendsPosts.Take(request.Limit).ToList();
        // the cursor for the NEXT request = CreatedAt of the LAST post in this page
        var nextFriendsCursor = hasNextPage ? directFriendsPosts.Last().CreatedAt : (DateTime?)null;
        // for the discover now 
        // fe post aktar wla la
        var hasNextDiscover = fofFriendsPosts.Count() > request.Limit;
        //lw ah shelo w 70t el cursor 3la akher post 
        if (hasNextDiscover)
            fofFriendsPosts = fofFriendsPosts.Take(request.Limit).ToList();

        var nextDiscover = hasNextDiscover ? request.DiscoverPage + 1 : (int?)null;

        // we need to map the posts to DTOs
        // ── 1. MAP entities → DTOs ───────────────────────────────────────────
        // AutoMapper copies all the simple fields (Id, Content, CreatedAt, etc.)
        var directPostDtos = mapper.Map<List<PostDto>>(directFriendsPosts);
        var discoverPostDtos = mapper.Map<List<PostDto>>(fofFriendsPosts);

        // ── 2. ENRICH direct friends posts ───────────────────────────────────
        // Zip pairs each PostDto with its original Post so we can read nav props
        foreach (var (dto, post) in directPostDtos.Zip(directFriendsPosts))
        {
            dto.IsLikedByMe = post.PostLikes.Any(l => l.UserId.ToString() == request.UserId);
            dto.LikeCount = post.PostLikes.Count;
            dto.CommentCount = post.Comments.Count(c => !c.IsDeleted);
            dto.RepostCount = post.Reposts.Count(r => !r.IsDeleted);
            dto.FeedSource= FeedSource.Friend; // ← this is the extra field
        }

        // ── 3. ENRICH discover (friends-of-friends) posts ────────────────────
        foreach (var (dto, post) in discoverPostDtos.Zip(fofFriendsPosts))
        {
            dto.IsLikedByMe = post.PostLikes.Any(l => l.UserId.ToString() == request.UserId);
            dto.LikeCount = post.PostLikes.Count;
            dto.CommentCount = post.Comments.Count(c => !c.IsDeleted);
            dto.RepostCount = post.Reposts.Count(r => !r.IsDeleted);
            dto.FeedSource = FeedSource.Discover; // ← different value here
        }

        // ── 4. MERGE & INTERLEAVE (the feed ratio) ────────────────────────────
        // We don't want 20 friend posts THEN 20 discover posts — that feels weird.
        // We interleave: for every 3 friend posts, insert 1 discover post.
        // Example with Limit=12: 9 friends + 3 discover = 12 posts, mixed together
        var mergedFeed = InterleaveFeeds(directPostDtos, discoverPostDtos, ratio: 3);

        // ── 5. RETURN ─────────────────────────────────────────────────────────
        return Result<FeedDto>.Ok(new FeedDto
        {
            Posts = mergedFeed,
            NextFriendsCursor = nextFriendsCursor,
            NextDiscoverPage = nextDiscover
        });
    }

    private static List<PostDto> InterleaveFeeds(
        List<PostDto> friends,
        List<PostDto> discover,
        int ratio = 3)
    {
        // ratio = 3 means: insert 1 discover post every 3 friend posts
        var result = new List<PostDto>();
        int discoverIndex = 0;

        for (int i = 0; i < friends.Count; i++)
        {
            result.Add(friends[i]);

            // every `ratio` friend posts, inject one discover post
            if ((i + 1) % ratio == 0 && discoverIndex < discover.Count)
                result.Add(discover[discoverIndex++]);
        }

        // append any leftover discover posts at the end
        result.AddRange(discover.Skip(discoverIndex));

        return result;
    }
}