using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.EditComment;

public class EditCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<EditCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var commentRepo = unitOfWork.GetRepository<Comment, Guid>();

        var comment = await commentRepo.GetByIdAsync(request.CommentId);

        if (comment is null || comment.IsDeleted == true)
            return Error.NotFound("Comment.NotFound", "Comment not found");

        if (comment.AuthorId != request.UserId)
            return Error.Forbidden("Comment.Forbidden", "You are not allowed to edit this comment");

        // Facebook-style rule: media asset is permanently locked to the comment.
        // The ONLY editable field is the text Content.
        // To change or remove the media asset the entire comment must be deleted.
        if (string.IsNullOrWhiteSpace(request.Dto.Content))
            return Error.BadRequest("Comment.EmptyContent", "Content cannot be empty when editing");

        comment.Content = request.Dto.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        commentRepo.Update(comment);
        await unitOfWork.SaveChangesAsync();

        var mappedComment = mapper.Map<CommentDto>(comment);
        return Result<CommentDto>.Ok(mappedComment);
    }
}
