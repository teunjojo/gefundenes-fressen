using GefundenesFressen.Models;
using GefundenesFressen.Views;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GefundenesFressen.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;
        private INavigation _navigation;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command SearchCommand { get; }
        public Command<Item> ItemTapped { get; }
        public Command<Item> RemoveItemCommand { get; }

        public ItemsViewModel()
        {
            Title = "Ingrediënten";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);

            RemoveItemCommand = new Command<Item>(OnDeleteItem);

            SaveCommand = new Command(OnSave, ValidateSave);

            SearchCommand = new Command(OnSearch);

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            ExecuteLoadItemsCommand();
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = JsonConvert.DeserializeObject<List<Item>>(CrossSettings.Current.GetValueOrDefault("Ingredients", string.Empty));
                foreach (var item in items)
                {
                    Items.Add(item);
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
        }
        private string text;

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnSave()
        {
            // Create List<Item>
            List<Item> items;
            items = new List<Item>();
            // If CrossSettings exists; import values to List<Item>
            if (CrossSettings.Current.Contains("Ingredients")) items = JsonConvert.DeserializeObject<List<Item>>(CrossSettings.Current.GetValueOrDefault("Ingredients", string.Empty));
            foreach (var item in items)
            {
                Items.Add(item);
            }
            // Add new Item
            items.Add(new Item { Id = Guid.NewGuid().ToString(), Text = Text });
            Text = null;

            // Store Item
            CrossSettings.Current.AddOrUpdateValue("Ingredients", JsonConvert.SerializeObject(items));
            await ExecuteLoadItemsCommand();
        }

        async void OnDeleteItem(Item item)
        {
            // Delete Item where item.Id From CrossSettings
            List<Item> ingrs = JsonConvert.DeserializeObject<List<Item>>(CrossSettings.Current.GetValueOrDefault("Ingredients", string.Empty));
            foreach (var ingr in ingrs)
            {
                if (item.Id == ingr.Id)
                {
                    ingrs.RemoveAll(p => p.Id == ingr.Id);
                    // Put remaining Items back in CrossSettings
                    CrossSettings.Current.AddOrUpdateValue("Ingredients", JsonConvert.SerializeObject(ingrs));
                    await ExecuteLoadItemsCommand();
                    return;
                }
            }
        }

        private async void OnSearch()
        {
            // If Ingredients exist goto searchpage with parameter ?Ingredients= 'Ingredients'
            if (CrossSettings.Current.Contains("Ingredients")) await Shell.Current.GoToAsync($"{nameof(SearchPage)}?Ingredients={CrossSettings.Current.GetValueOrDefault("Ingredients", string.Empty)}");
        }
    }
}