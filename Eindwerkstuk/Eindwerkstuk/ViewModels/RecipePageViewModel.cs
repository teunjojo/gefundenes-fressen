using Eindwerkstuk.Models;
using Eindwerkstuk.Views;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Eindwerkstuk.ViewModels
{
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class RecipePageViewModel : BaseViewModel
    {
        private int recipeId;
        private string title;
        private string[] ingredients;
        private string[] instructions;
        private string[] tags;
        private Uri img;
        private string totaltime;
        private string preptime;
        private string cooktime;
        private bool totaltimeexists;
        private bool cooktimeexists;
        private bool preptimeexists;
        private bool timeexists;
        private string favimg;
        readonly List<Recipe> recipesList;
        public Command SearchTagCommand { get; }
        public Command FavoriteCommand { get; }
        public ObservableCollection<Recipe> Recipes { get; }

        public RecipePageViewModel()
        {
            Recipes = new ObservableCollection<Recipe>();
            SearchTagCommand = new Command<string>(ByTag);
        }
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }
        public string FavoriteImage
        {
            get
            {
                return favimg;
            }
            set { favimg = value; OnPropertyChanged(); }
        }
        public Uri RecipeImgurl
        {
            get { return img; }
            set { img = value; OnPropertyChanged(); }
        }
        public string TotalTime
        {
            get { return totaltime; }
            set { totaltime = value; OnPropertyChanged(); }
        }
        public string PrepTime
        {
            get { return preptime; }
            set { preptime = value; OnPropertyChanged(); }
        }
        public string CookTime
        {
            get { return cooktime; }
            set { cooktime = value; OnPropertyChanged(); }
        }
        public string[] Ingredients
        {
            get { return ingredients; }
            set { ingredients = value; OnPropertyChanged(); }
        }
        public string[] Instructions
        {
            get { return instructions; }
            set { instructions = value; OnPropertyChanged(); }
        }
        public string[] Tags
        {
            get { return tags; }
            set { tags = value; OnPropertyChanged(); }
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
        public bool TotalTimeExists
        {
            get { return totaltimeexists; }
            set { totaltimeexists = value; OnPropertyChanged(); }
        }
        public bool CookTimeExists
        {
            get { return cooktimeexists; }
            set { cooktimeexists = value; OnPropertyChanged(); }
        }
        public bool PrepTimeExists
        {
            get { return preptimeexists; }
            set { preptimeexists = value; OnPropertyChanged(); }
        }

        public bool TimeExists
        {
            get { return timeexists; }
            set { timeexists = value; OnPropertyChanged(); }
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
            if (!string.IsNullOrEmpty(resultRecipe.TotalTime)) if (int.Parse(resultRecipe.TotalTime) > 0) { resultRecipe.TotalTimeExists = true; }
            if (!string.IsNullOrEmpty(resultRecipe.PrepTime)) if (int.Parse(resultRecipe.PrepTime) > 0) { resultRecipe.PrepTimeExists = true; }
            if (!string.IsNullOrEmpty(resultRecipe.CookTime)) if (int.Parse(resultRecipe.CookTime) > 0) { resultRecipe.CookTimeExists = true; }

            if (resultRecipe.TotalTimeExists) { resultRecipe.TotalTime = GetReadableTime(resultRecipe.TotalTime); }
            if (resultRecipe.PrepTimeExists) { resultRecipe.PrepTime = GetReadableTime(resultRecipe.PrepTime); }
            if (resultRecipe.CookTimeExists) { resultRecipe.CookTime = GetReadableTime(resultRecipe.CookTime); }

            if (resultRecipe.TotalTimeExists || resultRecipe.CookTimeExists) { TimeExists = true; } else { TimeExists = false; }
            Recipes.Add(resultRecipe);
            Title = resultRecipe.RecipeTitle;
            RecipeImgurl = resultRecipe.RecipeImgurl;
            Instructions = resultRecipe.Instructions;
            Ingredients = resultRecipe.Ingredients;
            Tags = resultRecipe.Tags;
            TotalTime = resultRecipe.TotalTime;
            PrepTime = resultRecipe.PrepTime;
            CookTime = resultRecipe.CookTime;
            TotalTimeExists = resultRecipe.TotalTimeExists;
            CookTimeExists = resultRecipe.CookTimeExists;
            PrepTimeExists = resultRecipe.PrepTimeExists;

            List<Saved> recipes;
            recipes = new List<Saved>();
            if (CrossSettings.Current.Contains("Fav")) recipes = JsonConvert.DeserializeObject<List<Saved>>(CrossSettings.Current.GetValueOrDefault("Fav", string.Empty));
            FavoriteImage = "favorite.png";
            foreach (var recipe in recipes)
            {
                if (id == recipe.RecipeId)
                {
                    FavoriteImage = "favoriteFill.png";
                }
            }
        }
        public string GetReadableTime(string m)
        {
            int minutes = int.Parse(m);
            string unit;
            if (minutes > 1) { unit = " minutes"; } else { unit = " minute"; }
            string readable = minutes.ToString() + unit;
            if (minutes / 60 >= 1)
            {
                int hours = minutes / 60;
                if (hours > 1) { unit = " hours"; } else { unit = " hour"; }
                readable = hours.ToString() + unit;
            }
            return readable;
        }
        private async void ByTag(string value)
        {
            await Shell.Current.GoToAsync($"{nameof(SearchPage)}?Tag={value}");
        }
        private void Favorite()
        {
            List<Saved> saved;
            saved = new List<Saved>();
            if (CrossSettings.Current.Contains("Fav")) saved = JsonConvert.DeserializeObject<List<Saved>>(CrossSettings.Current.GetValueOrDefault("Fav", string.Empty));
            foreach (var recipe in saved)
            {
                if (RecipeId == recipe.RecipeId)
                {
                    FavoriteImage = "favorite.png";
                    saved.RemoveAll(p => p.RecipeId == recipe.RecipeId);
                    CrossSettings.Current.AddOrUpdateValue("Fav", JsonConvert.SerializeObject(saved));
                    return;
                }
            }
            saved.Add(new Saved { RecipeId = RecipeId });
            CrossSettings.Current.AddOrUpdateValue("Fav", JsonConvert.SerializeObject(saved));
            FavoriteImage = "favoriteFill.png";
        }
    }
}
