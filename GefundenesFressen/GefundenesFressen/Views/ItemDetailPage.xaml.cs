using GefundenesFressen.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace GefundenesFressen.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}