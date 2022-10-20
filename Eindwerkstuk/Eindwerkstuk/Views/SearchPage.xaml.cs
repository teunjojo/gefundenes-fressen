﻿using System;
using System.Collections.Generic;
using System.Linq;
using Eindwerkstuk.ViewModels;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Eindwerkstuk.Models;

namespace Eindwerkstuk.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        async private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(RecipePage)}?{nameof(RecipePageViewModel.RecipeId)}={55}");
        }
    }
}