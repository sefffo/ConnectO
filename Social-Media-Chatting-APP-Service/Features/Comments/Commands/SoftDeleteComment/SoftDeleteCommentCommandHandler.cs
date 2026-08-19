using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.SoftDeleteComment;

public class SoftDeleteCommentCommandHandler (
    IUnitOfWork unitOfWork
    ) : IRequestHandler<SoftDeleteCommentCommand , Result<string>>
{
    public async Task<Result<string>> Handle(SoftDeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();
        var post = await postRepo.GetByIdAsync(request.PostId);
        if (post == null)
        {
            return Error.NotFound("Comment.Post.NotFound","Post not found");
        }
        var commentRepo = unitOfWork.GetRepository<Comment, Guid>();
        // check if comment is found 
        var comment = await commentRepo.GetByIdAsync(request.CommentId);
        if (comment == null)
        {
            return Error.NotFound("Comment.NotFound","Comment not found");
        }

        if (comment.AuthorId != request.AuthorId.ToString() )
        {
            return Error.Forbidden("Comment.AuthorId", "you can't delete this comment");
        }
        
        comment.DeletedAt = DateTime.UtcNow;
        comment.IsDeleted = true;
        commentRepo.Update(comment);
        await unitOfWork.SaveChangesAsync();
        return  Result<string>.Ok("Comment.Deleted");

    }
}