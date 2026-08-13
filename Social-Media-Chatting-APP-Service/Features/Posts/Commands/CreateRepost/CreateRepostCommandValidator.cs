using FluentValidation;
using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreateRepost;

public class CreateRepostCommandValidator : AbstractValidator<CreateRepostCommand>
{
    public CreateRepostCommandValidator()
    {
        // AuthorId must always be present
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("AuthorId is required");

        // OriginalPostId must always be provided and valid
        RuleFor(x => x.Dto.OriginalPostId)
            .NotEqual(Guid.Empty)
            .WithMessage("OriginalPostId is required");

        // PostType must be Repost or Quote only — Original is invalid here
        RuleFor(x => x.Dto.PostType)
            .Must(t => t == PostType.Repost || t == PostType.Quote)
            .WithMessage("PostType must be either Repost or Quote");

        // Quote requires QuoteContent
        When(x => x.Dto.PostType == PostType.Quote, () =>
        {
            RuleFor(x => x.Dto.QuoteContent)
                .NotEmpty()
                .WithMessage("Quote content is required for a Quote post");

            RuleFor(x => x.Dto.QuoteContent)
                .MaximumLength(3000)
                .WithMessage("Quote content must be less than 3000 characters");
        });

        // Repost must NOT have QuoteContent
        When(x => x.Dto.PostType == PostType.Repost, () =>
        {
            RuleFor(x => x.Dto.QuoteContent)
                .Null()
                .WithMessage("Repost cannot have quote content");
        });
    }
}