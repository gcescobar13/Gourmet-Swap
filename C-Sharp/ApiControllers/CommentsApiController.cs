using GSwap.Models;
using GSwap.Models.Domain.Comments;
using GSwap.Models.Requests.Comments;
using GSwap.Models.Responses;
using GSwap.Services;
using GSwap.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;


namespace GSwap.Web.Controllers.Api
{
    [RoutePrefix("api/comments")]
    public class CommentsApiController : ApiController
    {
        private ICommentsService _commentsService;
        private IUserAuthData _currentUser;
        public IPrincipal _principal = null;

        public CommentsApiController(ICommentsService commentsService, IPrincipal user)
        {
            _principal = user;
            _commentsService = commentsService;
            _currentUser = _principal.Identity.GetCurrentUser();

        }

        [Route(), HttpPost]
        public HttpResponseMessage Add(CommentAddRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            int commentId = _commentsService.Add(request, _currentUser.Id);
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = commentId;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(CommentUpdateRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _commentsService.Update(request, _currentUser.Id);

            SuccessResponse response = new SuccessResponse();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [Route("{OwnerId:int}/{OwnerTypeId:int}"), HttpGet]
        public HttpResponseMessage GetMessagesWithChef(int OwnerId, int OwnerTypeId)
        {

            List<ThreadMessage> messages = _commentsService.GetMessagesOwnerAndUserId(_currentUser.Id, OwnerId, OwnerTypeId);

            ItemsResponse<ThreadMessage> response = new ItemsResponse<ThreadMessage>();
            response.Items = messages;

            if (response.Items == null)
            {
                response.IsSuccessful = false;
                return Request.CreateResponse(HttpStatusCode.NotFound, response);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage Get(int id)
        {

            ThreadMessage comment = _commentsService.Get(id);

            ItemResponse<ThreadMessage> response = new ItemResponse<ThreadMessage>();
            response.Item = comment;

            if (response.Item == null)
            {
                response.IsSuccessful = false;
                return Request.CreateResponse(HttpStatusCode.NotFound, response);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage DeleteMe(int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<ThreadMessage> response = new ItemResponse<ThreadMessage>();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("thread/{ownerId:int}/{ownerTypeId:int}/{parentId:int}"), HttpGet]
        public HttpResponseMessage GetAllByOwnerAndParentId(int ownerId, int ownerTypeId, int parentId)
        {
     
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<ThreadMessage> response = new ItemsResponse<ThreadMessage>();
            response.Items = _commentsService.GetAllByOwnerAndParentId(ownerId, ownerTypeId, parentId);

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("threads/{ownerId:int}/{ownerTypeId:int}/{parentId:int}"), HttpGet]
        public HttpResponseMessage GetAll(int ownerId, int ownerTypeId, int parentId)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //}
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<ThreadMessage> response = new ItemsResponse<ThreadMessage>();
            response.Items = _commentsService.GetAllByOwnerAndParentId(ownerId, ownerTypeId, parentId);

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("threads/{ownerId:int}/{ownerTypeId:int}"), HttpGet]
        public HttpResponseMessage GetAllByOwner(int ownerId, int ownerTypeId)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //}
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<ThreadMessage> response = new ItemsResponse<ThreadMessage>();
            response.Items = _commentsService.GetAllByOwnerInfo(ownerId, ownerTypeId);

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }
    }
}
