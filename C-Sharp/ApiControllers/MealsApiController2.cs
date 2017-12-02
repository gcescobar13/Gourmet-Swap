using GSwap.Models;
using GSwap.Models.Domain.Meals;
using GSwap.Models.Requests.Meals;
using GSwap.Models.Responses;
using GSwap.Services;
using GSwap.Services.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;

namespace GSwap.Web.Controllers.Api.Public.Meals
{
    [RoutePrefix("api/meals")]
    public class MealsApiController : ApiController
    {

        private IMealService2 _mealService;
        private IUserAuthData _currentUser;
        public IPrincipal _principal = null;

        public MealsApiController(IMealService2 mealService, IPrincipal _principal)
        {
            _currentUser = _principal.Identity.GetCurrentUser();
            _mealService = mealService;
        }

  
        [Route(), HttpGet, AllowAnonymous]
        public HttpResponseMessage GetAllMeals()
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<Meal> response = new ItemsResponse<Meal>();

            response.Items = _mealService.GetAllMeals();

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        

        [Route("{mealId:int}/locations"), HttpGet]
        public HttpResponseMessage GetMealLocation(int mealId)
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemResponse<MealLocation> response = new ItemResponse<MealLocation>();

            response.Item = _mealService.GetMealLocation(mealId);
            if (response.Item == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("locations"), HttpGet]
        public HttpResponseMessage GetAllMealLocations()
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<MealLocation> response = new ItemsResponse<MealLocation>();

            response.Items = _mealService.GetAllMealLocations();

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("{id:int}/photo"), HttpGet]
        public HttpResponseMessage GetMealPhoto(int id)
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemResponse<MealPhoto> response = new ItemResponse<MealPhoto>();

            response.Item = _mealService.GetMealPhoto(id);
            if (response.Item == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("{mealId:int}/photos"), HttpGet, AllowAnonymous]
        public HttpResponseMessage GetMealPhotos(int mealId)
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<MealPhoto> response = new ItemsResponse<MealPhoto>();

            response.Items = _mealService.GetMealPhotos(mealId);

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("photos"), HttpGet]
        public HttpResponseMessage GetAllMealPhotos()
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<MealPhoto> response = new ItemsResponse<MealPhoto>();

            response.Items = _mealService.GetAllMealPhotos();

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

        [Route("positions/{lat:double}/{lng:double}/{maxDistance:int}"), HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetMealPositions(double lat, double lng, int maxDistance)
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<MealSearchResult> response = new ItemsResponse<MealSearchResult>();

            response.Items = _mealService.GetMealPositions(lat, lng, maxDistance);

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }


        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage UpdateMeal(MealUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _mealService.Update(model, _currentUser.Id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{mealId:int}/items"), HttpPut]
        public HttpResponseMessage UpdateMealItems(MealItemsAddRequest mealItems, int mealId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }


            _mealService.UpdateItems(mealItems.Items, mealId, _currentUser.Id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{mealId:int}/times"), HttpPut]
        public HttpResponseMessage UpdateMealTimes(MealTimesAddRequest mealTimes, int mealId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }


            _mealService.UpdateTimes(mealTimes.Times, mealId, _currentUser.Id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{id:int}/locations"), HttpPut]
        public HttpResponseMessage UpdateMealLocation(MealLocationUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            _mealService.UpdateLocation(model, _currentUser.Id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("positions/{lat:double}/{lng:double}/{maxDistance:int}/{cuisineId:int}"), HttpGet, AllowAnonymous]
        public HttpResponseMessage GetMealPositionsByCuisine(double lat, double lng, int maxDistance, int cuisineId)
        {
            HttpStatusCode code = HttpStatusCode.OK;

            ItemsResponse<MealSearchResultV2> response = new ItemsResponse<MealSearchResultV2>();

            response.Items = _mealService.GetMealPositionsByCuisine(lat, lng, maxDistance, cuisineId);

            if (response.Items == null)
            {
                code = HttpStatusCode.NotFound;
                response.IsSuccessful = false;
            }

            return Request.CreateResponse(code, response);
        }

      

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage DeleteMeal(int id)
        {            
            _mealService.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK);            
        }


        [Route("{id:int}/locations"), HttpDelete]
        public HttpResponseMessage DeleteMealLocation(int id)
        {
            _mealService.DeleteLocation(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{id:int}/photo"), HttpDelete]
        public HttpResponseMessage DeleteMealPhoto(int id)
        {
            _mealService.DeletePhoto(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("{mealId:int}/photos"), HttpDelete]
        public HttpResponseMessage DeleteMealPhotos(int mealId)
        {
            _mealService.DeletePhotos(mealId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
