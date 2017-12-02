//using Chronic;
//using GoogleMaps.LocationServices;
//using GSwap.Models.Bot;
//using GSwap.Models.Bot.Domain;
//using GSwap.Models.Domain.Meals;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Luis;
//using Microsoft.Bot.Builder.Luis.Models;
//using Microsoft.Bot.Connector;
//using Microsoft.Bot.Sample.QnABot;
//using Microsoft.IdentityModel.Protocols;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GSwap.Services.Bot
//{
//    [Serializable]
//    public class BotDialogHelper
//    {

//        #region - private members -

//        private const string EntityLocation = "Places.AbsoluteLocation";

//        private const string EntityCuisine = "Places.Cuisine";

//        private const string EntityDistance = "Places.Distance";

//        private const string EntityFood = "foodName";

//        private const string EntityCurrency = "builtin.currency";

//        private ISiteConfigService _siteConfigService;

//        private string BingApiKey
//        {
//            get { return _siteConfigService.BingMapsApiKey; }

//        }


//        private string LuisAppId
//        {
//            get { return _siteConfigService.LuisAppId; }
//        }

//        private string LuisAPIKey
//        {
//            get { return _siteConfigService.LuisAPIKey; }
//        }


//        private string AwsBaseUrl
//        {
//            get { return _siteConfigService.AwsBaseUrl; }
//        }

//        private string GoogleApiKey
//        {
//            get { return _siteConfigService.GoogleMapsApiKey; }
//        }


//        private IExternalService _externalService;
//        #endregion


//        public BotDialogHelper(IExternalService externalService, ISiteConfigService siteConfigService)
//        {
//            _externalService = externalService;
//            _siteConfigService = siteConfigService;
//        }

//        public void PromptForAddressConfirmation(IDialogContext context, BaseRequest request, ResumeAfter<bool> onAddressConfirmation)
//        {
//            string confirmMessage = $"I found '{request.AddressesArray[0]}'. Is this location correct?";

//            context.UserData.SetValue("searchMealRequest", searchMealRequest);
//            context.UserData.Replace(request.Key, request);

//            PromptDialog.Confirm(context, onAddressConfirmation, confirmMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);
//        }

//        public async Task HandleUnkownCuisine(IDialogContext context, BaseRequest model, Dictionary<string, int> cuisineDict, ResumeAfter<string> onNewCuisine)
//        {
//            await context.PostAsync($" {model.Cuisine} is not available. Please check the list of availability and try again.");

//            string anotherCuisine = $"Please choose a one of the cuisines above.";

//            context.UserData.SetValue("mealByDistance", mealByDistance);
//            context.UserData.Replace(model.Key, model);
//            PromptDialog.PromptChoice<string> enterCuisine = new PromptDialog.PromptChoice<string>(cuisineDict.Keys, anotherCuisine, "Retry", 3);
//            search cuisines
//            context.Call(enterCuisine, onNewCuisine);
//        }


//        public void HandleMultiAddress(IDialogContext context, BaseRequest request, ResumeAfter<string> onMultiAddressTask)
//        {
//            string multiAddressMessage = $"I narrowed down {request.AddressesArray.Length} possible locations. Please choose one below.";
//            List<string> list = new List<string>(request.AddressesArray)
//                    {
//                        "None"
//                    };

//            context.UserData.Replace(request.Key, request);

//            PromptDialog.Choice(context, onMultiAddressTask, list, multiAddressMessage);
//        }

//        public void HandleNoMatchedAddress(IDialogContext context, BaseRequest request, ResumeAfter<string> onNewAddress)
//        {
//            string anotherAddressMessage = $"Couldn't find an address that matches the location you provided. Please provide me a more detailed location with a zip code so I can pin point a location.";

//            context.UserData.SetValue("searchMealRequest", searchMealRequest);
//            context.UserData.Replace(request.Key, request);

//            PromptDialog.PromptString askForSecondAddressPrompt = new PromptDialog.PromptString(anotherAddressMessage, "Retry", 3);

//            context.Call(askForSecondAddressPrompt, onNewAddress);
//        }

