﻿using Eindwerkstuk.Models;
using Eindwerkstuk.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Eindwerkstuk.ViewModels
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
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
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
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
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
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Text = Text
        };
            Text = null;

            await DataStore.AddItemAsync(newItem);
           await ExecuteLoadItemsCommand();
        }

        async void OnDeleteItem(Item item)
        {
            if (item == null)
                return;

            await DataStore.DeleteItemAsync(item.Id);
            await ExecuteLoadItemsCommand();
        }

        async void OnSearch()
        {
            await _navigation.PushAsync(new SearchPage());
        }
    }
}