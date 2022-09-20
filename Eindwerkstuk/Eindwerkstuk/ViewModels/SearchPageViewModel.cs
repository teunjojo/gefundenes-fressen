using Eindwerkstuk.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Eindwerkstuk.ViewModels
{ 
    public class SearchPageViewModel : BaseViewModel
    {
        public ObservableCollection<Recipe> Recipes { get; }
        public SearchPageViewModel()
        {
            Title = "Zoeken";
            Recipes = new ObservableCollection<Recipe>();
            Recipes.Add(new Recipe() { Id = Guid.NewGuid().ToString(), Name = "How To Make Classic French Toast", Image = "https://img.buzzfeed.com/thumbnailer-prod-us-east-1/video-api/assets/341495.jpg" });
        }
    }
}
