using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        public ProjectViewModel()
        {
            CreateNewHierarchyCommand = new RelayCommand(ExecuteCreateNewHierarchy);
        }

        

        public ICommand CreateNewHierarchyCommand { get; private set; }

        private void ExecuteCreateNewHierarchy()
        {
            
        }
    }
}
