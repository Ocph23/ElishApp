using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ElishAppDesktop.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string text;
        private string description;

        public NewItemViewModel()
        {
            SaveCommand = new CommandHandler(OnSave, ValidateSave);
            CancelCommand = new CommandHandler(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.CanExecute(null);
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text)
                && !String.IsNullOrWhiteSpace(description);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
          //  await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            
           // await Shell.Current.GoToAsync("..");
        }
    }
}
