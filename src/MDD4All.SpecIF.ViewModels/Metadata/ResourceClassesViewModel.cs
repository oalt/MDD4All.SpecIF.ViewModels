using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class ResourceClassesViewModel : ViewModelBase
    {

        protected ISpecIfMetadataReader _specIfMetadataReader;
        protected ISpecIfMetadataWriter _specIfMetadataWriter;

        public ResourceClassesViewModel(ISpecIfMetadataReader metadataReader,
                                        ISpecIfMetadataWriter metadataWriter)
        {
            _specIfMetadataReader = metadataReader;
            _specIfMetadataWriter = metadataWriter;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddResourceClassCommand = new RelayCommand(ExecuteAddResourceClass);
            EditResourceClassCommand = new RelayCommand(ExecuteEditResourceClass);
            SaveResourceClassAsNewCommand = new RelayCommand(ExecuteSaveResourceClass);
            CancelEditOperationCommand = new RelayCommand(ExecuteCancelEditOperation);
        }

        public List<ResourceClassViewModel> ResourceClasses
        {
            get
            {
                List<ResourceClassViewModel> result = new List<ResourceClassViewModel>();

                List<ResourceClass> resourceClasses = _specIfMetadataReader.GetAllResourceClasses();

                foreach (ResourceClass resourceClass in resourceClasses)
                {
                    result.Add(new ResourceClassViewModel(_specIfMetadataReader, _specIfMetadataWriter, resourceClass));
                }

                return result;
            }
        }

        public List<PropertyClassViewModel> PropertyClasses
        {
            get
            {
                List<PropertyClassViewModel> result = new List<PropertyClassViewModel>();

                List<PropertyClass> propertyClasses = _specIfMetadataReader.GetAllPropertyClasses();

                foreach (PropertyClass propertyClass in propertyClasses)
                {
                    PropertyClassViewModel propertyClassViewModel = new PropertyClassViewModel(_specIfMetadataReader, _specIfMetadataWriter, propertyClass);

                    result.Add(propertyClassViewModel);
                }
                return result;
            }
        }

        public bool EditModeActive { get; set; } = false;

        public ResourceClassViewModel ResourceClassUnderEdit { get; set; }

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

        public ICommand AddResourceClassCommand { get; private set; }

        public ICommand EditResourceClassCommand { get; private set; }

        public ICommand SaveResourceClassAsNewCommand { get; private set; }

        public ICommand CancelEditOperationCommand { get; private set; }
        #endregion

        #region COMMAND_IMPLEMENTATIONS

        private void ExecuteAddResourceClass()
        {
            ResourceClass resourceClass = new ResourceClass();

            ResourceClassViewModel resourceClassViewModel = new ResourceClassViewModel(_specIfMetadataReader,
                                                                                       _specIfMetadataWriter,
                                                                                       resourceClass);

            ResourceClassUnderEdit = resourceClassViewModel;
            EditModeActive = true;
            StateChanged = true;
        }

        private void ExecuteEditResourceClass()
        {
            EditModeActive = true;
            StateChanged = true;
        }

        private void ExecuteSaveResourceClass()
        {
            _specIfMetadataWriter.AddResourceClass(ResourceClassUnderEdit.ResourceClass);

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