//        public async Task<SearchMealRequest> GetCuisineByPriceRequest(IDialogContext context, LuisResult result, ResumeAfter<object> messageReceived)
//        {
//            SearchMealRequest cuisineByPrice = new SearchMealRequest();



//            EntityRecommendation cuisineEntityRecommendation = null;

//            EntityRecommendation locationEntityRecommendation = null;

//            EntityRecommendation priceEntityRecommendation = null;



//            if (result.TryFindEntity(EntityLocation, out locationEntityRecommendation))
//            {

//                cuisineByPrice.Location = locationEntityRecommendation.Entity;

//            }
//            else
//            {
//                string locationError = $"Ooops! Something went your. Can you please try again. I didn't catch a location. Type 'help' assistance and search tips.";
//                await context.PostAsync(locationError);
//                context.Wait(messageReceived);

//            }

//            if (result.TryFindEntity(EntityCuisine, out cuisineEntityRecommendation))
//            {
//                cuisineByPrice.Cuisine = cuisineEntityRecommendation.Entity;
//            }
//            else
//            {
//                string cuisineError = $"Ooops! Something went wrong. Can you please try again. I didn't catch a cuisine. Type 'help' assistance and search tips.";
//                await context.PostAsync(cuisineError);
//                context.Wait(messageReceived);

//            }



//            if (result.TryFindEntity(EntityCurrency, out priceEntityRecommendation))
//            {

//                decimal price = 0;
//                Double.TryParse(priceEntityRecommendation.Entity, out price);
//                Decimal.TryParse(priceEntityRecommendation.Resolution["value"].ToString(), out price);
//                cuisineByPrice.Price = price;


//            }

//            return cuisineByPrice;
//        }
//        public async Task<SearchMealRequest> GetFoodByPriceRequest(IDialogContext context, LuisResult result, ResumeAfter<object> messageReceived)
//        {
//            SearchMealRequest searchMealRequest = new SearchMealRequest();

//            EntityRecommendation foodEntityRecommendation = null;

//            EntityRecommendation locationEntityRecommendation = null;

//            EntityRecommendation priceEntityRecommendation = null;

//            if (result.TryFindEntity(EntityLocation, out locationEntityRecommendation))
//            {
//                searchMealRequest.Location = locationEntityRecommendation.Entity;
//            }
//            else
//            {
//                string locationError = $"Ooops! Something went wrong. I didn't catch a location. Type 'help' assistance and search tips.";
//                await context.PostAsync(locationError);

//                context.Wait(messageReceived);
//            }

//            if (result.TryFindEntity(EntityFood, out foodEntityRecommendation))
//            {
//                searchMealRequest.MealName = foodEntityRecommendation.Entity;
//            }
//            else
//            {
//                string cuisineError = $"Ooops! Something went wrong. Can you please try again. I didn't catch a cuisine. Type 'help' assistance and search tips.";
//                await context.PostAsync(cuisineError);
//                context.Wait(messageReceived);

//            }

//            if (result.TryFindEntity(EntityCurrency, out priceEntityRecommendation))
//            {
//                decimal price = 0;
//                Double.TryParse(priceEntityRecommendation.Entity, out price);
//                Decimal.TryParse(priceEntityRecommendation.Resolution["value"].ToString(), out price);
//                searchMealRequest.Price = price;
//            }

//            return searchMealRequest;
//        }

//        public async Task<SearchMealRequest> GetSearchFoodRequest(IDialogContext context, LuisResult result, ResumeAfter<object> messageReceived)
//        {
//            SearchMealRequest searchMealRequest = new SearchMealRequest();

//            EntityRecommendation foodEntityRecommendation = null;

//            EntityRecommendation cityEntityRecommendation = null;


//            if (result.TryFindEntity(EntityFood, out foodEntityRecommendation) && result.TryFindEntity(EntityLocation, out cityEntityRecommendation))
//            {

//                searchMealRequest.MealName = foodEntityRecommendation.Entity;

//                searchMealRequest.Location = cityEntityRecommendation.Entity;

//                string message = $"Hey! Give me second while I search for {searchMealRequest.MealName}. But first, let's confirm the location you provided me.";

