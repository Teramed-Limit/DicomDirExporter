using System;
using DicomDirExporter.Enumerate;
using GalaSoft.MvvmLight;

namespace DicomDirExporter.Logic
{
    public class SnackbarMessenger : ObservableObject
    {
        private SnackbarType _snackbarType = SnackbarType.INFO;
        private MainSnackbarMessageQueue _snackbarMessageQueue;

        public SnackbarMessenger(MainSnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;
        }

        public MainSnackbarMessageQueue SnackbarMessageQueue
        {
            get { return _snackbarMessageQueue; }
            set
            {
                _snackbarMessageQueue = value;
                RaisePropertyChanged();
            }
        }

        public SnackbarType SnackbarType
        {
            get => _snackbarType;
            set
            {
                _snackbarType = value;
                RaisePropertyChanged();
            }
        }

        private void Show(string message)
        {
            SnackbarMessageQueue.Clear();
            SnackbarMessageQueue.Enqueue(
                message,
                null,
                null,
                null,
                false,
                true,
                TimeSpan.FromSeconds(1));
        }

        public void ShowErrorMessage(string message)
        {
            SnackbarType = SnackbarType.ERROR;
            Show(message);
        }

        public void ShowInfoMessage(string message)
        {
            SnackbarType = SnackbarType.INFO;
            Show(message);
        }

        public void ShowSuccessMessage(string message)
        {
            SnackbarType = SnackbarType.SUCCESS;
            Show(message);
        }

        public void ShowWarringMessage(string message)
        {
            SnackbarType = SnackbarType.WARRING;
            Show(message);
        }
    }
}