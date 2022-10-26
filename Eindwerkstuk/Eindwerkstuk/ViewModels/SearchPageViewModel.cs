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
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Plugin.Settings;

namespace Eindwerkstuk.ViewModels
{
    [QueryProperty(nameof(Ingredients), "Ingredients")]
    [QueryProperty(nameof(Tag), "Tag")]
    public class SearchPageViewModel : BaseViewModel
    {
        int numbItems = 0;
        readonly List<Recipe> recipesList;
        readonly List<Item> itemList;
        public ObservableCollection<Recipe> Recipes { get; }
        public ObservableCollection<Item> Items { get; }
        public Command<Recipe> ItemTapped { get; }
        public Command LoadMore { get; }
        public Command LoadItemsCommand { get; }
        public SearchPageViewModel()
        {
            Recipes = new ObservableCollection<Recipe>();
            Items = new ObservableCollection<Item>();

            Title = "Search";

            ItemTapped = new Command<Recipe>(OnItemSelected);
            LoadMore = new Command(LoadMoreItems);
        }
        public string Name;
        public string Ingredients
        {
            set
            {
                Items.Clear();
                // Convert Json (from parameter) to List (with model <Item>)
                var items = JsonConvert.DeserializeObject<List<Item>>(value);
                // For each value in List
                // Make the string Name from each item in the list, seperated by commas (like 'apple,carrot,pickle')
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(Name)) { Name += ","; }
                    Name += WebUtility.UrlEncode(item.Text);
                }
                // Make the search
                SearchIngredients();
            }
        }
        public string Tag
        {
            set
            {
                SearchTag(value);
            }
        }

        async void SearchIngredients()
        {
            Recipes.Clear();
            GetRecipesWithIngredients(Name, 0);
        }

        async void SearchTag(string value)
        {
            Recipes.Clear();
            GetRecipesWithTag(value, 0);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
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

            await Shell.Current.GoToAsync($"{nameof(RecipePage)}?{nameof(RecipePageViewModel.RecipeId)}={recipe.RecipeId}");
        }
        private void LoadMoreItems()
        {
            numbItems += 20;
            GetRecipesWithIngredients(Name, numbItems);
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
            if (!string.IsNullOrEmpty(resultRecipe.TotalTime)) if (int.Parse(resultRecipe.TotalTime) > 0) { resultRecipe.TotalTimeExists = true; }
            if (!string.IsNullOrEmpty(resultRecipe.PrepTime)) if (int.Parse(resultRecipe.PrepTime) > 0)  { resultRecipe.PrepTimeExists = true; }
            if (!string.IsNullOrEmpty(resultRecipe.CookTime)) if (int.Parse(resultRecipe.CookTime) > 0) { resultRecipe.CookTimeExists = true; }

            if (resultRecipe.TotalTimeExists) { resultRecipe.TotalTime = GetReadableTime(resultRecipe.TotalTime); }
            if (resultRecipe.PrepTimeExists) { resultRecipe.PrepTime = GetReadableTime(resultRecipe.PrepTime); }
            if (resultRecipe.CookTimeExists) { resultRecipe.CookTime = GetReadableTime(resultRecipe.CookTime); }

            Recipes.Add(resultRecipe);
        }
        async void GetRecipesWithIngredients(string name, int from)
        {
            var uri = new Uri("https://recipe.teunjojo.com/with-ingredient.php?name=" + name + "&from=" + from);
            var httpClient = new HttpClient();
            var resultJson = await httpClient.GetStringAsync(uri);

            var recipeIds = JsonConvert.DeserializeObject<JArray>(resultJson);
            foreach (int id in recipeIds)
            {
                GetRecipeInfo(id);
            }
        }
        async void GetRecipesWithTag(string name, int from)
        {
            var uri = new Uri("https://recipe.teunjojo.com/with-tag.php?name=" + name + "&from=" + from);
            var httpClient = new HttpClient();
            var resultJson = await httpClient.GetStringAsync(uri);

            var recipeIds = JsonConvert.DeserializeObject<JArray>(resultJson);
            foreach (int id in recipeIds)
            {
                GetRecipeInfo(id);
            }
        }
        public string GetReadableTime(string m)
        {
            int minutes = int.Parse(m);
            string readable = minutes.ToString() + " m";
            if (minutes / 60 > 1)
            {
                int hours = minutes / 60;
                readable = hours.ToString() + " h";
            }
            return readable;
        }
    }
}
