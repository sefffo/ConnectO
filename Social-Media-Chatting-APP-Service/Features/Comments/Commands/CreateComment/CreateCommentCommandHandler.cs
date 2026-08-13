using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.Posts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;
using StackExchange.Redis;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var commentRepo = unitOfWork.GetRepository<Comment, Guid>();
        //check the content of the comment first
        if (request.Dto.Content is null && request.Dto.MediaAsset is null)
        {
            return Error.BadRequest("Comment.EmptyContent", "Content cannot be null");
        }

        if (string.IsNullOrEmpty(request.Dto.Content) && request.Dto.MediaAsset is null)
        {
            return  Error.BadRequest("Comment.EmptyContent", "Content cannot be null");
        }
        
        // check if the parentId is Found  in the comment db and if found i need 
        // also check if it's related to this post 
        if (request.Dto.ParentCommentId is not null)
        {
            var isParentCommentFound = await  commentRepo.FindAsync(c=>c.Id == request.Dto.ParentCommentId);
            if (isParentCommentFound == null && isParentCommentFound.PostId != request.Dto.PostId)
            {
                return;
            }

            if (isParentCommentFound.IsDeleted == true)
            {
                return Error.Forbidden("Comment.Forbidden", "user can't reply on deleted comment");
            }        
        }
        
        
        var comment = new Comment()
        {
            Content = request.Dto.Content,
            AuthorId = request.AuthorId.ToString(),
            CreatedAt = DateTime.UtcNow,
            ParentCommentId = request.Dto.ParentCommentId,
            PostId = request.Dto.PostId
        };

        await commentRepo.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();


        // comment can only add one asset per comment 

        if (request.Dto.MediaAsset != null)
        {
            var mediaAssetRepo = unitOfWork.GetRepository<MediaAsset, Guid>();
            //as the upload is done first we take the media asset by finding it (by its public id) and which comment is related to it 
            var asset = await mediaAssetRepo.FindAsync(m => m.PublicId == request.Dto.MediaAsset.PublicId && !m.IsDeleted);
            //u add that as  a ref for it in teh DB in the MA table  
            if (asset is not null)
            {
                asset.CommentId = comment.Id;
                //FK ref
            }
        }

        await unitOfWork.SaveChangesAsync();
        var mappedComment = mapper.Map<CommentDto>(comment);
        return Result<CommentDto>.Ok(mappedComment);
    }
}