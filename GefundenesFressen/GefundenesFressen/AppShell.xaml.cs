using GefundenesFressen.ViewModels;
using GefundenesFressen.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GefundenesFressen
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(RecipePage), typeof(RecipePage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        }

    }
}
