﻿using Eindwerkstuk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerkstuk.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecipePage : ContentPage
    {
        RecipePageViewModel _viewModel;
        public RecipePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new RecipePageViewModel();
        }
    }
}