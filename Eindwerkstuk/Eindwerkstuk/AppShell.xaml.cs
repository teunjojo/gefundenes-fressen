using Eindwerkstuk.ViewModels;
using Eindwerkstuk.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Eindwerkstuk
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
        }

    }
}
