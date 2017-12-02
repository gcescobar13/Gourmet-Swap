using GSwap.Models.Domain.Comments;
using GSwap.Models.Requests.Comments;
using System.Collections.Generic;

namespace GSwap.Services
{
    public interface ICommentsService
    {
        List<ThreadMessage> GetMessagesOwnerAndUserId(int UserId, int OwnerId, int OwnerTypeId);
        int Add(CommentAddRequest request, int userId);
        void Update(CommentUpdateRequest request, int id);
        ThreadMessage Get(int id);
        List<ThreadMessage> GetAllByOwnerAndParentId(int ownerId, int ownerTypeId, int parentId);
        List<ThreadMessage> GetAllByOwnerInfo(int ownerId, int ownerTypeId);
    }
}