//                await context.PostAsync(message);
//            }
//            else
//            {
//                var errorMessage = $"Ooops! Something went your. Can you please try again. Type 'help' assistance and search tips.";

//                await context.PostAsync(errorMessage);

//                context.Wait(messageReceived);

//            }

//            GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

//            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);
//            return searchMealRequest;
//        }
//        public static SearchMealRequest GetSearchCuisineRequest(LuisResult result)
//        {

//            SearchMealRequest searchMealRequest = new SearchMealRequest();
//            EntityRecommendation cuisineEntityRecommendation = null;

//            EntityRecommendation cityEntityRecommendation = null;

//            if (result.TryFindEntity(EntityLocation, out cityEntityRecommendation))
//            {
//                searchMealRequest.Location = cityEntityRecommendation.Entity;
//            }

//            if (result.TryFindEntity(EntityCuisine, out cuisineEntityRecommendation))
//            {
//                searchMealRequest.Cuisine = cuisineEntityRecommendation.Entity;


//            }

//            return searchMealRequest;
//        }
//        public static async Task<SearchMealsByDistance> GetMealByDistanceRequest(IDialogContext context, LuisResult result)
//        {
//            SearchMealsByDistance mealByDistance = new SearchMealsByDistance();



//            EntityRecommendation foodEntityRecommendation = null;

//            EntityRecommendation locationEntityRecommendation = null;

//            EntityRecommendation priceEntityRecommendation = null;



//            EntityRecommendation distanceEntityRecommendation = null;



//            if (result.TryFindEntity(EntityDistance, out distanceEntityRecommendation))
//            {
//                int miles = 0;
//                Double.TryParse(priceEntityRecommendation.Entity, out price);
//                Int32.TryParse(distanceEntityRecommendation.Entity.ToString(), out miles);
//                mealByDistance.Distance = miles;
//            }

//            if (result.TryFindEntity(EntityCurrency, out priceEntityRecommendation))
//            {
//                decimal price = 0;
//                Double.TryParse(priceEntityRecommendation.Entity, out price);
//                Decimal.TryParse(priceEntityRecommendation.Resolution["value"].ToString(), out price);
//                mealByDistance.Price = price;
//            }

//            string welcomeMessage = null;

//            if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
//            {
//                welcomeMessage = $"Give me a second while I look for next meal within {mealByDistance.Distance} miles.";
//            }
//            else
//            {
//                welcomeMessage = $"Give me a second while I look for next meal within {mealByDistance.Distance} under  ${mealByDistance.Price}.";
//            }

//            if (result.TryFindEntity(EntityLocation, out locationEntityRecommendation))
//            {
//                mealByDistance.Location = locationEntityRecommendation.Entity;
//            }


//            if (result.TryFindEntity(EntityFood, out foodEntityRecommendation))
//            {

//                mealByDistance.MealName = foodEntityRecommendation.Entity;
//            }

//            await context.PostAsync(welcomeMessage);
//            return mealByDistance;
//        }
//        public IEnumerable<MealSearchResultV2> GetMealSearchResults(SearchMealsByDistance mealByDistance)
//        {
//            double lat = 0;
//            double lng = 0;
//            _externalService.GeocodeLocation(mealByDistance, out lat, out lng);

//            IEnumerable<MealSearchResultV2> searchedMeals = null;

//            if (mealByDistance.MealName != null)
//            {
//                mealByDistance.Meals = _externalService.SearchMealsList(lat, lng, mealByDistance.Distance);

//                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
//                {
//                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName)).OrderBy(x => x.Distance);
//                }
//                else
//                {
//                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName) && meal.Price != null && meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
//                }

//            }
//            else if (mealByDistance.Cuisine != null)
//            {

//                mealByDistance.Meals = _externalService.SearchMealsByCuisineList(lat, lng, mealByDistance.Distance, mealByDistance.CuisineId);

//                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
//                {
//                    searchedMeals = mealByDistance.Meals.OrderBy(x => x.Distance);
//                }
//                else
//                {
//                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
//                }
//            }

//            return searchedMeals;
//        }
//        public IEnumerable<MealSearchResultV2> GetFiltedMealsByDistanceResults(SearchMealsByDistance mealByDistance, double lat, double lng)
//        {
//            IEnumerable<MealSearchResultV2> searchedMeals = null;

