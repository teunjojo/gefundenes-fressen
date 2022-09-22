using Eindwerkstuk.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Eindwerkstuk.ViewModels
{
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class RecipePageViewModel : BaseViewModel
    {
        private string recipeId;
        private string name;
        private string description;
        readonly List<Recipe> recipesList;
        public ObservableCollection<Recipe> Recipes { get; }

        public RecipePageViewModel()
        {
            // FIXME: fix pagina titel
            Title = Name;
            Recipes = new ObservableCollection<Recipe>();
        }
        public string Id { get; set; }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string RecipeId
        {
            get
            {
                return recipeId;
            }
            set
            {
                recipeId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string recipeId)
        {
            try
            {
                var item = await Task.FromResult(recipesList.FirstOrDefault(s => s.Id == recipeId));
                Id = item.Id;
                Name = item.Name;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
