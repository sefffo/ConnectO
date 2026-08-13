using FluentValidation;
using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        // AuthorId must always be present
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("AuthorId is required");

        // Original posts must have content or media — reposts/quotes are exempt
        When(x => x.Dto.PostType == PostType.Original, () =>
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Dto.Content) || 
                           (x.Dto.MediaAsset != null && x.Dto.MediaAsset.Any()))
                .WithMessage("Post must have either text content or a media asset");
        });

        // Content length cap when provided
        When(x => !string.IsNullOrWhiteSpace(x.Dto.Content), () =>
        {
            RuleFor(x => x.Dto.Content)
                .MaximumLength(3000)
                .WithMessage("Post content must be less than 3000 characters");
        });

        // Repost and Quote must have OriginalPostId
        When(x => x.Dto.PostType == PostType.Repost || x.Dto.PostType == PostType.Quote, () =>
        {
            RuleFor(x => x.Dto.OriginalPostId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage("OriginalPostId is required for Repost and Quote");
        });

        // Quote must have QuoteContent
        When(x => x.Dto.PostType == PostType.Quote, () =>
        {
            RuleFor(x => x.Dto.QuoteContent)
                .NotEmpty()
                .MaximumLength(3000)
                .WithMessage("Quote content is required and must be less than 3000 characters");
        });

        // PostType must be a valid enum value
        RuleFor(x => x.Dto.PostType)
            .IsInEnum()
            .WithMessage("Invalid post type");
    }
}