//            if (mealByDistance.MealName != null)
//            {
//                mealByDistance.Meals = _externalService.SearchMealsList(lat, lng, mealByDistance.Distance);

//                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
//                {
//                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName)).OrderBy(x => x.Distance);
//                }
//                else
//                {
//                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName) && meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
//                }

//            }
//            else if (mealByDistance.Cuisine != null)
//            {

//                mealByDistance.Meals = _externalService.SearchMealsByCuisineList(lat, lng, mealByDistance.Distance, mealByDistance.CuisineId);

//                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
//                {
//                    searchedMeals = mealByDistance.Meals.OrderBy(x => x.Distance);
//                }
//                else
//                {
//                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Price != null && meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
//                }
//            }

//            return searchedMeals;
//        }
//        public IEnumerable<MealSearchResultV2> GetMealSearchResults(SearchMealRequest searchMealRequest)
//        {
//            double lat = 0;
//            double lng = 0;
//            _externalService.GeocodeLocation(searchMealRequest, out lat, out lng);

//            IEnumerable<MealSearchResultV2> searchedMeals = null;


//            if (searchMealRequest.MealName != null)
//            {
//                searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

//                searchedMeals = searchMealRequest.Meals.Where(meals => meals.Title.Contains(searchMealRequest.MealName));
//            }
//            else if (searchMealRequest.Cuisine != null)
//            {

//                searchMealRequest.Meals = _externalService.SearchMealsByCuisineList(lat, lng, 20, searchMealRequest.CuisineId);

//                searchedMeals = searchMealRequest.Meals;
//            }

//            return searchedMeals;
//        }


//        public void ResponsedToAddresses(IDialogContext context, SearchMealRequest searchMealRequest)
//        {

//            if (searchMealRequest.AddressesArray.Length == 1)
//            {

//                string confirmMessage = $"I found '{searchMealRequest.AddressesArray[0]}'. Is the location you want me search in?";

//                context.UserData.SetValue("searchMealRequest", searchMealRequest);

//                context.UserData.Replace("searchMealRequest", searchMealRequest);

//                PromptDialog.Confirm(context, MealNameAddressConfirmation, confirmMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);

//            }
//            else
//            {
//                HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);



//            }
//        }

//        public void PromptNewDetailedAddress(IDialogContext context, BaseRequest modelRequest, ResumeAfter<string> onNewEnteredAddress)
//        {
//            string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";

//            /*context.UserData.SetValue("mealByDistance", modelRequest)*/
//            ;
//            context.UserData.Replace(modelRequest.Key, modelRequest);

//            PromptDialog.PromptString prompt = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);

//            context.Call(prompt, onNewEnteredAddress);
//        }

//        public static async Task<string> OtherCuisinePrompt(IDialogContext context, Dictionary<string, int> cuisineDict)
//        {
//            string cuisineNames = "";

//            cuisineDict.Keys.ForEach(x => cuisineNames += x + ", ");

//            cuisineNames.TrimEnd(',');

//            await context.PostAsync($"The cuisines we carry are: {cuisineNames}");

//            string anotherCuisine = $"Please choose a one of the cuisines above.";
//            return anotherCuisine;
//        }

//        public async Task OnEnterOtherCuisine(IDialogContext context, IAwaitable<string> result)
//        {

//            string newCuisine = await result;
//            await context.PostAsync(newCuisine);
//            context.Done(newCuisine);
//        }
//        search cuisine and search food
//        public async Task OnSingleAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
//        {
//            Action<IDialogContext, string> callb
//            bool confirm = await result;

//            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");

//            if (confirm)
//            {

//                await context.PostAsync("Great thank you for confirming your location.");

//                IEnumerable<MealSearchResultV2> searchedMeals = GetMealSearchResults(searchMealRequest);

//                await context.PostAsync($"I found {searchedMeals.Count()} meals near you:");

//                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

//                await context.PostAsync(resultMessage);

