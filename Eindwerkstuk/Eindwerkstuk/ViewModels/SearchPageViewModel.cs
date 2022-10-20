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

namespace Eindwerkstuk.ViewModels
{ 
    public class SearchPageViewModel : BaseViewModel
    {
        private string name;
        readonly List<Recipe> recipesList;
        readonly List<Item> itemList;
        public ObservableCollection<Recipe> Recipes { get; }
        public ObservableCollection<Item> Items { get; }
        public Command<Recipe> ItemTapped { get; }
        public Command LoadItemsCommand { get; }
        public SearchPageViewModel()
        {
            Recipes = new ObservableCollection<Recipe>();
            Items = new ObservableCollection<Item>();

            Title = "Zoeken";

            ItemTapped = new Command<Recipe>(OnItemSelected);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ExecuteLoadItemsCommand();
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                GetSearchResult(Items[0].Text);
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
            Debug.WriteLine("test");
            if (recipe == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(RecipePage)}?{nameof(RecipePageViewModel.RecipeId)}={recipe.RecipeId}");
        }
        private void GetSearchResult(string name)
        {
            Recipes.Clear();
            GetRecipesWithIngredient(name);
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
