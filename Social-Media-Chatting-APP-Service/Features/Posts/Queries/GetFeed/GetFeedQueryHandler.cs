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
        var friendshipRepo = unitOfWork.GetRepository<Social_Media_Chatting_APP_Domain.Entities.Friendship, Guid>();
        var postRepo       = unitOfWork.GetRepository<Post, Guid>();

        // ── Step 1: resolve direct friend IDs ──────────────────────────────────
        var friendships = await friendshipRepo.FindAllAsync(f =>
            (f.RequestId.ToString()  == request.UserId ||
             f.AddresseeId.ToString() == request.UserId) &&
            f.Status == FriendshipStatus.Accepted);

        var directFriendIds = friendships
            .Select(f => f.RequestId.ToString() == request.UserId
                ? f.AddresseeId.ToString()
                : f.RequestId.ToString())
            .ToList();

        // ── Step 2: resolve blocked user IDs (exclude from discover) ──────────
        var blockedRows = await friendshipRepo.FindAllAsync(f =>
            f.Status == FriendshipStatus.Blocked &&
            (f.RequestId.ToString()  == request.UserId ||
             f.AddresseeId.ToString() == request.UserId));

        var blockedIds = blockedRows
            .SelectMany(f => new[]
            {
                f.RequestId.ToString(),
                f.AddresseeId.ToString()
            })
            .Distinct()
            .Where(id => id != request.UserId)
            .ToList();

        // ── Step 3: TIER 1 — friends feed ─────────────────────────────────────
        // Include self so own posts appear in your own feed
        var friendAndSelfIds = directFriendIds.Append(request.UserId).ToList();

        var friendSpec = new FeedPostSpecification(
            friendAndSelfIds,
            request.FriendsCursor,
            request.Limit);

        var friendPosts = (await postRepo.FindAllAsync(friendSpec)).ToList();

        var hasMoreFriends = friendPosts.Count > request.Limit;
        if (hasMoreFriends) friendPosts = friendPosts.Take(request.Limit).ToList();

        DateTime? nextFriendsCursor = hasMoreFriends
            ? friendPosts.Last().CreatedAt
            : null;

        // ── Step 4: TIER 2 — discover (friends-of-friends) ────────────────────
        // Collect all friends-of-friends from each direct friend's friendship rows
        var fofFriendships = await friendshipRepo.FindAllAsync(f =>
            (directFriendIds.Contains(f.RequestId.ToString()) ||
             directFriendIds.Contains(f.AddresseeId.ToString())) &&
            f.Status == FriendshipStatus.Accepted);

        var fofIds = fofFriendships
            .SelectMany(f => new[]
            {
                f.RequestId.ToString(),
                f.AddresseeId.ToString()
            })
            .Distinct()
            .ToList();

        // Exclude: me, direct friends, blocked users
        var excludedFromDiscover = directFriendIds
            .Append(request.UserId)
            .Concat(blockedIds)
            .ToList();

        var discoverCandidateIds = fofIds
            .Except(excludedFromDiscover)
            .ToList();

        // How many discover posts to inject per page
        var discoverLimit = Math.Max(request.Limit / 4, 3); // ~25% of page, min 3
        var skip          = request.DiscoverPage * discoverLimit;

        List<Post> discoverPosts = new();
        bool hasMoreDiscover     = false;
        int? nextDiscoverPage    = null;

        if (discoverCandidateIds.Count > 0)
        {
            var discoverSpec = new DiscoverPostSpecification(
                discoverCandidateIds,
                excludedFromDiscover,
                discoverLimit,
                skip);

            var rawDiscover = (await postRepo.FindAllAsync(discoverSpec)).ToList();

            // Sort in-memory by popularity (likes + comments) descending
            var sorted = rawDiscover
                .OrderByDescending(p => p.PostLikes.Count + p.Comments.Count(c => !c.IsDeleted))
                .ToList();

            // Apply skip + take for cursor-style paging
            var paged = sorted.Skip(skip).Take(discoverLimit + 1).ToList();

            hasMoreDiscover = paged.Count > discoverLimit;
            if (hasMoreDiscover) paged = paged.Take(discoverLimit).ToList();

            discoverPosts = paged;
            nextDiscoverPage = hasMoreDiscover ? request.DiscoverPage + 1 : null;
        }

        // ── Step 5: map + tag FeedSource ──────────────────────────────────────
        var friendDtos = mapper.Map<List<PostDto>>(friendPosts);
        foreach (var (dto, post) in friendDtos.Zip(friendPosts))
        {
            dto.FeedSource   = FeedSource.Friend;
            dto.IsLikedByMe  = post.PostLikes.Any(l => l.UserId == request.UserId);
            dto.LikeCount    = post.PostLikes.Count;
            dto.CommentCount = post.Comments.Count(c => !c.IsDeleted);
            dto.RepostCount  = post.Reposts.Count(r => !r.IsDeleted);
        }

        var discoverDtos = mapper.Map<List<PostDto>>(discoverPosts);
        foreach (var (dto, post) in discoverDtos.Zip(discoverPosts))
        {
            dto.FeedSource   = FeedSource.Discover;
            dto.IsLikedByMe  = post.PostLikes.Any(l => l.UserId == request.UserId);
            dto.LikeCount    = post.PostLikes.Count;
            dto.CommentCount = post.Comments.Count(c => !c.IsDeleted);
            dto.RepostCount  = post.Reposts.Count(r => !r.IsDeleted);
        }

        // Interleave: friends first, discover injected after
        var allPosts = friendDtos.Concat(discoverDtos).ToList();

        return Result<FeedDto>.Ok(new FeedDto(
            allPosts,
            nextFriendsCursor,
            nextDiscoverPage,
            hasMoreFriends,
            hasMoreDiscover));
    }
}
