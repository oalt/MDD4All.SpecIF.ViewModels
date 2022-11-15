using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class DataTypeViewModel
    {
        public ISpecIfMetadataReader DataProvider { get; set; }

        public DataType DataType { get; set; }

        public DataTypeViewModel(DataType dataType, ISpecIfMetadataReader dataProvider)
        {
            DataType = dataType;
            DataProvider = dataProvider;
        }

        private string _editType = "edit";

        public string EditType
        {
            get { return _editType; }
            set { _editType = value; }
        }

        public string KeyString
        {
            get
            {
                Key key = new Key(DataType.ID, DataType.Revision);

                return key.ToString();
            }
        }

        public string DisplayedName
        {
            get
            {
                return DataType.Title + " [" + DataType.Revision + "]";
            }
        }
    }
}
