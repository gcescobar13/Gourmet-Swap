using GSwap.Data;
using GSwap.Data.Providers;
using GSwap.Models;
using GSwap.Models.Domain;
using GSwap.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Services
{
    public class DashboardService : IDashboardService
    {
        private IDataProvider _dataProvider;

        public DashboardService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        private Dictionary<int, string> rolesConstant = new Dictionary<int, string>()
        {
            {1, "User"},
            {2, "Chef"},
            {3, "Admin"},
            {4, "Driver"}
        };



        private static User UserMapper(IDataReader reader)
        {
            User user = new User();
            int startingIndex = 0;

            user.Id = reader.GetSafeInt32(startingIndex++);
            user.FirstName = reader.GetSafeString(startingIndex++);
            user.LastName = reader.GetSafeString(startingIndex++);
            user.Email = reader.GetSafeString(startingIndex++);
            user.Number = reader.GetSafeString(startingIndex++);
            user.ZipCode = reader.GetSafeString(startingIndex++);
            user.DateAdded = reader.GetSafeDateTime(startingIndex++);
            user.DateModified = reader.GetSafeDateTime(startingIndex++);
            return user;
        }

        public PagedList<User> GetPaginatedUsers(PagedUsersRequest request)
        {
            int totalCount = 0;
            PagedList<User> pagedContent = null;
            List<User> pagedUsers = null;
            Dictionary<int, List<string>> userRolesDict = null;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                
                paramCollection.AddWithValue("@PageIndex", request.PageIndex);
                paramCollection.AddWithValue("@PageSize", request.PageSize);
                paramCollection.AddWithValue("@SortTypeId", request.SortTypeId);
                paramCollection.AddWithValue("@SearchTerm", request.SearchTerm);
                paramCollection.AddWithValue("@RoleId", request.RoleId);
            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                
                if(set == 0)
                {

                    int startingIndex = 0;
                    int userId = reader.GetSafeInt32(startingIndex++);
                    int roleId = reader.GetSafeInt32(startingIndex++);

                    if (userRolesDict == null)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                        userRolesDict = new Dictionary<int, List<string>>();
                    }

                    if (!userRolesDict.ContainsKey(userId))
                    {
                        userRolesDict.Add(userId, new List<string>());
                    }

                    userRolesDict[userId].Add(rolesConstant[roleId]);

                }

                if (set == 1)
                {

                    User user = UserMapper(reader);

                    if (pagedUsers == null)
                    {
                        pagedUsers = new List<User>();
                    }

                    pagedUsers.Add(user);

                }
            };

            _dataProvider.ExecuteCmd("dbo.Users_Pagination", inputParamDelegate, singleRecMapper);

            if(pagedUsers != null && userRolesDict != null)
            {
                foreach(User currentUser in pagedUsers)
                {
                    if (userRolesDict.ContainsKey(currentUser.Id)){
                        currentUser.Roles = userRolesDict[currentUser.Id];
                    }
                }
            }

            if(pagedContent == null)
            {
                pagedContent = new PagedList<User>(pagedUsers, request.PageIndex, request.PageSize, totalCount);
            }

            return pagedContent;

        }
        public void ToggleChef(int userId, string role)
        {
            

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@UserId", userId);
                paramCollection.AddWithValue("@Role", role);

            };

            string proc = "dbo.UserRole_ToggleChef";
            _dataProvider.ExecuteNonQuery(proc, inputParamDelegate);


           

        }
    }
}