//                callb(context, "confirming");
//                context.Wait(MessageReceived);
//            }
//            else
//            {
//                PromptNewDetailedAddress(context, searchMealRequest, OnSecondAddressTask);
//            }


//        }

//        public async Task OnMultiAddressTask(IDialogContext context, IAwaitable<string> result)
//        {

//            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
//            searchMealRequest.Location = await result;

//            if (searchMealRequest.Location != "None")
//            {

//                await context.PostAsync("Great thank you for confirming your location.");

//                double lat = 0;
//                double lng = 0;
//                _externalService.GeocodeLocation(searchMealRequest, out lat, out lng);

//                if (searchMealRequest.MealName != null)
//                {
//                    searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);
//                }
//                else if (searchMealRequest.Cuisine != null)
//                {
//                    searchMealRequest.Meals = _externalService.SearchMealsByCuisineList(lat, lng, 20, searchMealRequest.CuisineId);
//                }


//                await context.PostAsync($"I found {searchMealRequest.Meals.Count()} meals near you:");

//                IMessageActivity resultMessage = GenerateCarousel(context, searchMealRequest.Meals);

//                await context.PostAsync(resultMessage);

//                context.Wait(messageReceived);
//            }
//            else
//            {
//                PromptNewDetailedAddress(context, searchMealRequest, OnSecondAddressTask);
//            }


//        }


//        public async Task OnSecondAddressTask(IDialogContext context, IAwaitable<string> result)
//        {

//            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");

//            searchMealRequest.Location = await result;

//            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);

//            if (searchMealRequest.AddressesArray != null)
//            {
//                if (searchMealRequest.AddressesArray.Length == 1)
//                {
//                    PromptForAddressConfirmation(context, searchMealRequest, OnSingleAddressConfirmation);

//                }
//                else
//                {
//                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);
//                }
//            }


//        }
//        public async Task OnSecondAddressByDistanceTask(IDialogContext context, IAwaitable<string> result)
//        {
//            string addressSelected = await result;

//            SearchMealsByDistance mealByDistance = context.UserData.GetValue<SearchMealsByDistance>("mealByDistance");

//            GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

//            mealByDistance.AddressesArray = _externalService.GetAddressesArray(addressSelected);

//            if (mealByDistance.AddressesArray != null)
//            {
//                if (mealByDistance.AddressesArray.Length == 1)
//                {
//                    string singleAddressMessage = $"I found '{mealByDistance.AddressesArray[0]}'. Is this the location you want me to search in?";

//                    context.UserData.SetValue("mealByDistance", mealByDistance);
//                    context.UserData.Replace("mealByDistance", mealByDistance);

//                    PromptDialog.Confirm(context, MealByDistanceAddressConfirmation, singleAddressMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);
//                }
//                else
//                {
//                    HandleMultiAddress(context, mealByDistance, OnMultiAddressTask);
//                }
//            }


//        }
//        public async Task OnSecondAddressFoodByPrice(IDialogContext context, IAwaitable<string> result)
//        {

//            string addressSelected = await result;
//            var searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
//            GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

//            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(addressSelected);

//            if (searchMealRequest.AddressesArray != null)
//            {
//                if (searchMealRequest.AddressesArray.Length == 1)
//                {
//                    string singleAddressMessage = $"I found '{searchMealRequest.AddressesArray[0]}'. Is this the location you want me to search in?";
//                    PromptDialog.Confirm(context, SingleAddressConfirmationFoodByPrice, singleAddressMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);

//                }
//                else
//                {
//                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);

//                }
//            }


//        }

//        public async Task ReturnMealSearchResults(IDialogContext context, SearchMealRequest searchMealRequest, double lat, double lng)
//        {
//            searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

//            capture mathsign lessthan or greaer than 1:02am
//            IEnumerable<MealSearchResultV2> searchedMeals = searchMealRequest.Meals.Where(meal => meal.Title.Contains(searchMealRequest.MealName) && meal.Price != null && meal.Price <= searchMealRequest.Price).OrderBy(x => x.Price);

//            if (searchedMeals != null && searchedMeals.Count() > 0)
//            {
//                await context.PostAsync($"I found {searchedMeals.Count()} meals near you under ${searchMealRequest.Price}:");

