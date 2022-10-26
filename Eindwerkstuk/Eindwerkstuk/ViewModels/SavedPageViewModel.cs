using Eindwerkstuk.Models;
using Eindwerkstuk.Views;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Eindwerkstuk.ViewModels
{
    internal class SavedPageViewModel : BaseViewModel
    {
        readonly List<Recipe> recipesList;
        readonly List<Item> itemList;
        public ObservableCollection<Recipe> Recipes { get; }
        public ObservableCollection<Item> Items { get; }
        public Command<Recipe> ItemTapped { get; }
        public Command LoadItemsCommand { get; }
        public SavedPageViewModel()
        {
            Recipes = new ObservableCollection<Recipe>();

            Title = "Favorite";

            ItemTapped = new Command<Recipe>(OnItemSelected);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            if (CrossSettings.Current.Contains("Fav")) ;
                SearchSaved();
        }
        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        async void SearchSaved()
        {
            Recipes.Clear();
            List<Saved> recipes;
            recipes = new List<Saved>();
            if (CrossSettings.Current.Contains("Fav")) recipes = JsonConvert.DeserializeObject<List<Saved>>(CrossSettings.Current.GetValueOrDefault("Fav", string.Empty));
            foreach (var recipe in recipes)
            {
                if (recipe.RecipeId == 0) return;
                GetRecipeInfo(recipe.RecipeId);
            }
            IsRefreshing = false;
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            SearchSaved();
            IsBusy = false;
        }
        async void OnItemSelected(Recipe recipe)
        {
            if (recipe == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(RecipePage)}?{nameof(RecipePageViewModel.RecipeId)}={recipe.RecipeId}");
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
            if (!string.IsNullOrEmpty(resultRecipe.PrepTime)) if (int.Parse(resultRecipe.PrepTime) > 0) { resultRecipe.PrepTimeExists = true; }
            if (!string.IsNullOrEmpty(resultRecipe.CookTime)) if (int.Parse(resultRecipe.CookTime) > 0) { resultRecipe.CookTimeExists = true; }

            if (resultRecipe.TotalTimeExists) { resultRecipe.TotalTime = GetReadableTime(resultRecipe.TotalTime); }
            if (resultRecipe.PrepTimeExists) { resultRecipe.PrepTime = GetReadableTime(resultRecipe.PrepTime); }
            if (resultRecipe.CookTimeExists) { resultRecipe.CookTime = GetReadableTime(resultRecipe.CookTime); }

            Recipes.Add(resultRecipe);
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
