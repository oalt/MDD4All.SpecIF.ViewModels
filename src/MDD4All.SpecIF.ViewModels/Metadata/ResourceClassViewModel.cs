using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class ResourceClassViewModel : ViewModelBase
    {
        protected ISpecIfMetadataReader _specIfMetadataReader;
        protected ISpecIfMetadataWriter _specIfMetadataWriter;

        public ResourceClassViewModel(ISpecIfMetadataReader metadataReader,
                                      ISpecIfMetadataWriter metadataWriter,
                                      ResourceClass resourceClass)
        {
            _specIfMetadataReader = metadataReader;
            _specIfMetadataWriter = metadataWriter;

            ResourceClass = resourceClass;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddPropertyClassCommand = new RelayCommand<string>(ExecuteAddPropertyClass);
        }

        public ResourceClass ResourceClass { get; set; }

        public string Description
        {
            get
            {
                return ResourceClass.Description.GetDefaultStringValue();
            }

            set
            {
                //PropertyClass.Description.
            }

        }

        public List<PropertyClassViewModel> AssignedPropertyClasses
        {
            get
            {
                List<PropertyClassViewModel> result = new List<PropertyClassViewModel>();

                if (ResourceClass.PropertyClasses != null)
                {
                    foreach (Key propertyClassKey in ResourceClass.PropertyClasses)
                    {
                        PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(propertyClassKey);

                        if (propertyClass != null)
                        {
                            PropertyClassViewModel propertyClassViewModel = new PropertyClassViewModel(_specIfMetadataReader,
                                                                                                       _specIfMetadataWriter,
                                                                                                       propertyClass);

                            result.Add(propertyClassViewModel);
                        }
                    }
                }

                return result;
            }
        }

        public Key SelectedPropertyClassToAdd { get; set; }

        public ICommand AddPropertyClassCommand { get; private set; }

        private void ExecuteAddPropertyClass(string keyString)
        {
            Key key = new Key();
            key.InitailizeFromKeyString(keyString);

            if(ResourceClass.PropertyClasses == null)
            {
                ResourceClass.PropertyClasses = new List<Key>();
            }
            ResourceClass.PropertyClasses.Add(key);
        }
    }
}
