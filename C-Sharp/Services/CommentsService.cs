using GSwap.Data;
using GSwap.Data.Providers;
using GSwap.Models.Domain.Comments;
using GSwap.Models.Requests.Comments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Services
{
    public class CommentsService : ICommentsService
    {
        private IDataProvider _dataProvider;

        public CommentsService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }



        public int Add(CommentAddRequest request, int userId)
        {
            int commentId = 0;
            _dataProvider.ExecuteNonQuery("dbo.Comments_Insert"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Title", request.Title);
                    paramCollection.AddWithValue("@Comment", request.Comment);
                    paramCollection.AddWithValue("@ParentId", request.ParentId);
                    paramCollection.AddWithValue("@OwnerId", request.OwnerId);
                    paramCollection.AddWithValue("@OwnerTypeId", request.OwnerTypeId);
                    paramCollection.AddWithValue("@UserId", userId);


                    SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int);
                    idParameter.Direction = ParameterDirection.Output;

                    paramCollection.Add(idParameter);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    Int32.TryParse(param["@Id"].Value.ToString(), out commentId);
                }
                );

            return commentId;

        }

        public void Update(CommentUpdateRequest request, int userId)
        {

            _dataProvider.ExecuteNonQuery("dbo.Comments_Update"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Title", request.Title);
                    paramCollection.AddWithValue("@Comment", request.Comment);
                    paramCollection.AddWithValue("@ParentId", request.ParentId);
                    paramCollection.AddWithValue("@OwnerId", request.OwnerId);
                    paramCollection.AddWithValue("@OwnerType", request.OwnerTypeId);
                    paramCollection.AddWithValue("@UserId", userId);
                    paramCollection.AddWithValue("@Id", request.Id);

                }, returnParameters: delegate (SqlParameterCollection param)
                {

                }
                );

        }

        private static ThreadMessage MessageMapper(IDataReader reader)
        {
            ThreadMessage comment = new ThreadMessage();
            int startingIndex = 0;

            comment.Id = reader.GetSafeInt32(startingIndex++);
            comment.Title = reader.GetSafeString(startingIndex++);
            comment.Comment = reader.GetSafeString(startingIndex++);
            comment.ParentId = reader.GetSafeInt32(startingIndex++);
            comment.OwnerId = reader.GetSafeInt32(startingIndex++);
            comment.OwnerTypeId = reader.GetSafeInt32(startingIndex++);
            comment.UserId = reader.GetSafeInt32(startingIndex++);
            comment.DateAdded = reader.GetSafeDateTime(startingIndex++);
            comment.DateModified = reader.GetSafeDateTime(startingIndex++);




            return comment;
        }

        private List<ThreadMessage> NestedRecursion(List<ThreadMessage> list, int parentId)
        {
            List<ThreadMessage> result = new List<ThreadMessage>();

            foreach (var comment in list)
            {
                if (comment.ParentId == parentId)
                {
                    List<ThreadMessage> children = NestedRecursion(list, comment.Id);

                    if (children.Count > 0)
                    {
                        comment.Replies = children;
                    }

                    result.Add(comment);

                }
            }
            return result;

        }

        public ThreadMessage Get(int id)
        {
            //get targetmessage
            //fill list
            //by parent 
            ThreadMessage targetMessage = null;
            List<ThreadMessage> allMessages = null;
            Dictionary<int, List<ThreadMessage>> dict = null;
            //kepp a list of children by parent Id
            //except those with parents of null have key 0

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
                //strings have to match the stored proc parameter names
            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {

                ThreadMessage comment = MessageMapper(reader);
                int parentId = comment.ParentId.HasValue ? comment.ParentId.Value : 0;

                if (comment.Id == id)
                {
                    targetMessage = comment;
                }
                if (allMessages == null)
                {
                    allMessages = new List<ThreadMessage>();
                }
                if (dict == null)
                {
                    dict = new Dictionary<int, List<ThreadMessage>>();
                }
                allMessages.Add(comment);

                if (!dict.ContainsKey(parentId))
                {
                    dict.Add(parentId, new List<ThreadMessage>());
                }
                dict[parentId].Add(comment);


            };

            _dataProvider.ExecuteCmd("dbo.Comments_SelectById", inputParamDelegate, singleRecMapper);

            if (allMessages != null)
            {
                foreach (ThreadMessage currentMessage in allMessages)
                {

                    if (dict.ContainsKey(currentMessage.Id))
                    {
                        currentMessage.Replies = dict[currentMessage.Id];
                    }
                }
            }

            return targetMessage;
        }


        public List<ThreadMessage> GetMessagesOwnerAndUserId(int UserId, int OwnerId, int OwnerTypeId)
        {
            List<ThreadMessage> allMessages = null;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserId", UserId);
                paramCollection.AddWithValue("@OwnerId", OwnerId);
                paramCollection.AddWithValue("@OwnerTypeId", OwnerTypeId);

            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {

                ThreadMessage comment = MessageMapper(reader);

                if (allMessages == null)
                {
                    allMessages = new List<ThreadMessage>();
                }

                allMessages.Add(comment);

            };

            _dataProvider.ExecuteCmd("dbo.Comments_GetCommentsByUserIdAndChefId", inputParamDelegate, singleRecMapper);

            return allMessages;

        }



        //copy this change it
        public List<ThreadMessage> GetAllByOwnerAndParentId(int ownerId, int ownerTypeId, int parentId)
        {
            List<ThreadMessage> allMessages = null;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@OwnerId", ownerId);
                paramCollection.AddWithValue("@OwnerTypeId", ownerTypeId);
                paramCollection.AddWithValue("@ParentId", parentId);

            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {

                ThreadMessage comment = MessageMapper(reader);

                if (allMessages == null)
                {
                    allMessages = new List<ThreadMessage>();
                }

                allMessages.Add(comment);

            };

            _dataProvider.ExecuteCmd("dbo.Comments_GetAllByOwnerIdOwnerTypeIdParentId", inputParamDelegate, singleRecMapper);

            return allMessages;

        }

        public List<ThreadMessage> GetAllByOwnerInfo(int ownerId, int ownerTypeId)
        {
            List<ThreadMessage> allMessages = null;
            List<Author> allAuthors = null;
            Dictionary<int, Author> dict = null;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@OwnerId", ownerId);
                paramCollection.AddWithValue("@OwnerTypeId", ownerTypeId);

            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                if (set == 0)
                {
                    Author author = new Author();
                    int startingIndex = 0;
                    author.Id = reader.GetSafeInt32(startingIndex++);
                    author.FirstName = reader.GetSafeString(startingIndex++);
                    author.LastName = reader.GetSafeString(startingIndex++);

                    if (allAuthors == null)
                    {
                        allAuthors = new List<Author>();
                    }

                    allAuthors.Add(author);
                    if (dict == null)
                    {
                        dict = new Dictionary<int, Author>();
                    }
                    dict[author.Id] = author;

                }

                if (set == 1)
                {
                    ThreadMessage comment = MessageMapper(reader);

                    if (allMessages == null)
                    {
                        allMessages = new List<ThreadMessage>();
                    }

                    allMessages.Add(comment);

                    if (dict.ContainsKey(comment.UserId))
                    {
                        comment.AuthorInfo = dict[comment.UserId];
                    }
                }

            };

            _dataProvider.ExecuteCmd("dbo.Comments_GetAllByOwnerIdOwnerTypeId_V2", inputParamDelegate, singleRecMapper);

            return allMessages;

        }




    }
}
