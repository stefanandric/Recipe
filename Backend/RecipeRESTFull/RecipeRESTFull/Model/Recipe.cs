using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRESTFull.Model
{
    public class Recipe
    {
        public int ID { get; set; }

        [JsonProperty("ingredients")]
        public List<Ingrediant> Ingrediants { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imagePath")]
        public string ImgPath { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
