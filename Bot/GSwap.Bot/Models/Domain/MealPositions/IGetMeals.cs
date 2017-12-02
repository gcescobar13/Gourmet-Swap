using System.Collections.Generic;
using System.Threading.Tasks;
using GSwap.Models.Domain.Meals;

namespace QnABot.Models.MealPositions
{
    public interface IGetMeals
    {
        Task<List<MealSearchResultV2>> GetAllMealPositions(double lat, double lng, int radius);
        Task<List<MealSearchResultV2>> GetAllMealPositionsByCuisine(double lat, double lng, int radius, int cuisineId);
    }
}