//                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

//                await context.PostAsync(resultMessage);

//                context.Wait(MessageReceived);
//            }
//            else
//            {
//                var noMealsMessage = $"I couldn't find {searchMealRequest.MealName} near {searchMealRequest.Location}. Try searching for another food.";

//                await context.PostAsync(noMealsMessage);

//                context.Wait(MessageReceived);
//            }
//        }
//        public async Task ReturnMealSearchResults(IDialogContext context, IEnumerable<MealSearchResultV2> searchedMeals, ResumeAfter<object> messageReceived)
//        {
//            if (searchedMeals != null && searchedMeals.Count() > 0)
//            {
//                await context.PostAsync($"I found {searchedMeals.Count()} meals near you:");

//                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

//                await context.PostAsync(resultMessage);

//                context.Wait(messageReceived);
//            }
//            else
//            {
//                string noMealsMessage = $"Ooop! It looks like there's no meals that match your search. Try searching for another food or cuisine, or expand your radius.";

//                await context.PostAsync(noMealsMessage);

//                context.Wait(messageReceived);
//            }
//        }

//        public async Task SingleAddressConfirmationFoodByPrice(IDialogContext context, IAwaitable<bool> result)
//        {
//            bool confirm = await result;
//            string userAddress = null;

//            if (confirm)
//            {


//                await context.PostAsync("Great thank you for confirming your location.");

//                GoogleLocationService locationService = new GoogleLocationService("AIzaSyCgv9wO_gY768dq28ZRf_YRykSsiUF2j2Q");
//                GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

//                MapPoint point = locationService.GetLatLongFromAddress(userAddress);

//                double lat = point.Latitude;
//                double lng = point.Longitude;


//                IEnumerable<MealSearchResultV2> searchedMeals = null;

//                await context.PostAsync($"I found {searchedMeals.Count()} meals near you:");

//                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

//                await context.PostAsync(resultMessage);

//                context.Wait(messageReceived);
//            }
//            else
//            {
//                string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";
//                PromptDialog.PromptString prompt = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);
//                context.Call(prompt, OnSecondAddressTask);
//            }


//        }
//        public async Task FoodByPriceAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
//        {
//            bool confirm = await result;
//            var searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
//            if (confirm)
//            {
//                searchMealRequest.Location = searchMealRequest.AddressesArray[0];

//                await context.PostAsync("Great thank you for confirming your address.");

//                double lat = 0;
//                double lng = 0;
//                _externalService.GeocodeLocation(searchMealRequest, out lat, out lng);

//                searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

//                capture mathsign lessthan or greaer than 1:02am
//                IEnumerable<MealSearchResultV2> searchedMeals = searchMealRequest.Meals.Where(meal => meal.Title.Contains(searchMealRequest.MealName) && meal.Price != null && meal.Price <= searchMealRequest.Price).OrderBy(x => x.Price);

//                await ReturnMealSearchResults(context, searchMealRequest, lat, lng);

//            }
//            else
//            {

//                PromptNewDetailedAddress(context, searchMealRequest, OnSecondAddressTask);
//            }

//        }
//        public async Task MealByDistanceAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
//        {
//            bool confirm = await result;

//            SearchMealsByDistance mealByDistance = context.UserData.GetValue<SearchMealsByDistance>("searchMealRequest");

//            if (confirm)
//            {
//                mealByDistance.Location = mealByDistance.AddressesArray[0];

//                await context.PostAsync("Great thank you for confirming your address.");
//                IEnumerable<MealSearchResultV2> searchedMeals = GetMealSearchResults(mealByDistance);

//                IEnumerable<MealSearchResultV2> searchedMeals = await GetFiltedMealsByDistance(mealByDistance, lat, lng);


//                capture mathsign lessthan or greaer than 1:02am

//                await ReturnMealSearchResults(context, searchedMeals);

//            }
//            else
//            {

//                PromptNewDetailedAddress(context, mealByDistance, OnSecondAddressByDistanceTask);
//            }

