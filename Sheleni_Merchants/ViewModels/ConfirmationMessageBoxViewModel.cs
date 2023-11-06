using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sheleni_Merchants.ViewModels
{
    class ConfirmationMessageBoxViewModel : BaseViewModel
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _confirmationResult;
        public bool ConfirmationResult
        {
            get { return _confirmationResult; }
            set { SetProperty(ref _confirmationResult, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private TaskCompletionSource<bool> confirmationTaskCompletionSource;

        public ICommand MainFrameTappedCommand { get; }
        public ConfirmationMessageBoxViewModel()
        {
            confirmationTaskCompletionSource = new TaskCompletionSource<bool>();

            MainFrameTappedCommand = new Xamarin.Forms.Command(OnMainFrameTapped);
        }

        private void OnMainFrameTapped()
        {
            confirmationTaskCompletionSource.TrySetResult(false);
            PopupNavigation.Instance.PopAsync();
        }

        public async Task<bool> GetConfirmationResult()
        {
            return await confirmationTaskCompletionSource.Task;
        }

        public void OnNoButtonClicked()
        {
            confirmationTaskCompletionSource.TrySetResult(false);
            PopupNavigation.Instance.PopAsync();
        }

        public void OnYesButtonClicked()
        {
            confirmationTaskCompletionSource.TrySetResult(true);
            PopupNavigation.Instance.PopAsync();
        }
    }
}
