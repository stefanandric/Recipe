using RecipeRESTFull.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRESTFull.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetRecipes();
        Task InsertRecipe(List<Recipe> recipe);
        Task<Recipe> GetRecipeById(int recipeId);
        Task<bool> RemoveRecipe(int id);
    }
}
