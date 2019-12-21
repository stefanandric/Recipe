using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRESTFull.Model
{
    public class Ingrediant
    {
        public int ID { get; set; }

        public int RecipeID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("amount")]
        public int Count { get; set; }
    }
}
