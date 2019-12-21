using RecipeRESTFull.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRESTFull.Interfaces
{
    public interface IDataProvider
    {
        Task<List<Recipe>> LoadRecipes();
        Task<Recipe> LoadRecipe(int recipeID);
        Task InsertRecipe(List<Recipe> recipe);
        Task<List<int>> GetRecipeIdsByIds(List<int> recipeIds);
        Task<bool> DeleteRecipe(int id);
    }
}
