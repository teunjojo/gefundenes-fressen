using Eindwerkstuk.Services;
using Eindwerkstuk.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Eindwerkstuk
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