//        }
//        public async Task CuisineByPriceAddressConfirmation(IDialogContext context, IAwaitable<bool> result, ResumeAfter<object> messageReceived)
//        {
//            bool confirm = await result;
//            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
//            if (confirm)
//            {
//                searchMealRequest.Location = searchMealRequest.AddressesArray[0];

//                await context.PostAsync("Great thank you for confirming your address.");

//                GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

//                MapPoint point = locationService.GetLatLongFromAddress(searchMealRequest.Location);

//                double lat = point.Latitude;
//                double lng = point.Longitude;

//                searchMealRequest.Meals = _externalService.SearchMealsByCuisineList(lat, lng, 100, searchMealRequest.CuisineId);

//                capture mathsign lessthan or greaer than 1:02am
//                IEnumerable<MealSearchResultV2> searchedMeals = searchMealRequest.Meals.Where(meal => meal.Price <= searchMealRequest.Price);

//                if (searchedMeals != null && searchedMeals.Count() > 0)
//                {
//                    await context.PostAsync($"I found {searchedMeals.Count()} {searchMealRequest.Cuisine} meals near you:");
//                }
//                else
//                {
//                    var noMealsMessage = $"I couldn't find {searchMealRequest.Cuisine} food near {searchMealRequest.Location}. Try searching for another cuisine.";
//                    await context.PostAsync(noMealsMessage);
//                    option to research cuisine since 0 came back

//                }


//                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

//                await context.PostAsync(resultMessage);

//                context.Wait(messageReceived);
//            }
//            else
//            {
//                string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";

//                context.UserData.SetValue("cuisineByPrice", cuisineByPrice);
//                context.UserData.Replace("cuisineByPrice", searchMealRequest);

//                PromptDialog.PromptString zipCodePrompt = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);
//                search food


//                context.Call(zipCodePrompt, OnSecondAddressTask);
//            }


//        }
//        public async Task MealNameAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
//        {
//            bool confirm = await result;
//            SearchMealNameRequest mealRequest = context.UserData.GetValue<SearchMealNameRequest>("searchMealRequest");
//            if (confirm)
//            {
//                await context.PostAsync("Great thank you for confirming your address.");

//                double lat = 0;
//                double lng = 0;
//                _externalService.GeocodeLocation(mealRequest, out lat, out lng);

//                mealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

//                IEnumerable<MealSearchResultV2> searchedMeals = mealRequest.Meals.Where(meals => meals.Title.Contains(mealRequest.MealName));

//                await ReturnMealSearchResults(context, searchedMeals);

//            }
//            else
//            {
//                string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";
//                var fdf = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);
//                search food
//                context.Call(fdf, OnSecondAddressTask);
//            }


//        }


//        public IMessageActivity GenerateCarousel(IDialogContext context, IEnumerable<MealSearchResultV2> searchedMeals)
//        {
//            IMessageActivity resultMessage = context.MakeMessage();
//            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
//            resultMessage.Attachments = new List<Attachment>();

//            foreach (MealSearchResultV2 meal in searchedMeals)
//            {
//                CardImage cardImage = new CardImage() { Url = AwsBaseUrl + meal.Photo };

//                CardAction cardAction = new CardAction()
//                {
//                    Title = "More details",
//                    Type = ActionTypes.OpenUrl,
//                    Value = AwsBaseUrl + meal.Photo

//                };

//                string distanceInfo = null;
//                if (meal.Price != null)
//                {
//                    distanceInfo = $"${meal.Price} - {Math.Round(meal.Distance, 2)} mi - {meal.Address.City}";
//                }
//                else
//                {
//                    distanceInfo = $"{Math.Round(meal.Distance, 2)} mi - {meal.Address.City}";
//                }


//                Attachment heroCardAttachment = GetHeroCard(meal.Title, meal.Description, distanceInfo, cardImage, cardAction);

//                resultMessage.Attachments.Add(heroCardAttachment);
//            }

//            return resultMessage;
//        }
//        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
//        {
//            HeroCard heroCard = new HeroCard
//            {
//                Title = title,
//                Subtitle = subtitle,
//                Text = text,
//                Images = new List<CardImage>() { cardImage },
//                Buttons = new List<CardAction>() { cardAction },
//            };

//            return heroCard.ToAttachment();
//        }
//    }


//}
