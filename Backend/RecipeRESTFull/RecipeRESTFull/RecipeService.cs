using RecipeRESTFull.Interfaces;
using RecipeRESTFull.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRESTFull
{
    public class RecipeService : IRecipeService
    {
        IDataProvider dataProvider;

        public RecipeService(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            List<Recipe> recipes = await dataProvider.LoadRecipes().ConfigureAwait(false);
            return recipes;
        }

        public async Task InsertRecipe(List<Recipe> recipes)
        {
            List<int> recipeIds = recipes.Select(x => x.ID).ToList();

            List<int> uniqeRecipe = await CheckIsRecipesExist(recipeIds);

            if (uniqeRecipe.Count != 0)
            {
                List<Recipe> recipeForInsert = new List<Recipe>();

                foreach (var recipe in recipes)
                {
                    if (uniqeRecipe.Contains(recipe.ID))
                    {
                        recipeForInsert.Add(recipe);
                    }
                }

                await dataProvider.InsertRecipe(recipeForInsert);
            }
        }

        public async Task<Recipe> GetRecipeById(int recipeId)
        {
            Recipe recipe = await dataProvider.LoadRecipe(recipeId).ConfigureAwait(false);

            return recipe;
        }

        private async Task<List<int>> CheckIsRecipesExist(List<int> recipeIds)
        {
            List<int> recipesIdsDB = await dataProvider.GetRecipeIdsByIds(recipeIds);

            List<int> recipeIdsNoExist = recipeIds.Except(recipesIdsDB).ToList();
            return recipeIdsNoExist;
        }

        public async Task<bool> RemoveRecipe(int id)
        {
            return await dataProvider.DeleteRecipe(id).ConfigureAwait(false); 
        }
    }
}
