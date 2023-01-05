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

        public Action ConnectAction { get; set; }

        public ISpecIfDataProviderFactory SpecIfDataProviderFactory { get; set; }

        public ICommand ConnectCommand { get; private set; }
    }
}
