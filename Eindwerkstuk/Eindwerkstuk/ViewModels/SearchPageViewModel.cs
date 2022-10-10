﻿using Eindwerkstuk.Models;
using Eindwerkstuk.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using static System.Net.Mime.MediaTypeNames;

namespace Eindwerkstuk.ViewModels
{ 
    public class SearchPageViewModel : BaseViewModel
    {
        readonly List<Recipe> recipesList;
        public ObservableCollection<Recipe> Recipes { get; }
        public Command LoadItemsCommand { get; }
        ICommand tapCommand;
        public SearchPageViewModel()
        {
            Title = "Zoeken";
            Recipes = new ObservableCollection<Recipe>();
            GetSearchResult();
            tapCommand = new Command(OnTapped);

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }
        public ICommand TapCommand
        {
            get { return tapCommand; }
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
        void OnTapped(object s)
        {
            Debug.WriteLine("");
            Debug.WriteLine(s);
            Debug.WriteLine("");
            // This will push the RecipePage onto the navigation stack
            Shell.Current.GoToAsync($"{nameof(RecipePage)}?{nameof(RecipePageViewModel.RecipeId)}={s}");
        }
        private void GetSearchResult()
        {
            Recipes.Clear();
            Recipes.Add(new Recipe() { Id = Guid.NewGuid().ToString(), Name = "How To Make Classic French Toast", Image = "https://img.buzzfeed.com/thumbnailer-prod-us-east-1/video-api/assets/341495.jpg" });
            Recipes.Add(new Recipe() { Id = Guid.NewGuid().ToString(), Name = "Easy Beef Hand Pies", Image = "https://img.buzzfeed.com/tasty-app-user-assets-prod-us-east-1/recipes/11e6176999dd4d3fa7444224e8891cdb.jpeg" });
            Recipes.Add(new Recipe() { Id = Guid.NewGuid().ToString(), Name = "Instant Pot Texas Smoked Brisket Chowder", Image = "https://img.buzzfeed.com/tasty-app-user-assets-prod-us-east-1/recipes/1a08783ea26843a88d3b14c938976ee0.jpeg" });
        }
    }
}
