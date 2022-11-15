using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class DataTypesViewModel : ViewModelBase
    {

        public ISpecIfMetadataReader DataProvider { get; set; }

        public DataTypesViewModel(ISpecIfMetadataReader dataProvider)
        {
            DataProvider = dataProvider;
        }

        public List<DataTypeViewModel> DataTypes
        {
            get
            {
                List<DataTypeViewModel> result = new List<DataTypeViewModel>();

                List<DataType> dataTypes = DataProvider.GetAllDataTypes();

                foreach (DataType dataType in dataTypes)
                {
                    result.Add(new DataTypeViewModel(dataType, DataProvider));
                }

                return result;
            }
        }
    }
}
