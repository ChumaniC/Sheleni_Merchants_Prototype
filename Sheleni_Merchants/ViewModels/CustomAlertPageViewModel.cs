using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sheleni_Merchants.ViewModels
{
    public class CustomAlertPageViewModel : BaseViewModel
    {
        private bool _confirmationResult;
        private string _icon;

        private string _message;

        private string _title;

        // Define a TaskCompletionSource to track the result of the confirmation
        private TaskCompletionSource<bool> confirmationTaskCompletionSource;

        public CustomAlertPageViewModel()
        {
            confirmationTaskCompletionSource = new TaskCompletionSource<bool>();

            MainFrameTappedCommand = new Xamarin.Forms.Command(OnMainFrameTapped);
        }

        public bool ConfirmationResult
        {
            get { return _confirmationResult; }
            set { SetProperty(ref _confirmationResult, value); }
        }

        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        public ICommand MainFrameTappedCommand { get; }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public Task<bool> GetConfirmationResult()
        {
            return confirmationTaskCompletionSource.Task;
        }

        public void OnOkayButtonClicked()
        {
            confirmationTaskCompletionSource.TrySetResult(true);
            PopupNavigation.Instance.PopAsync();
        }

        private void OnMainFrameTapped()
        {
            confirmationTaskCompletionSource.TrySetResult(true);
            PopupNavigation.Instance.PopAsync();
        }
    }
}
