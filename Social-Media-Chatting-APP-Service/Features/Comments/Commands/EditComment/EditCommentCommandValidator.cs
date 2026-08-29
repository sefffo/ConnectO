using FluentValidation;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.EditComment;

public class EditCommentCommandValidator : AbstractValidator<EditCommentCommand>
{
    public EditCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("CommentId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.Dto.Content)
            .NotEmpty().WithMessage("Content is required when editing a comment")
            .MaximumLength(2000).WithMessage("Comment content cannot exceed 2000 characters");
    }
}
