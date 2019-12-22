using Microsoft.Extensions.Options;
using RecipeRESTFull.Interfaces;
using RecipeRESTFull.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRESTFull.DAL
{
    public class DataProvider : IDataProvider
    {
        private AppSettings _settings;

        public DataProvider(IOptions<AppSettings> options)
        {
            _settings = options.Value;
        }

        #region Queries

        string LOAD_RECIPES = @"
                                select 
                                    r.ID as ID,
                                    r.Name as RecipeName,
                                    r.Description as Description,
                                    r.ImagePath as ImgPath,
	                                r.DateCreated as DateCreated
                                from Recipe r
                                ";

        private const string LOAD_RECIPE = @"select r.ID as ID, r.Name as RecipeName, r.Description as Description, r.ImagePath as ImgPath, r.DateCreated as DateCreated
                                from Recipe r where r.ID = @ID";

        private const string LOAD_INGREDIANTS = @"
                                            select 
	                                            ID,
	                                            RecipeID,
	                                            Name,
	                                            Count,
	                                            DateCreated
                                            from Ingrediant
                                            ";

        private const string LOAD_INGREDIANT = @"
                                            select 
	                                            ID,
	                                            RecipeID,
	                                            Name,
	                                            Count,
	                                            DateCreated
                                            from 
                                                Ingrediant
											where
												RecipeID = @RecipeID
                                                ";

        private const string INSERT_INTO_RECIPE_AND_GET_SCOPE_IDENTITY = @"insert into Recipe(Name, Description, ImagePath, DateCreated)
                                                            values(@recipeName, @descriptions, @imgPath, @DateCreated);
                                                            select CAST(SCOPE_IDENTITY() as int);";

        private const string INSERT_INTO_INGREDIANTS = @"insert into Ingrediant(RecipeID, Name, Count, DateCreated)
                                            values(@recipeId, @ingrediantName, @count, @DateCreated);";

        private const string GET_RECIPEIDS_BY_IDS = @"
                                            select ID from Recipe
                                            ";

        private const string DELETE_INGREDIANTS = @"
                                            delete from Ingrediant
                                            where RecipeID = @id
                                            ";

        private const string DELETE_RECIPE = @"
                                            delete from Recipe
                                            where ID = @id
                                            ";

        #endregion Queries

        public async Task<Recipe> LoadRecipe(int recipeID)
        {
            Recipe recipe = new Recipe();

            using (SqlConnection connection = new SqlConnection(_settings.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (SqlCommand cmd = new SqlCommand(LOAD_RECIPE, connection))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = recipeID;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recipe.ID = Convert.ToInt32(reader["ID"]);
                            recipe.Name = Convert.ToString(reader["RecipeName"]);
                            recipe.Description = Convert.ToString(reader["Description"]);
                            recipe.ImgPath = Convert.ToString(reader["ImgPath"]);
                            recipe.Ingrediants = new List<Ingrediant>();
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(LOAD_INGREDIANT, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        cmd.Parameters.Add("@RecipeID", SqlDbType.Int).Value = recipeID;

                        while (reader.Read())
                        {
                            Ingrediant ingrediant = new Ingrediant();
                            ingrediant.ID = Convert.ToInt32(reader["ID"]);
                            ingrediant.RecipeID = Convert.ToInt32(reader["RecipeID"]);
                            ingrediant.Name = Convert.ToString(reader["Name"]);
                            ingrediant.Count = Convert.ToInt32(reader["Count"]);
                            recipe.Ingrediants.Add(ingrediant);
                        }
                    }
                }
                connection.Close();
            }
            return recipe;
        }
    
        public async Task<List<Recipe>> LoadRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_settings.ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (SqlCommand cmd = new SqlCommand(LOAD_RECIPES, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                Recipe recipe = new Recipe();
                                recipe.ID = Convert.ToInt32(reader["ID"]);
                                recipe.Name = Convert.ToString(reader["RecipeName"]);
                                recipe.Description = Convert.ToString(reader["Description"]);
                                recipe.ImgPath = Convert.ToString(reader["ImgPath"]);
                                recipe.DateCreated = Convert.ToDateTime(reader["DateCreated"]);
                                recipe.Ingrediants = new List<Ingrediant>();
                                recipes.Add(recipe);
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(LOAD_INGREDIANTS, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                Ingrediant ingrediant = new Ingrediant();
                                ingrediant.ID = Convert.ToInt32(reader["ID"]);
                                ingrediant.RecipeID = Convert.ToInt32(reader["RecipeID"]);
                                ingrediant.Name = Convert.ToString(reader["Name"]);
                                ingrediant.Count = Convert.ToInt32(reader["Count"]);

                                Recipe recipe = recipes.Where(x => x.ID == ingrediant.RecipeID).First();
                                recipe.Ingrediants.Add(ingrediant);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
            return recipes;
        }

        public async Task InsertRecipe(List<Recipe> recipes)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_settings.ConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    foreach (var recipe in recipes)
                    {
                        int recipeId = int.MinValue;

                        using (SqlCommand cmd = new SqlCommand(INSERT_INTO_RECIPE_AND_GET_SCOPE_IDENTITY, connection))
                        {
                            cmd.Parameters.Add("@recipeName", SqlDbType.VarChar).Value = recipe.Name;
                            cmd.Parameters.Add("@descriptions", SqlDbType.VarChar).Value = recipe.Description;
                            cmd.Parameters.Add("@imgPath", SqlDbType.VarChar).Value = recipe.ImgPath;
                            cmd.Parameters.Add("@DateCreated", SqlDbType.DateTime2).Value = DateTime.UtcNow;

                            recipeId = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                            recipe.ID = recipeId;
                        }

                        foreach (var ingrediant in recipe.Ingrediants)
                        {
                            using (SqlCommand command = new SqlCommand(INSERT_INTO_INGREDIANTS, connection))
                            {
                                command.Parameters.Add("@recipeId", SqlDbType.Int).Value = recipeId;
                                command.Parameters.Add("@ingrediantName", SqlDbType.VarChar).Value = ingrediant.Name;
                                command.Parameters.Add("@count", SqlDbType.Int).Value = ingrediant.Count;
                                command.Parameters.Add("@DateCreated", SqlDbType.DateTime2).Value = DateTime.UtcNow;

                                ingrediant.ID = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<int>> GetRecipeIdsByIds(List<int> recipeIds)
        {
            List<int> recipeIDsDB = new List<int>();

            using (SqlConnection connection = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(GET_RECIPEIDS_BY_IDS, connection))
                {
                    await cmd.Connection.OpenAsync().ConfigureAwait(false);

                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            int recipeId = Convert.ToInt32(reader["ID"]);
                            recipeIDsDB.Add(recipeId);
                        }
                    }
                }
            }
            return recipeIDsDB;
        }

        public async Task<bool> DeleteRecipe(int id)
        {
            bool deletedIngridients = false;
            bool deletedRecipe = false;

            using (SqlConnection connection = new SqlConnection(_settings.ConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                
                using (SqlCommand cmd = new SqlCommand(DELETE_INGREDIANTS, connection))
                {
                    cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;

                    await cmd.ExecuteNonQueryAsync();

                    deletedIngridients = true;
                }

                using (SqlCommand command = new SqlCommand(DELETE_RECIPE, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    await command.ExecuteNonQueryAsync();

                    deletedRecipe = true;
                }
                connection.Close();
            }

            if (deletedIngridients && deletedRecipe)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
