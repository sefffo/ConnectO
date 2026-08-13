using FluentValidation;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        // Either Content or MediaAsset must be present
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Dto.Content) || x.Dto.MediaAsset != null)
            .WithMessage("Comment must have either text content or a media asset");

        // If Content is provided, enforce length cap
        When(x => !string.IsNullOrWhiteSpace(x.Dto.Content), () =>
        {
            RuleFor(x => x.Dto.Content)
                .MaximumLength(3000)
                .WithMessage("Comment content must be less than 3000 characters");
        });

        // PostId must not be empty
        RuleFor(x => x.Dto.PostId)
            .NotEmpty()
            .WithMessage("PostId is required");

        // AuthorId must not be empty
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("AuthorId is required");
    }
}