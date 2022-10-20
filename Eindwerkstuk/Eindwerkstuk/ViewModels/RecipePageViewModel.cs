using Eindwerkstuk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Eindwerkstuk.ViewModels
{
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class RecipePageViewModel : BaseViewModel
    {
        private int recipeId;
        private string title;
        private string[] instructions;
        private Uri img;
        readonly List<Recipe> recipesList;
        public ObservableCollection<Recipe> Recipes { get; }

        public RecipePageViewModel()
        {

            Recipes = new ObservableCollection<Recipe>();
        }
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }
        public Uri RecipeImgurl
        {
            get { return img; }
            set { img = value; }
        }
        public string[] Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }

        public int RecipeId
        {
            get
            {
                return recipeId;
            }
            set
            {
                recipeId = value;
                GetRecipeInfo(value);
            }
        }

        public async void LoadItemId(int recipeId)
        {
            try
            {
                var recipes = await Task.FromResult(recipesList);
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
                Debug.WriteLine("asdfsdf");
                //var currentRecipe = recipes.Find(x => x.RecipeTitle.Contains("test"));
                Console.WriteLine("\nFind: Part where name contains \"e\": {0}",
            recipes.Find(x => x.RecipeTitle.Contains("e")).RecipeId);
                Debug.WriteLine("testt");
                //Debug.WriteLine(currentRecipe.RecipeTitle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        async void GetRecipeInfo(int id)
        {
            var uri = new Uri("https://recipe.teunjojo.com/recipe.php?id=" + id);
            var httpClient = new HttpClient();
            var resultJson = await httpClient.GetStringAsync(uri);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var resultRecipe =
                JsonConvert.DeserializeObject<Recipe>(resultJson, settings);
            Recipes.Add(resultRecipe);
            Title = resultRecipe.RecipeTitle;
            RecipeId = resultRecipe.RecipeId;
            RecipeImgurl = resultRecipe.RecipeImgurl;
            Instructions = resultRecipe.Instructions;

        }
    }
}
