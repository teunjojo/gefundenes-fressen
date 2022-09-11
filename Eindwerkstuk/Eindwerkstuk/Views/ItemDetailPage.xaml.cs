using Eindwerkstuk.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Eindwerkstuk.Views
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