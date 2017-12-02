using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Web.Configuration;
using GoogleMaps.LocationServices;
using QnABot.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs.Internals;
using QnABot.Models.MealPositions;
using Newtonsoft.Json;
using Chronic.Handlers;
using GSwap.Models.Domain.Cuisines;
using GSwap.Models.Responses;
using Microsoft.Bot.Builder.Location;
using GSwap.Models.Domain.Meals;
using Chronic;
using GSwap.Models.Bot;
using GSwap.Services;
using GSwap.Models.Bot.Domain;

namespace Microsoft.Bot.Sample.QnABot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-qnamaker
    //[LuisModel("c446f04c-fc34-40d9-8853-3947206e7e91", "7c9c6106d0144f42b132891ae3749be2")]
    [Serializable]
    [BotAuthentication]
    public partial class BasicQnAMakerDialog : LuisDialog<object>
    {
        #region - private members -


        private const string EntityLocation = "Places.AbsoluteLocation";

        private const string EntityCuisine = "Places.Cuisine";

        private const string EntityDistance = "Places.Distance";

        private const string EntityFood = "foodName";

        private const string EntityCurrency = "builtin.currency";

        //private Dictionary<string, int> cuisineDict = null;

        private string _channelId;

        private IGetMeals _getMeals;

        private ISiteConfigService _siteConfigService;

        private IExternalService _externalService;

        private string AwsBaseUrl
        {
            get { return _siteConfigService.AwsBaseUrl; }
        }

        private string GoogleApiKey
        {
            get { return _siteConfigService.GoogleMapsApiKey; }
        }

        private string SiteDomain
        {
            get { return _siteConfigService.SiteDomain; }
        }

        private string BingAPIKey
        {
            get { return _siteConfigService.BingMapsApiKey; }
        }


        private string luisAppId = ConfigurationManager.AppSettings["LuisAppId"];

        private string luisAPIKey = ConfigurationManager.AppSettings["LuisAPIKey"];
        #endregion

        //1. refactoring -- all request models need a "BaseRequest" class with 'base' properties and key property (hard code it)
        //2. doing step 1 will allow use to stop repeats (i.e. HandleUnknownRequest)

        public BasicQnAMakerDialog(string channelId, IGetMeals getMeals, ISiteConfigService siteConfigService, IExternalService externalService) : base(new LuisService(new LuisModelAttribute("c446f04c-fc34-40d9-8853-3947206e7e91", "7c9c6106d0144f42b132891ae3749be2")))
        {
            _channelId = channelId;
            _getMeals = getMeals;
            _siteConfigService = siteConfigService;
            _externalService = externalService;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            //helper(delegate(IDialogContext c, string cmd) {

            //    if(cmd == "")
            //    {

            //    }



            //});
        }


        [LuisIntent("greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = $"Hey, I'm here to help you find your next meal! Feel free to search for meals and I will bring you back meal results. Need help? Type 'help' for search tips. ";

            await context.PostAsync(message);

            context.Wait(MessageReceived);
        }

        [LuisIntent("bootcamp")]
        public async Task Bootcamp(IDialogContext context, LuisResult result)
        {
            string message = $"Don't you know?! Gregorio's favorite boot camp is Code Smith :)";

            await context.PostAsync(message);
            IMessageActivity resultMessage = context.MakeMessage();

            resultMessage.Attachments.Add(new Attachment()
            {
                ContentUrl = "https://d3c5s1hmka2e2b.cloudfront.net/uploads/topic/image/438/codesmith_logo.png",
                ContentType = "image/png",
                Name = "Bender_Rodriguez.png"
            });

            await context.PostAsync(resultMessage);

            context.Wait(MessageReceived);
        }
        [LuisIntent("search cuisines")]
        public async Task SearchCuisines(IDialogContext context, LuisResult result)
        {

            string message = $"Welcome to the Cuisine finder! Give me a second as I analyze your cusine search: '{result.Query}'...";

            await context.PostAsync(message);

            SearchMealRequest searchMealRequest = GetSearchCuisineRequest(result);


            Dictionary<string, int> cuisineDict = _externalService.CuisineDictionary();

            if (!cuisineDict.ContainsKey(searchMealRequest.Cuisine))
            {
                await HandleUnkownCuisine(context, searchMealRequest, cuisineDict, OnEnterOtherCuisine);

            }
            else
            {
                searchMealRequest.CuisineId = cuisineDict[searchMealRequest.Cuisine];

                searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);

                if (searchMealRequest.AddressesArray != null)
                {
                    if (searchMealRequest.AddressesArray.Length == 1)
                    {
                        PromptForAddressConfirmation(context, searchMealRequest, OnSingleAddressConfirmation);
                    }
                    else
                    {
                        HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);

                    }
                }
                else
                {
                    HandleNoMatchedAddress(context, searchMealRequest, OnSecondAddressTask);
                }
            }

        }

        [LuisIntent("help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Let's find your next meal. Try asking me things like 'search for Italian cuisine in Seattle', 'search American food near Los Angeles, CA' or 'find sushi near 400 Culver Point, Culver City, CA within 10 miles and under $50'.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("search food")]
        public async Task SearchFood(IDialogContext context, LuisResult result)
        {
            //var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];

            //var options = LocationOptions.UseNativeControl | LocationOptions.ReverseGeocode;

            //var requiredFields = LocationRequiredFields.StreetAddress | LocationRequiredFields.Locality |
            //                     LocationRequiredFields.Region | LocationRequiredFields.Country |
            //                     LocationRequiredFields.PostalCode;

            //var prompt = "Where should I ship your order?";

            //LocationResourceManager lrm = new LocationResourceManager();
            ////location
            ////LocationCardBuilder card = new LocationCardBuilder(bingApiKey, lrm);
            //LocationDialog locationDialog = new LocationDialog(apiKey, "emulator", prompt, options, requiredFields);

            //context.Call(locationDialog, this.ResumeAfterLocationDialogAsync);


            SearchMealRequest searchMealRequest = await GetSearchFoodRequest(context, result);

            if (searchMealRequest.AddressesArray != null)
            {
                if (searchMealRequest.AddressesArray.Length == 1)
                {
                    PromptForAddressConfirmation(context, searchMealRequest, MealNameAddressConfirmation);
                }
                else
                {
                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);
                }
            }
            else
            {
                HandleNoMatchedAddress(context, searchMealRequest, OnSecondAddressTask);
            }

        }

        [LuisIntent("search cuisines by price")]
        private async Task CuisinesByPrice(IDialogContext context, LuisResult result)
        {
            SearchMealRequest searchMealRequest = await GetCuisineByPriceRequest(context, result);

            string welcomeMessage = $"Give me a second while I look for {searchMealRequest.Cuisine} in {searchMealRequest.Location} for under  ${searchMealRequest.Price}.";

            await context.PostAsync(welcomeMessage);

            Dictionary<string, int> cuisineDict = _externalService.CuisineDictionary();

            if (!cuisineDict.ContainsKey(searchMealRequest.Cuisine))
            {
                await HandleUnkownCuisine(context, searchMealRequest, cuisineDict, OnEnterOtherCuisine);

            }

            searchMealRequest.CuisineId = cuisineDict[searchMealRequest.Cuisine];

            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);


            if (searchMealRequest.AddressesArray != null || searchMealRequest.AddressesArray.Count() > 0)
            {
                if (searchMealRequest.AddressesArray.Count() == 1)
                {
                    searchMealRequest.Location = searchMealRequest.AddressesArray[0];
                    PromptForAddressConfirmation(context, searchMealRequest, CuisineByPriceAddressConfirmation);
                }
                else
                {
                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);

                }
            }
            else
            {
                HandleNoMatchedAddress(context, searchMealRequest, OnSecondAddressTask);

            }

        }

        [LuisIntent("search food by price")]
        private async Task FoodByPrice(IDialogContext context, LuisResult result)
        {
            SearchMealRequest searchMealRequest = await GetFoodByPriceRequest(context, result);

            string welcomeMessage = $"Give me a second while I look for '{searchMealRequest.MealName} in {searchMealRequest.Location} for under  ${searchMealRequest.Price}.";

            await context.PostAsync(welcomeMessage);

            //checking location


            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);


            if (searchMealRequest.AddressesArray != null || searchMealRequest.AddressesArray.Length > 0)
            {
                if (searchMealRequest.AddressesArray.Length == 1)
                {
                    PromptForAddressConfirmation(context, searchMealRequest, FoodByPriceAddressConfirmation);
                }
                else
                {
                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);

                }
            }
            else
            {
                HandleNoMatchedAddress(context, searchMealRequest, OnSecondAddressTask);
            }

        }

        [LuisIntent("find cuisine/food by price and distance")]
        private async Task MealsByPriceAndDistance(IDialogContext context, LuisResult result)
        {
            SearchMealsByDistance mealByDistance = await GetMealByDistanceRequest(context, result);

            await context.PostAsync("But first, let's confirm your location.");

            EntityRecommendation cuisineEntityRecommendation = null;

            if (result.TryFindEntity(EntityCuisine, out cuisineEntityRecommendation))
            {

                mealByDistance.Cuisine = cuisineEntityRecommendation.Entity;

                Dictionary<string, int> cuisineDict = _externalService.CuisineDictionary();

                if (!cuisineDict.ContainsKey(mealByDistance.Cuisine))
                {
                    await HandleUnkownCuisine(context, mealByDistance, cuisineDict, OnEnterOtherCuisine);

                }
                else
                {
                    mealByDistance.CuisineId = cuisineDict[mealByDistance.Cuisine];
                }
            }




            mealByDistance.AddressesArray = _externalService.GetAddressesArray(mealByDistance.Location);


            if (mealByDistance.AddressesArray != null || mealByDistance.AddressesArray.Length > 0)
            {
                if (mealByDistance.AddressesArray.Length == 1)
                {
                    PromptForAddressConfirmation(context, mealByDistance, MealByDistanceAddressConfirmation);
                }
                else
                {
                    HandleMultiAddress(context, mealByDistance, OnMultiAddressTask);

                }
            }
            else
            {
                HandleNoMatchedAddress(context, mealByDistance, OnSecondAddressTask);
            }

        }

        private void PromptForAddressConfirmation(IDialogContext context, BaseRequest request, ResumeAfter<bool> onAddressConfirmation)
        {
            string confirmMessage = $"I found '{request.AddressesArray[0]}'. Is this location correct?";

            //context.UserData.SetValue("searchMealRequest", searchMealRequest);
            context.UserData.Replace(request.Key, request);

            PromptDialog.Confirm(context, onAddressConfirmation, confirmMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);
        }

        private async Task HandleUnkownCuisine(IDialogContext context, BaseRequest model, Dictionary<string, int> cuisineDict, ResumeAfter<string> onNewCuisine)
        {
            await context.PostAsync($" {model.Cuisine} is not available. Please check the list of availability and try again.");

            string anotherCuisine = $"Please choose a one of the cuisines above.";

            //context.UserData.SetValue("mealByDistance", mealByDistance);
            context.UserData.Replace(model.Key, model);
            PromptDialog.PromptChoice<string> enterCuisine = new PromptDialog.PromptChoice<string>(cuisineDict.Keys, anotherCuisine, "Retry", 3);
            //search cuisines
            context.Call(enterCuisine, onNewCuisine);
        }


        private void HandleMultiAddress(IDialogContext context, BaseRequest request, ResumeAfter<string> onMultiAddressTask)
        {
            string multiAddressMessage = $"I narrowed down {request.AddressesArray.Length} possible locations. Please choose one below.";
            List<string> list = new List<string>(request.AddressesArray)
                    {
                        "None"
                    };

            context.UserData.Replace(request.Key, request);

            PromptDialog.Choice(context, onMultiAddressTask, list, multiAddressMessage);
        }

        private void HandleNoMatchedAddress(IDialogContext context, BaseRequest request, ResumeAfter<string> onNewAddress)
        {
            string anotherAddressMessage = $"Couldn't find an address that matches the location you provided. Please provide me a more detailed location with a zip code so I can pin point a location.";

            //context.UserData.SetValue("searchMealRequest", searchMealRequest);
            context.UserData.Replace(request.Key, request);

            PromptDialog.PromptString askForSecondAddressPrompt = new PromptDialog.PromptString(anotherAddressMessage, "Retry", 3);

            context.Call(askForSecondAddressPrompt, onNewAddress);
        }

        private async Task<SearchMealRequest> GetCuisineByPriceRequest(IDialogContext context, LuisResult result)
        {
            SearchMealRequest cuisineByPrice = new SearchMealRequest();



            EntityRecommendation cuisineEntityRecommendation = null;

            EntityRecommendation locationEntityRecommendation = null;

            EntityRecommendation priceEntityRecommendation = null;



            if (result.TryFindEntity(EntityLocation, out locationEntityRecommendation))
            {

                cuisineByPrice.Location = locationEntityRecommendation.Entity;

            }
            else
            {
                string locationError = $"Ooops! Something went your. Can you please try again. I didn't catch a location. Type 'help' assistance and search tips.";
                await context.PostAsync(locationError);
                context.Wait(MessageReceived);

            }

            if (result.TryFindEntity(EntityCuisine, out cuisineEntityRecommendation))
            {
                cuisineByPrice.Cuisine = cuisineEntityRecommendation.Entity;
            }
            else
            {
                string cuisineError = $"Ooops! Something went wrong. Can you please try again. I didn't catch a cuisine. Type 'help' assistance and search tips.";
                await context.PostAsync(cuisineError);
                context.Wait(MessageReceived);

            }



            if (result.TryFindEntity(EntityCurrency, out priceEntityRecommendation))
            {

                decimal price = 0;
                //Double.TryParse(priceEntityRecommendation.Entity, out price);
                Decimal.TryParse(priceEntityRecommendation.Resolution["value"].ToString(), out price);
                cuisineByPrice.Price = price;


            }

            return cuisineByPrice;
        }
        private async Task<SearchMealRequest> GetFoodByPriceRequest(IDialogContext context, LuisResult result)
        {
            SearchMealRequest searchMealRequest = new SearchMealRequest();

            EntityRecommendation foodEntityRecommendation = null;

            EntityRecommendation locationEntityRecommendation = null;

            EntityRecommendation priceEntityRecommendation = null;

            if (result.TryFindEntity(EntityLocation, out locationEntityRecommendation))
            {
                searchMealRequest.Location = locationEntityRecommendation.Entity;
            }
            else
            {
                string locationError = $"Ooops! Something went wrong. I didn't catch a location. Type 'help' assistance and search tips.";
                await context.PostAsync(locationError);

                context.Wait(MessageReceived);
            }

            if (result.TryFindEntity(EntityFood, out foodEntityRecommendation))
            {
                searchMealRequest.MealName = foodEntityRecommendation.Entity;
            }
            else
            {
                string cuisineError = $"Ooops! Something went wrong. Can you please try again. I didn't catch a cuisine. Type 'help' assistance and search tips.";
                await context.PostAsync(cuisineError);
                context.Wait(MessageReceived);

            }

            if (result.TryFindEntity(EntityCurrency, out priceEntityRecommendation))
            {
                decimal price = 0;
                //Double.TryParse(priceEntityRecommendation.Entity, out price);
                Decimal.TryParse(priceEntityRecommendation.Resolution["value"].ToString(), out price);
                searchMealRequest.Price = price;
            }

            return searchMealRequest;
        }
        private async Task<SearchMealRequest> GetSearchFoodRequest(IDialogContext context, LuisResult result)
        {
            SearchMealRequest searchMealRequest = new SearchMealRequest();

            EntityRecommendation foodEntityRecommendation = null;

            EntityRecommendation cityEntityRecommendation = null;


            if (result.TryFindEntity(EntityFood, out foodEntityRecommendation) && result.TryFindEntity(EntityLocation, out cityEntityRecommendation))
            {

                searchMealRequest.MealName = foodEntityRecommendation.Entity;

                searchMealRequest.Location = cityEntityRecommendation.Entity;

                string message = $"Hey! Give me second while I search for {searchMealRequest.MealName}. But first, let's confirm the location you provided me.";

                await context.PostAsync(message);
            }
            else
            {
                var errorMessage = $"Ooops! Something went your. Can you please try again. Type 'help' assistance and search tips.";

                await context.PostAsync(errorMessage);

                context.Wait(MessageReceived);

            }

            //GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);
            return searchMealRequest;
        }
        private static SearchMealRequest GetSearchCuisineRequest(LuisResult result)
        {

            SearchMealRequest searchMealRequest = new SearchMealRequest();
            EntityRecommendation cuisineEntityRecommendation = null;

            EntityRecommendation cityEntityRecommendation = null;

            if (result.TryFindEntity(EntityLocation, out cityEntityRecommendation))
            {
                searchMealRequest.Location = cityEntityRecommendation.Entity;
            }

            if (result.TryFindEntity(EntityCuisine, out cuisineEntityRecommendation))
            {
                searchMealRequest.Cuisine = cuisineEntityRecommendation.Entity;


            }

            return searchMealRequest;
        }
        private static async Task<SearchMealsByDistance> GetMealByDistanceRequest(IDialogContext context, LuisResult result)
        {
            SearchMealsByDistance mealByDistance = new SearchMealsByDistance();



            EntityRecommendation foodEntityRecommendation = null;

            EntityRecommendation locationEntityRecommendation = null;

            EntityRecommendation priceEntityRecommendation = null;



            EntityRecommendation distanceEntityRecommendation = null;



            if (result.TryFindEntity(EntityDistance, out distanceEntityRecommendation))
            {
                int miles = 0;
                //Double.TryParse(priceEntityRecommendation.Entity, out price);
                Int32.TryParse(distanceEntityRecommendation.Entity.ToString(), out miles);
                mealByDistance.Distance = miles;
            }

            if (result.TryFindEntity(EntityCurrency, out priceEntityRecommendation))
            {
                decimal price = 0;
                //Double.TryParse(priceEntityRecommendation.Entity, out price);
                Decimal.TryParse(priceEntityRecommendation.Resolution["value"].ToString(), out price);
                mealByDistance.Price = price;
            }

            string welcomeMessage = null;

            if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
            {
                welcomeMessage = $"Give me a second while I look for next meal within {mealByDistance.Distance} miles.";
            }
            else
            {
                welcomeMessage = $"Give me a second while I look for next meal within {mealByDistance.Distance} under  ${mealByDistance.Price}.";
            }

            if (result.TryFindEntity(EntityLocation, out locationEntityRecommendation))
            {
                mealByDistance.Location = locationEntityRecommendation.Entity;
            }


            if (result.TryFindEntity(EntityFood, out foodEntityRecommendation))
            {

                mealByDistance.MealName = foodEntityRecommendation.Entity;
            }

            await context.PostAsync(welcomeMessage);
            return mealByDistance;
        }
        private async Task<IEnumerable<MealSearchResultV2>> GetMealSearchResults(SearchMealsByDistance mealByDistance)
        {
            double lat = 0;
            double lng = 0;
            _externalService.GeocodeLocation(mealByDistance, out lat, out lng);

            IEnumerable<MealSearchResultV2> searchedMeals = null;

            if (mealByDistance.MealName != null)
            {
                mealByDistance.Meals = _externalService.SearchMealsList(lat, lng, mealByDistance.Distance);

                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
                {
                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName)).OrderBy(x => x.Distance);
                }
                else
                {
                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName) && meal.Price != null && meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
                }

            }
            else if (mealByDistance.Cuisine != null)
            {

                mealByDistance.Meals = _externalService.SearchMealsByCuisineList(lat, lng, mealByDistance.Distance, mealByDistance.CuisineId);

                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
                {
                    searchedMeals = mealByDistance.Meals.OrderBy(x => x.Distance);
                }
                else
                {
                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
                }
            }

            return searchedMeals;
        }
        private async Task<IEnumerable<MealSearchResultV2>> GetFiltedMealsByDistanceResults(SearchMealsByDistance mealByDistance, double lat, double lng)
        {
            IEnumerable<MealSearchResultV2> searchedMeals = null;

            if (mealByDistance.MealName != null)
            {
                mealByDistance.Meals = _externalService.SearchMealsList(lat, lng, mealByDistance.Distance);

                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
                {
                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName)).OrderBy(x => x.Distance);
                }
                else
                {
                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Title.Contains(mealByDistance.MealName) && meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
                }

            }
            else if (mealByDistance.Cuisine != null)
            {

                mealByDistance.Meals = _externalService.SearchMealsByCuisineList(lat, lng, mealByDistance.Distance, mealByDistance.CuisineId);

                if (mealByDistance.Price == null || !mealByDistance.Price.HasValue)
                {
                    searchedMeals = mealByDistance.Meals.OrderBy(x => x.Distance);
                }
                else
                {
                    searchedMeals = mealByDistance.Meals.Where(meal => meal.Price != null && meal.Price <= mealByDistance.Price).OrderBy(x => x.Distance);
                }
            }

            return searchedMeals;
        }
        private async Task<IEnumerable<MealSearchResultV2>> GetMealSearchResults(SearchMealRequest searchMealRequest)
        {
            double lat = 0;
            double lng = 0;
            _externalService.GeocodeLocation(searchMealRequest, out lat, out lng);

            IEnumerable<MealSearchResultV2> searchedMeals = null;


            if (searchMealRequest.MealName != null)
            {
                searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

                searchedMeals = searchMealRequest.Meals.Where(meals => meals.Title.Contains(searchMealRequest.MealName));
            }
            else if (searchMealRequest.Cuisine != null)
            {

                searchMealRequest.Meals = _externalService.SearchMealsByCuisineList(lat, lng, 20, searchMealRequest.CuisineId);

                searchedMeals = searchMealRequest.Meals;
            }

            return searchedMeals;
        }


        private void ResponsedToAddresses(IDialogContext context, SearchMealRequest searchMealRequest)
        {

            if (searchMealRequest.AddressesArray.Length == 1)
            {

                string confirmMessage = $"I found '{searchMealRequest.AddressesArray[0]}'. Is the location you want me search in?";

                //context.UserData.SetValue("searchMealRequest", searchMealRequest);

                context.UserData.Replace("searchMealRequest", searchMealRequest);

                PromptDialog.Confirm(context, MealNameAddressConfirmation, confirmMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);

            }
            else
            {
                HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);



            }
        }

        private void PromptNewDetailedAddress(IDialogContext context, BaseRequest modelRequest, ResumeAfter<string> onNewEnteredAddress)
        {
            string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";

            /*context.UserData.SetValue("mealByDistance", modelRequest)*/
            ;
            context.UserData.Replace(modelRequest.Key, modelRequest);

            PromptDialog.PromptString prompt = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);

            context.Call(prompt, onNewEnteredAddress);
        }

        private static async Task<string> OtherCuisinePrompt(IDialogContext context, Dictionary<string, int> cuisineDict)
        {
            string cuisineNames = "";

            cuisineDict.Keys.ForEach(x => cuisineNames += x + ", ");

            cuisineNames.TrimEnd(',');

            await context.PostAsync($"The cuisines we carry are: {cuisineNames}");

            string anotherCuisine = $"Please choose a one of the cuisines above.";
            return anotherCuisine;
        }

        private async Task ResumeAfterLocationDialogAsync(IDialogContext context, IAwaitable<Place> result)
        {
            var place = await result;

            if (place != null)
            {
                var address = place.GetPostalAddress();
                var formatteAddress = string.Join(", ", new[]
                {
                        address.StreetAddress,
                        address.Locality,
                        address.Region,
                        address.PostalCode,
                        address.Country
                    }.Where(x => !string.IsNullOrEmpty(x)));

                await context.PostAsync("Thanks, I will ship it to " + formatteAddress);
            }

            context.Done<string>(null);
        }
        //search cuisines
        private async Task OnEnterOtherCuisine(IDialogContext context, IAwaitable<string> result)
        {
            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
            searchMealRequest.Cuisine = await result;
            await context.PostAsync($"Great you chose: {searchMealRequest.Cuisine}. Let me search that for you.");
            Dictionary<string, int> cuisineDict = _externalService.CuisineDictionary();
            searchMealRequest.CuisineId = cuisineDict[searchMealRequest.Cuisine];
            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);

            if (searchMealRequest.AddressesArray != null)
            {
                if (searchMealRequest.AddressesArray.Length == 1)
                {
                    PromptForAddressConfirmation(context, searchMealRequest, OnSingleAddressConfirmation);
                }
                else
                {
                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);

                }
            }



        }
        //search cuisine and search food
        private async Task OnSingleAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;

            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");

            if (confirm)
            {

                await context.PostAsync("Great thank you for confirming your location.");

                IEnumerable<MealSearchResultV2> searchedMeals = await GetMealSearchResults(searchMealRequest);

                if (searchedMeals != null && searchedMeals.Any())
                {

                    await context.PostAsync($"I found {searchedMeals.Count()} meals near you:");

                    IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

                    await context.PostAsync(resultMessage);
                }
                else
                {
                    await context.PostAsync($"Sorry, I couldn't find any meals in {searchMealRequest.Location}. You can increase searching distance by researching: 'find {searchMealRequest.Cuisine} within 30 miles'");
                }



                context.Wait(MessageReceived);
            }
            else
            {
                PromptNewDetailedAddress(context, searchMealRequest, OnSecondAddressTask);
            }


        }

        private async Task OnMultiAddressTask(IDialogContext context, IAwaitable<string> result)
        {

            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
            searchMealRequest.Location = await result;

            if (searchMealRequest.Location != "None")
            {

                await context.PostAsync("Great thank you for confirming your location.");

                double lat = 0;
                double lng = 0;
                _externalService.GeocodeLocation(searchMealRequest, out lat, out lng);

                if (searchMealRequest.MealName != null)
                {
                    searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);
                }
                else if (searchMealRequest.Cuisine != null)
                {
                    searchMealRequest.Meals = _externalService.SearchMealsByCuisineList(lat, lng, 20, searchMealRequest.CuisineId);
                }


                await context.PostAsync($"I found {searchMealRequest.Meals.Count()} meals near you:");

                IMessageActivity resultMessage = GenerateCarousel(context, searchMealRequest.Meals);

                await context.PostAsync(resultMessage);

                context.Wait(MessageReceived);
            }
            else
            {
                PromptNewDetailedAddress(context, searchMealRequest, OnSecondAddressTask);
            }


        }


        private async Task OnSecondAddressTask(IDialogContext context, IAwaitable<string> result)
        {

            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");

            searchMealRequest.Location = await result;

            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(searchMealRequest.Location);

            if (searchMealRequest.AddressesArray != null)
            {
                if (searchMealRequest.AddressesArray.Length == 1)
                {
                    searchMealRequest.Location = searchMealRequest.AddressesArray[0];
                    PromptForAddressConfirmation(context, searchMealRequest, OnSingleAddressConfirmation);

                }
                else
                {
                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);
                }
            }


        }
        private async Task OnSecondAddressByDistanceTask(IDialogContext context, IAwaitable<string> result)
        {
            string addressSelected = await result;

            SearchMealsByDistance mealByDistance = context.UserData.GetValue<SearchMealsByDistance>("mealByDistance");

            //GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

            mealByDistance.AddressesArray = _externalService.GetAddressesArray(addressSelected);

            if (mealByDistance.AddressesArray != null)
            {
                if (mealByDistance.AddressesArray.Length == 1)
                {
                    string singleAddressMessage = $"I found '{mealByDistance.AddressesArray[0]}'. Is this the location you want me to search in?";

                    //context.UserData.SetValue("mealByDistance", mealByDistance);
                    context.UserData.Replace("mealByDistance", mealByDistance);

                    PromptDialog.Confirm(context, MealByDistanceAddressConfirmation, singleAddressMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);
                }
                else
                {
                    HandleMultiAddress(context, mealByDistance, OnMultiAddressTask);
                }
            }


        }
        private async Task OnSecondAddressFoodByPrice(IDialogContext context, IAwaitable<string> result)
        {

            string addressSelected = await result;
            var searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
            //GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

            searchMealRequest.AddressesArray = _externalService.GetAddressesArray(addressSelected);

            if (searchMealRequest.AddressesArray != null)
            {
                if (searchMealRequest.AddressesArray.Length == 1)
                {
                    string singleAddressMessage = $"I found '{searchMealRequest.AddressesArray[0]}'. Is this the location you want me to search in?";
                    PromptDialog.Confirm(context, SingleAddressConfirmationFoodByPrice, singleAddressMessage, "Sorry, I didn't get that!", 3, PromptStyle.Keyboard);

                }
                else
                {
                    HandleMultiAddress(context, searchMealRequest, OnMultiAddressTask);

                }
            }


        }

        //private async Task ReturnMealSearchResults(IDialogContext context, SearchMealRequest searchMealRequest, double lat, double lng)
        //{

        //    if (searchMealRequest.Meals != null && searchMealRequest.Meals.Count() > 0)
        //    {
        //        await context.PostAsync($"I found {searchMealRequest.Meals.Count()} meals near you under ${searchMealRequest.Price}:");

        //        IMessageActivity resultMessage = GenerateCarousel(context, searchMealRequest);

        //        await context.PostAsync(resultMessage);

        //        context.Wait(MessageReceived);
        //    }
        //    else
        //    {
        //        var noMealsMessage = $"I couldn't find {searchMealRequest.MealName} near {searchMealRequest.Location}. Try searching for another food.";

        //        await context.PostAsync(noMealsMessage);

        //        context.Wait(MessageReceived);
        //    }
        //}
        private async Task ReturnMealSearchResults(IDialogContext context, IEnumerable<MealSearchResultV2> searchedMeals)
        {
            if (searchedMeals != null && searchedMeals.Count() > 0)
            {
                await context.PostAsync($"I found {searchedMeals.Count()} meals near you:");

                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

                await context.PostAsync(resultMessage);

                context.Wait(MessageReceived);
            }
            else
            {
                string noMealsMessage = $"Ooop! It looks like there's no meals that match your search. Try searching for another food or cuisine, or expand your radius.";

                await context.PostAsync(noMealsMessage);

                context.Wait(MessageReceived);
            }
        }

        private async Task SingleAddressConfirmationFoodByPrice(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;
            string userAddress = null;

            if (confirm)
            {


                await context.PostAsync("Great thank you for confirming your location.");

                //GoogleLocationService locationService = new GoogleLocationService("AIzaSyCgv9wO_gY768dq28ZRf_YRykSsiUF2j2Q");
                GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

                MapPoint point = locationService.GetLatLongFromAddress(userAddress);

                double lat = point.Latitude;
                double lng = point.Longitude;


                IEnumerable<MealSearchResultV2> searchedMeals = null;

                await context.PostAsync($"I found {searchedMeals.Count()} meals near you:");

                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

                await context.PostAsync(resultMessage);

                context.Wait(MessageReceived);
            }
            else
            {
                string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";
                PromptDialog.PromptString fdf = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);
                context.Call(fdf, OnSecondAddressTask);
            }


        }
        private async Task FoodByPriceAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;
            var searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
            if (confirm)
            {
                searchMealRequest.Location = searchMealRequest.AddressesArray[0];

                await context.PostAsync("Great thank you for confirming your address.");

                double lat = 0;
                double lng = 0;
                _externalService.GeocodeLocation(searchMealRequest, out lat, out lng);

                searchMealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

                //capture mathsign lessthan or greaer than 1:02am
                IEnumerable<MealSearchResultV2> searchedMeals = searchMealRequest.Meals.Where(meal => meal.Title.Contains(searchMealRequest.MealName) && meal.Price != null && meal.Price <= searchMealRequest.Price).OrderBy(x => x.Price);

                await ReturnMealSearchResults(context, searchedMeals);

            }
            else
            {

                PromptNewDetailedAddress(context, searchMealRequest, OnSecondAddressTask);
            }

        }
        private async Task MealByDistanceAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;

            SearchMealsByDistance mealByDistance = context.UserData.GetValue<SearchMealsByDistance>("searchMealRequest");

            if (confirm)
            {
                mealByDistance.Location = mealByDistance.AddressesArray[0];

                await context.PostAsync("Great thank you for confirming your address.");
                IEnumerable<MealSearchResultV2> searchedMeals = await GetMealSearchResults(mealByDistance);

                //IEnumerable<MealSearchResultV2> searchedMeals = await GetFiltedMealsByDistance(mealByDistance, lat, lng);


                //capture mathsign lessthan or greaer than 1:02am

                await ReturnMealSearchResults(context, searchedMeals);

            }
            else
            {

                PromptNewDetailedAddress(context, mealByDistance, OnSecondAddressByDistanceTask);
            }

        }
        private async Task CuisineByPriceAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;
            SearchMealRequest searchMealRequest = context.UserData.GetValue<SearchMealRequest>("searchMealRequest");
            if (confirm)
            {
                searchMealRequest.Location = searchMealRequest.AddressesArray[0];

                await context.PostAsync("Great thank you for confirming your address.");

                GoogleLocationService locationService = new GoogleLocationService(GoogleApiKey);

                MapPoint point = locationService.GetLatLongFromAddress(searchMealRequest.Location);

                double lat = point.Latitude;
                double lng = point.Longitude;

                searchMealRequest.Meals = _externalService.SearchMealsByCuisineList(lat, lng, 100, searchMealRequest.CuisineId);

                //capture mathsign lessthan or greaer than 1:02am
                IEnumerable<MealSearchResultV2> searchedMeals = searchMealRequest.Meals.Where(meal => meal.Price <= searchMealRequest.Price);

                if (searchedMeals != null && searchedMeals.Count() > 0)
                {
                    await context.PostAsync($"I found {searchedMeals.Count()} {searchMealRequest.Cuisine} meals near you:");
                }
                else
                {
                    var noMealsMessage = $"I couldn't find {searchMealRequest.Cuisine} food near {searchMealRequest.Location}. Try searching for another cuisine.";
                    await context.PostAsync(noMealsMessage);
                    //option to research cuisine since 0 came back

                }


                IMessageActivity resultMessage = GenerateCarousel(context, searchedMeals);

                await context.PostAsync(resultMessage);

                context.Wait(MessageReceived);
            }
            else
            {
                string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";

                //context.UserData.SetValue("cuisineByPrice", cuisineByPrice);
                context.UserData.Replace("cuisineByPrice", searchMealRequest);

                PromptDialog.PromptString zipCodePrompt = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);
                //search food


                context.Call(zipCodePrompt, OnSecondAddressTask);
            }


        }
        private async Task MealNameAddressConfirmation(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirm = await result;
            SearchMealNameRequest mealRequest = context.UserData.GetValue<SearchMealNameRequest>("searchMealRequest");
            if (confirm)
            {
                await context.PostAsync("Great thank you for confirming your address.");

                double lat = 0;
                double lng = 0;
                _externalService.GeocodeLocation(mealRequest, out lat, out lng);

                mealRequest.Meals = _externalService.SearchMealsList(lat, lng, 20);

                IEnumerable<MealSearchResultV2> searchedMeals = mealRequest.Meals.Where(meals => meals.Title.Contains(mealRequest.MealName));

                await ReturnMealSearchResults(context, searchedMeals);

            }
            else
            {
                string zipCodeRequest = $"Okay, please provide me a more detailed location with a zip code so I can pin point your location.";
                var fdf = new PromptDialog.PromptString(zipCodeRequest, "Retry", 3);
                //search food
                context.Call(fdf, OnSecondAddressTask);
            }


        }


        private IMessageActivity GenerateCarousel(IDialogContext context, IEnumerable<MealSearchResultV2> searchedMeals)
        {
            IMessageActivity resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            foreach (MealSearchResultV2 meal in searchedMeals)
            {
                if (meal.User != null)
                {


                    CardImage cardImage = new CardImage() { Url = AwsBaseUrl + meal.Photo };


                    CardAction cardAction = new CardAction()
                    {
                        Title = "View Chef's Profile",
                        Type = ActionTypes.OpenUrl,
                        Value = SiteDomain + "cooks/" + meal.User.Id

                    };



                    string distanceInfo = null;
                    if (meal.Price != null)
                    {
                        distanceInfo = $"${meal.Price} - {Math.Round(meal.Distance, 2)} mi - {meal.Address.City}";
                    }
                    else
                    {
                        distanceInfo = $"{Math.Round(meal.Distance, 2)} mi - {meal.Address.City}";
                    }



                    Attachment heroCardAttachment = GetHeroCard(meal.Title, meal.Description, distanceInfo, cardImage, cardAction);

                    resultMessage.Attachments.Add(heroCardAttachment);
                }
            }

            return resultMessage;
        }
        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            HeroCard heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction> { cardAction }
            };

            return heroCard.ToAttachment();
        }

    }



    public static class IBotDataBagExt
    {
        public static void Replace(this IBotDataBag botBag, string key, object val)
        {
            if (botBag.ContainsKey(key))
            {
                botBag.RemoveValue(key);
            }
            botBag.SetValue(key, val);
        }
    }

    //move this else where.
    //like a c# gulp, all the partial class BasicQnAMakerDialog will me gulped together
    //will have access to protected methods like example above
    //this should be in a folder called 'BasicQnAMakerDialog' which will contain all my partial classes of BasicQnAMakerDialog

    public partial class BasicQnAMakerDialog
    {

        protected int MyProperty { get; set; }

    }

}



