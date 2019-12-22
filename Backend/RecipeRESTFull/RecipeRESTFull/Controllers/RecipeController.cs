using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecipeRESTFull.Interfaces;
using RecipeRESTFull.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeRESTFull.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : Controller
    {
        private IRecipeService service;

        public RecipeController(IRecipeService recipeService)
        {
            service = recipeService;
        }
        // GET api/values
        [HttpGet("recipes")]
        public async Task<ActionResult<Recipe[]>> Get()
        {
            List<Recipe> recipe = await service.GetRecipes();
            return Ok(recipe);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int recipeId)
        {
            return Ok(service.GetRecipeById(recipeId));
        }

        // POST api/values
        [HttpPost("postRecipe")]
        public async Task<IActionResult> Post([FromBody] List<Recipe> recipes)
        {
            recipes.ForEach(x => x.DateCreated = DateTime.UtcNow);

            await service.InsertRecipe(recipes);

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            bool deleted = await service.RemoveRecipe(id).ConfigureAwait(false);

            if (deleted)
            {
                return Ok(deleted);
            }
            else
            {
                return NotFound(deleted);
            }
        }
    }
}
