using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSwap.Models.Requests.Users;
using GSwap.Models.Responses;
using System.Web.Http.ModelBinding;
using GSwap.Services;
using GSwap.Models;
using System.Security.Principal;
using GSwap.Services.Security;
using GSwap.Models.Domain.Addresses;
using GSwap.Models.Domain.Users;
using GSwap.Models.Domain.Comments;
using GSwap.Models.Domain;
using GSwap.Models.Domain.ServicesOffered;
using System.Threading.Tasks;

namespace GSwap.Web.Controllers.Api.Users
{

    [RoutePrefix("api/users")]
    public class UsersApiController : ApiController
    {
        private IUserService _userService;
        private IAuthenticationService _auth;
        private IAddressService _addressService;
        private IEmailService _emailService;
        private ITokenService _tokenService;
        private IPrincipal _principal = null;
        private IUserAuthData _currentUser;
        private ICommentsService _commentsService;

        public UsersApiController(IUserService userService, IAuthenticationService auth, IPrincipal principal, IAddressService addressService, IEmailService emailService, ICommentsService commentsService,
            ITokenService tokenService)
        {
            _userService = userService;
            _auth = auth;
            _principal = principal;

            _addressService = addressService;
            _emailService = emailService;
            _tokenService = tokenService;

            _currentUser = _principal.Identity.GetCurrentUser();
            _commentsService = commentsService;
        }

        
        [Route("register/{role}"), HttpPost, AllowAnonymous]
        public async Task<HttpResponseMessage> Add(UserAddRequest request, string role = null)
        {
            int userId = 0;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            };

            //fix create below
            
            try
            {
                userId = _userService.Create(request, role);
            }
            catch (Exception)
            {
                ErrorResponse error = new ErrorResponse("There was an error creating your account.");
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
             }



            int TokenTypeId = 2;
            Guid getToken = _tokenService.GenerateToken(userId, TokenTypeId);
            bool result = await _emailService.ConfirmationSend(request, getToken);

            ItemResponse<int> resp = new ItemResponse<int>();

            resp.Item = userId;

            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }


        [Route("login"), HttpPost, AllowAnonymous]
        public HttpResponseMessage Log(LoginRequest request)
        {

            if (!ModelState.IsValid || request == null)
            {
                if (request == null)
                {
                    ErrorResponse err = new ErrorResponse("request is null");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, err);

                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }



            bool result = _userService.LogIn(request.Email, request.Password);
            ItemResponse<bool> response = new ItemResponse<bool>();

            if (!result)
            {
                response.Item = result;
                response.IsSuccessful = result;
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }


            response.Item = result;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("cookie"), HttpGet, AllowAnonymous]
        public string GetCookie()
        {
            string result = _userService.GetCookie();
            return result;
        }



        [Route("confirmation/{token:guid}"), HttpPost, AllowAnonymous]
        public HttpResponseMessage Confirm(Guid token)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            };

            int userId = _tokenService.GetIdByGuid(token);
            

            if (userId == 0)
            {
                ItemResponse<bool> response = new ItemResponse<bool>();

                response.Item = false;

                return Request.CreateResponse(HttpStatusCode.NotFound, response);
            }

            else
            {
                ItemResponse<int> response = new ItemResponse<int>();

                response.Item = userId;
               
                _tokenService.SetIsConfirmed(userId);

                int tokenTypeId = 2;

                _tokenService.TokenDelete(userId, tokenTypeId);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            
            
        }

        
        [Route("logout"), HttpGet]
        public HttpResponseMessage Logout()
        {
            _auth.LogOut();

            return Request.CreateResponse(HttpStatusCode.OK, new GSwap.Models.Responses.SuccessResponse());
        }

        [Route("current/full"), HttpGet]
        public HttpResponseMessage GetUser()
        {

            HttpStatusCode code = HttpStatusCode.OK;

            ItemResponse<UserInfo> response = new ItemResponse<UserInfo>();

            response.Item = _userService.GetUser(_currentUser.Id);
            if (response.Item == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("current"), HttpGet]
        public HttpResponseMessage Current()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _principal.Identity.GetCurrentUser());
        }

       

        [Route("admin/pagedlist"), HttpGet]
        public HttpResponseMessage GetPagedUsers([FromUri]PagedUsersRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            HttpStatusCode code = HttpStatusCode.OK;

            ItemResponse<PagedList<User>> response = new ItemResponse<PagedList<User>>();

            response.Item = _userService.GetPaginatedUsers(request);

            if (response.Item == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("admin/disable"), HttpPut]
        public HttpResponseMessage ChangeDisableStatus(UpdateDisableStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _userService.ChangeUserDisableStatus(request);

            return Request.CreateResponse(HttpStatusCode.OK, new GSwap.Models.Responses.SuccessResponse());
        }

    }
}

