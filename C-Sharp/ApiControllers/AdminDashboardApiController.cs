using GSwap.Models;
using GSwap.Models.Domain;
using GSwap.Models.Requests.Users;
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
    [RoutePrefix("api/dashboard")]
    public class AdminDashboardApiController : ApiController
    {

        private IDashboardService _dashboardService;
        private IUserAuthData _currentUser;
        public IPrincipal _principal = null;

        public AdminDashboardApiController(IDashboardService dashboardService, IPrincipal user)
        {
            _dashboardService = dashboardService;
            _principal = user;
            _currentUser = _principal.Identity.GetCurrentUser();
        }


        [Route("users"), HttpGet]
        public HttpResponseMessage GetUsers([FromUri]PagedUsersRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            HttpStatusCode code = HttpStatusCode.OK;

            ItemResponse<PagedList<User>> response = new ItemResponse<PagedList<User>>();

            response.Item = _dashboardService.GetPaginatedUsers(request);

            if (response.Item == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("users/roles/{userId:int}/{role}"), HttpPut]
        public HttpResponseMessage UpdateRole(int userId, string role)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            SuccessResponse success = new SuccessResponse();
            HttpStatusCode code = HttpStatusCode.OK;
            
            if(userId > 0)
            {
                _dashboardService.ToggleChef(userId, role);
                return Request.CreateResponse(HttpStatusCode.OK, success);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse("No UserId submitted"));
            }
          
           
        }
    }
}
