using System;

namespace Eindwerkstuk.Models
{
    using System;
    using Newtonsoft.Json;
    public class Recipe
    {
        [JsonProperty("RECIPE_ID")]
        public int RecipeId { get; set; }

        [JsonProperty("RECIPE_TITLE")]
        public string RecipeTitle { get; set; }

        [JsonProperty("RECIPE_IMGURL")]
        public Uri RecipeImgurl { get; set; }

        [JsonProperty("TOTAL_TIME")]
        public int TotalTime { get; set; }

        [JsonProperty("PREP_TIME")]
        public int PrepTime { get; set; }

        [JsonProperty("COOK_TIME")]
        public int CookTime { get; set; }

        [JsonProperty("RATING_POSITIVE")]
        public int RatingPositive { get; set; }

        [JsonProperty("RATING_NEGATIVE")]
        public int RatingNegative { get; set; }

        [JsonProperty("RATING_SCORE")]
        public int RatingScore { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("ingredients")]
        public string[] Ingredients { get; set; }

        [JsonProperty("instructions")]
        public string[] Instructions { get; set; }
    }
}
