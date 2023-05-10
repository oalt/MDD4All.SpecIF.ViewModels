using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class PropertyClassesViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _specIfMetadataReader;
        private ISpecIfMetadataWriter _specIfMetadataWriter;

        public PropertyClassesViewModel(ISpecIfMetadataReader metadataReader,
                                        ISpecIfMetadataWriter metadataWriter)
        {
            _specIfMetadataReader = metadataReader;
            _specIfMetadataWriter = metadataWriter;

            List<PropertyClass> propertyClasses = metadataReader.GetAllPropertyClasses();

            PropertyClasses = new List<PropertyClassViewModel>();

            foreach (PropertyClass propertyClass in propertyClasses)
            {
                PropertyClassViewModel viewModel = new PropertyClassViewModel(metadataReader,
                                                                              metadataWriter,
                                                                              propertyClass);

                PropertyClasses.Add(viewModel);
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddPropertyClassCommand = new RelayCommand(ExecuteAddPropertyClass);
            EditPropertyClassCommand = new RelayCommand(ExecuteEditPropertyClass);
            SavePropertyClassAsNewCommand = new RelayCommand(ExecuteSavePropertyClass);
            CancelEditOperationCommand = new RelayCommand(ExecuteCancelEditOperation);
        }


        public List<PropertyClassViewModel> PropertyClasses { get; set; }

        public PropertyClassViewModel PropertyClassUnderEdit { get; set; }

        public List<DataTypeViewModel> DataTypes
        {
            get
            {
                List<DataTypeViewModel> result = new List<DataTypeViewModel>();

                List<DataType> dataTypes = _specIfMetadataReader.GetAllDataTypes();

                foreach(DataType dataType in dataTypes)
                {
                    DataTypeViewModel dataTypeViewModel = new DataTypeViewModel(dataType, _specIfMetadataReader);
                    result.Add(dataTypeViewModel);
                }

                return result;
            }
        }

        public bool EditModeActive { get; set; } = false;

        public bool StateChanged
        {
            set
            {
                if (value)
                {
                    RaisePropertyChanged("StateChanged");
                }
            }
        }

        #region COMMAND_DEFINITIONS

        public ICommand AddPropertyClassCommand { get; private set; }

        public ICommand EditPropertyClassCommand { get; private set; }

        public ICommand SavePropertyClassAsNewCommand { get; private set; }

        public ICommand CancelEditOperationCommand { get; private set; }
        #endregion

        #region COMMAND_IMPLEMENTATIONS

        private void ExecuteAddPropertyClass()
        {
            PropertyClass propertyClass = new PropertyClass();

            PropertyClassViewModel propertyClassViewModel = new PropertyClassViewModel(_specIfMetadataReader,
                                                                                       _specIfMetadataWriter,
                                                                                       propertyClass);

            PropertyClassUnderEdit = propertyClassViewModel;
            EditModeActive = true;
            StateChanged = true;
        }

        private void ExecuteEditPropertyClass()
        {
            EditModeActive = true;
            StateChanged = true;
        }

        private void ExecuteSavePropertyClass()
        {
            _specIfMetadataWriter.AddPropertyClass(PropertyClassUnderEdit.PropertyClass);
            
            _specIfMetadataReader.NotifyMetadataChanged();

            EditModeActive = false;
            StateChanged = true;
        }

        private void ExecuteCancelEditOperation()
        {
            EditModeActive = false;
            StateChanged = true;
        }

        #endregion

    }
}
