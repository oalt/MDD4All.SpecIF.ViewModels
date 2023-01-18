using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class DataConnectorViewModel : ViewModelBase
    {
        public DataConnectorViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ConnectCommand = new RelayCommand(ExecuteConnectCommand);
        }

        private void ExecuteConnectCommand()
        {
            if(ConnectAction != null)
            {
                ConnectAction();
            }
        }

        private bool _isConnecting = false;

        public bool IsConnecting 
        {
            get
            { 
                return _isConnecting;
            } 
            
            set
            {
                _isConnecting = value;
                RaisePropertyChanged("IsConnecting");
            }
        }

        private string _statusMessageKey = "";

        public string StatusMessageKey
        {
            get 
            { 
                return _statusMessageKey; 
            }

            set
            {
                _statusMessageKey = value;
                RaisePropertyChanged("StatusMessageKey");
            }
        }


        public Action ConnectAction { get; set; }

        public ISpecIfDataProviderFactory SpecIfDataProviderFactory { get; set; }

        public ICommand ConnectCommand { get; private set; }
    }
}
