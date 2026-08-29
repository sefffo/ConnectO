using FluentValidation;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.EditPost;

public class EditPostCommandValidator : AbstractValidator<EditPostCommand>
{
    public EditPostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("PostId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        // At least one editable field must be provided
        RuleFor(x => x)
            .Must(x => x.Dto.Content is not null || x.Dto.QuoteContent is not null)
            .WithMessage("At least one field (Content or QuoteContent) must be provided to update");

        RuleFor(x => x.Dto.Content)
            .MaximumLength(5000).When(x => x.Dto.Content is not null)
            .WithMessage("Content cannot exceed 5000 characters");
    }
}
