using GSwap.Data;
using GSwap.Data.Providers;
using GSwap.Models;
using GSwap.Models.Domain;
using GSwap.Models.Domain.Addresses;
using GSwap.Models.Domain.Meals;
using GSwap.Models.Domain.Users;
using GSwap.Models.Requests.Meals;
using GSwap.Services.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Services
{
    public class MealService2 : IMealService2
    {
        private IDataProvider _dataProvider;
        private IUserService _getProfile;        
        private IUserAuthData _currentUser;
        public IPrincipal _principal = null;

        public MealService2(IDataProvider dataProvider, IUserService getProfile, IPrincipal user)
        {
            _dataProvider = dataProvider;
            _getProfile = getProfile;
            _principal = user;
            _currentUser = _principal.Identity.GetCurrentUser();
        }

        //-----------------------------------------------------Insert-----------------------------------------------------//

        //MEAL
        public int AddMeal(MealAddRequest model, int UserId)
        {
            int id = 0;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@ServiceId", model.ServiceId);
                paramCollection.AddWithValue("@CuisineId", model.CuisineId);
                paramCollection.AddWithValue("@Title", model.Title);
                paramCollection.AddWithValue("@Description", model.Description);
                paramCollection.AddWithValue("@DeliveryOption", model.DeliveryOption);
                paramCollection.AddWithValue("@Comment", model.Comment);
                paramCollection.AddWithValue("@DietaryLabel", model.DietaryLabel);
                paramCollection.AddWithValue("@UserId", UserId);

                SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                idParameter.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(idParameter);
            };

            Action<SqlParameterCollection> returnParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                Int32.TryParse(paramCollection["@Id"].Value.ToString(), out id);
            };

            _dataProvider.ExecuteNonQuery("dbo.Meals_Insert", inputParamDelegate, returnParamDelegate);

            return id;
        }

        //MEAL ITEMS
        public void AddMealItems(List<MealItemAddRequest> mealItems, int mealId, int userId)
        {
            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@MealId", mealId);
                paramCollection.AddWithValue("@UserId", userId);

                SqlParameter itemParameter = new SqlParameter("@Items", System.Data.SqlDbType.Structured);

                if (mealItems != null && mealItems.Any())
                {
                    MealItemDataRecords tbl = new MealItemDataRecords(mealItems);
                    itemParameter.Value = tbl;
                }

                paramCollection.Add(itemParameter);
            };

            _dataProvider.ExecuteNonQuery("dbo.MealItems_Insert", inputParamDelegate);
        }

        //MEAL TIMES
        public void AddMealTimes(List<MealTimeAddRequest> mealTimes, int mealId, int userId)
        {
            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@MealId", mealId);
                paramCollection.AddWithValue("@UserId", userId);

                SqlParameter timeParameter = new SqlParameter("@Times", System.Data.SqlDbType.Structured);

                if (mealTimes != null && mealTimes.Any())
                {
                    MealTimeDataRecords tbl = new MealTimeDataRecords(mealTimes);
                    timeParameter.Value = tbl;

                }

                paramCollection.Add(timeParameter);
            };

            _dataProvider.ExecuteNonQuery("dbo.MealTimes_Insert", inputParamDelegate);
        }

        //MEAL LOCATION
        public int AddMealLocation(MealLocationAddRequest model, int UserId)
        {
            int id = 0;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@CurbPickup", model.CurbPickup);
                paramCollection.AddWithValue("@AddressId", model.AddressId);
                paramCollection.AddWithValue("@MealId", model.MealId);
                paramCollection.AddWithValue("@UserId", UserId);

                SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                idParameter.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(idParameter);
            };

            Action<SqlParameterCollection> returnParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                Int32.TryParse(paramCollection["@Id"].Value.ToString(), out id);
            };

            _dataProvider.ExecuteNonQuery("dbo.MealLocation_Insert", inputParamDelegate, returnParamDelegate);

            return id;
        }

        //MEAL LOG
        public void AddMealsLog(Dictionary<int, MealLogAddRequest> meals)
        {
            List<MealLogAddRequest> mealLogList = null;
            MealLogAddRequest singleItem = null;

            foreach (KeyValuePair<int, MealLogAddRequest> meal in meals)
            {
                if (singleItem == null)
                {
                    singleItem = new MealLogAddRequest();
                }

                singleItem.MealId = meal.Value.MealId;
                singleItem.Price = meal.Value.Price;
                singleItem.MealQuant = meal.Value.MealQuant;
                singleItem.TrueTotalMealCost = meal.Value.TrueTotalMealCost;

                if (mealLogList == null)
                {
                    mealLogList = new List<MealLogAddRequest>();
                }

                mealLogList.Add(singleItem);
            }

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserId", _currentUser.Id);

                SqlParameter itemParameter = new SqlParameter("@Orders", System.Data.SqlDbType.Structured);

                if (mealLogList != null && mealLogList.Any())
                {
                    MealOrderLogDataRecords tbl = new MealOrderLogDataRecords(mealLogList);
                    itemParameter.Value = tbl;
                }

                paramCollection.Add(itemParameter);
            };

            _dataProvider.ExecuteNonQuery("dbo.MealOrderLog_Insert", inputParamDelegate);
        }

        //----------------------------------------------------Select-----------------------------------------------------//

        //MEAL BY ID
        public Meal GetMeal(int mealId)
        {
            Meal myMeal = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                Meal singleItem = new Meal();
                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.ServiceId = reader.GetSafeInt32(startingIndex++);
                singleItem.CuisineId = reader.GetSafeInt32(startingIndex++); 
                singleItem.Title = reader.GetSafeString(startingIndex++);
                singleItem.Description = reader.GetSafeString(startingIndex++);
                singleItem.DeliveryOption = reader.GetSafeInt32(startingIndex++);
                singleItem.Comment = reader.GetSafeString(startingIndex++);
                singleItem.DietaryLabel = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                myMeal = singleItem;
            };

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", mealId);
            };

            _dataProvider.ExecuteCmd("dbo.Meals_SelectById", inputParamDelegate, singleRecMapper);

            return myMeal;
        }

        //ALL MEALS
        public List<Meal> GetAllMeals()
        {
            List<Meal> mealsList = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                Meal singleItem = new Meal();

                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.ServiceId = reader.GetSafeInt32(startingIndex++);
                singleItem.Title = reader.GetSafeString(startingIndex++);
                singleItem.Description = reader.GetSafeString(startingIndex++);
                singleItem.DeliveryOption = reader.GetSafeInt32(startingIndex++);
                singleItem.Comment = reader.GetSafeString(startingIndex++);
                singleItem.DietaryLabel = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                if (mealsList == null)
                {
                    mealsList = new List<Meal>();
                }

                mealsList.Add(singleItem);
            };

            Action<SqlParameterCollection> inputParamDelegate = null;

            _dataProvider.ExecuteCmd("dbo.Meals_SelectAll", inputParamDelegate, singleRecMapper);

            return mealsList;
        }

        

        //MEAL LOCATION BY ID
        public MealLocation GetMealLocation(int mealId)
        {
            MealLocation myMealLocation = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealLocation singleItem = new MealLocation();
                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.CurbPickup = reader.GetSafeBool(startingIndex++);
                singleItem.AddressId = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                myMealLocation = singleItem;
            };

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@MealId", mealId);
            };

            _dataProvider.ExecuteCmd("dbo.MealLocation_SelectById", inputParamDelegate, singleRecMapper);

            return myMealLocation;
        }

        //ALL MEAL LOCATIONS
        public List<MealLocation> GetAllMealLocations()
        {
            List<MealLocation> mealLocationsList = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealLocation singleItem = new MealLocation();

                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.CurbPickup = reader.GetSafeBool(startingIndex++);
                singleItem.AddressId = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                if (mealLocationsList == null)
                {
                    mealLocationsList = new List<MealLocation>();
                }

                mealLocationsList.Add(singleItem);
            };

            Action<SqlParameterCollection> inputParamDelegate = null;

            _dataProvider.ExecuteCmd("dbo.MealLocation_SelectAll", inputParamDelegate, singleRecMapper);

            return mealLocationsList;
        }

       
        public MealPhoto GetMealPhoto(int id)
        {
            MealPhoto myMealPhoto = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealPhoto singleItem = new MealPhoto();
                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.PhotoId = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                myMealPhoto = singleItem;
            };

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            };

            _dataProvider.ExecuteCmd("dbo.MealPhotos_SelectById", inputParamDelegate, singleRecMapper);

            return myMealPhoto;
        }

        //MEAL PHOTOS BY MEALID
        public List<MealPhoto> GetMealPhotos(int mealId)
        {
            List<MealPhoto> mealPhotosList = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealPhoto singleItem = new MealPhoto();
                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.PhotoId = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                if (mealPhotosList == null)
                {
                    mealPhotosList = new List<MealPhoto>();
                }

                mealPhotosList.Add(singleItem);
            };

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@MealId", mealId);
            };

            _dataProvider.ExecuteCmd("dbo.MealPhotos_SelectByMealId", inputParamDelegate, singleRecMapper);

            return mealPhotosList;
        }

        //ALL MEAL PHOTOS
        public List<MealPhoto> GetAllMealPhotos()
        {
            List<MealPhoto> mealPhotosList = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealPhoto singleItem = new MealPhoto();
                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.PhotoId = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);

                if (mealPhotosList == null)
                {
                    mealPhotosList = new List<MealPhoto>();
                }

                mealPhotosList.Add(singleItem);
            };

            Action<SqlParameterCollection> inputParamDelegate = null;

            _dataProvider.ExecuteCmd("dbo.MealPhotos_SelectAll", inputParamDelegate, singleRecMapper);

            return mealPhotosList;
        }

        //MEAL POSITIONS WITHIN RADIUS VERSION 1
        public List<MealSearchResult> GetMealPositions(double lat, double lng, int maxDistance)
        {
            List<MealSearchResult> mealPositionList = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealSearchResult singleItem = new MealSearchResult();
                UserProfile user = new UserProfile();
                AddressLite address = new AddressLite();

                int startingIndex = 0; //startingOrdinal

                user.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.Title = reader.GetSafeString(startingIndex++);
                singleItem.Description = reader.GetSafeString(startingIndex++);
                singleItem.DeliveryOption = reader.GetSafeInt32(startingIndex++);
                address.LineOne = reader.GetSafeString(startingIndex++);
                address.City = reader.GetSafeString(startingIndex++);
                address.Zip = reader.GetSafeString(startingIndex++);
                address.Lat = reader.GetSafeDouble(startingIndex++);
                address.Lng = reader.GetSafeDouble(startingIndex++);
                singleItem.Distance = reader.GetSafeDouble(startingIndex++);
                singleItem.Photo = reader.GetSafeString(startingIndex++);
                singleItem.Price = reader.GetSafeDecimal(startingIndex++);

                singleItem.User = user;
                singleItem.Address = address;

                if (mealPositionList == null)
                {
                    mealPositionList = new List<MealSearchResult>();
                }

                mealPositionList.Add(singleItem);
            };

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Lat", lat);
                paramCollection.AddWithValue("@Lng", lng);
                paramCollection.AddWithValue("@MaxDistance", maxDistance);
            };

            _dataProvider.ExecuteCmd("dbo.Meals_SelectAllPositions", inputParamDelegate, singleRecMapper);

            if(mealPositionList != null)
            {
                foreach (var meal in mealPositionList)

                {
                    meal.User = _getProfile.GetCookProfile(meal.User.Id);
                }
            }
            
            return mealPositionList;
        }

        
        //ALL MEAL PHOTOS WITHIN RADIUS
        public List<MealPhoto> GetPhotos(List<int> mealIds)
        {
            List<MealPhoto> mealSearchResultsPhotoList = null;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                SqlParameter itemParameter = new SqlParameter("@MealIds", System.Data.SqlDbType.Structured);

                if (mealIds != null && mealIds.Any())
                {
                    MealIdTable mealIdsTbl = new MealIdTable(mealIds);
                    itemParameter.Value = mealIdsTbl;
                }

                paramCollection.Add(itemParameter);
            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealPhoto singleItem = new MealPhoto();
                int startingIndex = 0; //startingOrdinal

                singleItem.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.PhotoId = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                singleItem.DateAdded = reader.GetSafeDateTime(startingIndex++);
                singleItem.DateModified = reader.GetSafeDateTime(startingIndex++);
                singleItem.FileName = reader.GetSafeString(startingIndex++);
                singleItem.FileTypeId = reader.GetSafeInt32(startingIndex++);

                if (mealSearchResultsPhotoList == null)
                {
                    mealSearchResultsPhotoList = new List<MealPhoto>();
                }

                mealSearchResultsPhotoList.Add(singleItem);
            };

            _dataProvider.ExecuteCmd("dbo.MealPhotos_SelectByMealIds", inputParamDelegate, singleRecMapper);

            return mealSearchResultsPhotoList;
        }

        
        public void UpdateLocation(MealLocationUpdateRequest model, int UserId)
        {

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@CurbPickup", model.CurbPickup);
                paramCollection.AddWithValue("@AddressId", model.AddressId);
                paramCollection.AddWithValue("@MealId", model.MealId);
                paramCollection.AddWithValue("@Id", model.Id);
                paramCollection.AddWithValue("@UserId", UserId);
            };

            _dataProvider.ExecuteNonQuery("dbo.MealLocation_Update", inputParamDelegate);
        }

   

        public List<MealSearchResultV2> GetMealPositionsByCuisine(double lat, double lng, int maxDistance, int cuisineId)
        {
            List<MealSearchResultV2> mealPositionList = null;

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                MealSearchResultV2 singleItem = new MealSearchResultV2();
                UserProfile user = new UserProfile();
                AddressLite address = new AddressLite();

                int startingIndex = 0; //startingOrdinal

                user.Id = reader.GetSafeInt32(startingIndex++);
                singleItem.MealId = reader.GetSafeInt32(startingIndex++);
                singleItem.Title = reader.GetSafeString(startingIndex++);
                singleItem.Description = reader.GetSafeString(startingIndex++);
                singleItem.DeliveryOption = reader.GetSafeInt32(startingIndex++);
                address.LineOne = reader.GetSafeString(startingIndex++);
                address.City = reader.GetSafeString(startingIndex++);
                address.Zip = reader.GetSafeString(startingIndex++);
                address.Lat = reader.GetSafeDouble(startingIndex++);
                address.Lng = reader.GetSafeDouble(startingIndex++);
                singleItem.Distance = reader.GetSafeDouble(startingIndex++);
                singleItem.Photo = reader.GetSafeString(startingIndex++);
                singleItem.Price = reader.GetSafeDecimalNullable(startingIndex++);
                singleItem.CuisineId = reader.GetSafeInt32(startingIndex++);

                singleItem.User = user;
                singleItem.Address = address;

                if (mealPositionList == null)
                {
                    mealPositionList = new List<MealSearchResultV2>();
                }

                mealPositionList.Add(singleItem);
            };

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Lat", lat);
                paramCollection.AddWithValue("@Lng", lng);
                paramCollection.AddWithValue("@MaxDistance", maxDistance);
                paramCollection.AddWithValue("@CuisineId", cuisineId);
            };

            _dataProvider.ExecuteCmd("dbo.Meals_SelectAllPositionsByCuisine", inputParamDelegate, singleRecMapper);

            if (mealPositionList != null)
            {
                foreach (var meal in mealPositionList)

                {
                    meal.User = _getProfile.GetCookProfile(meal.User.Id);
                }
            }

            return mealPositionList;
        }

    }
};

