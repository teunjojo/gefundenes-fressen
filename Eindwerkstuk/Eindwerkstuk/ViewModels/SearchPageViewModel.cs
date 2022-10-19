using Eindwerkstuk.Models;
using Eindwerkstuk.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Eindwerkstuk.ViewModels
{ 
    public class SearchPageViewModel : BaseViewModel
    {
        readonly List<Recipe> recipesList;
        public ObservableCollection<Recipe> Recipes { get; }
        public Command<Recipe> ItemTapped { get; }
        public Command LoadItemsCommand { get; }
        public SearchPageViewModel()
        {
            Title = "Zoeken";
            Recipes = new ObservableCollection<Recipe>();
            GetSearchResult();

            ItemTapped = new Command<Recipe>(OnItemSelected);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                GetSearchResult();
                var recipes = await Task.FromResult(recipesList);
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        async void OnItemSelected(Recipe recipe)
        {
            if (recipe == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(RecipePage)}?{nameof(RecipePageViewModel.RecipeId)}={recipe.Id}");
        }
        private void GetSearchResult()
        {
            Recipes.Clear();
            GetRecipesWithIngredient("carrot");
            GetRecipeInfo(6);
            GetRecipeInfo(7);
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
        }
        async void GetRecipesWithIngredient(string name)
        {
            var uri = new Uri("https://recipe.teunjojo.com/with-ingredient.php?name=" + name);
            var httpClient = new HttpClient();
            var resultJson = await httpClient.GetStringAsync(uri);

            var recipeIds = JsonConvert.DeserializeObject<JArray>(resultJson);
            foreach (int id in recipeIds)
            {
                GetRecipeInfo(id);
            }
        }
    }
}
