using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.DataModels.Manipulation;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class PropertyClassViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _specIfMetadataReader;
        private ISpecIfMetadataWriter _specIfMetadataWriter;

        public PropertyClassViewModel(ISpecIfMetadataReader metadataReader, 
                                      ISpecIfMetadataWriter metadataWriter,
                                      PropertyClass propertyClass)
        {
            _specIfMetadataReader = metadataReader;
            _specIfMetadataWriter = metadataWriter;

            PropertyClass = propertyClass;

            
        }

        public PropertyClass PropertyClass { get; private set; }

        public Key DataTypeKey
        {
            get
            {
                return PropertyClass.DataType;
            }

            set
            {
                PropertyClass.DataType = value;
            }
        }

        public string Description
        {
            get
            {
                return PropertyClass.Description.GetDefaultStringValue();
            }

            set
            {
                //PropertyClass.Description.
            }

        }

        public string KeyString
        {
            get
            {
                string result = "";

                Key key = new Key(PropertyClass.ID, PropertyClass.Revision);

                result = key.ToString();

                return result;
            }
        }

    }
}
