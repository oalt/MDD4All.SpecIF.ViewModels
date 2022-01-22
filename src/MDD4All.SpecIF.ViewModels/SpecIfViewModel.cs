using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Models;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels
{
    public class SpecIfViewModel : ViewModelBase
    {


        public SpecIfViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;
        }

        private ISpecIfMetadataReader _metadataReader;

        public ISpecIfMetadataReader MetadataReader
        {
            get { return _metadataReader; }

        }

        private ISpecIfDataReader _specIfDataReader;

        public ISpecIfDataReader DataReader
        {
            get
            {
                return _specIfDataReader;
            }
        }

        private ISpecIfDataWriter _specIfDataWriter;

        public ISpecIfDataWriter DataWriter
        {
            get
            {
                return _specIfDataWriter;
            }
        }


        public List<HierarchyDescriptor> HierarchyDescriptorList
        {
            get
            {
                List<HierarchyDescriptor> result = new List<HierarchyDescriptor>();

                List<Node> hierarchies = _specIfDataReader.GetAllHierarchies();

                foreach (Node hierarchy in hierarchies)
                {
                    Resource rootResource = _specIfDataReader.GetResourceByKey(hierarchy.ResourceReference);

                    if (rootResource != null)
                    {
                        HierarchyDescriptor descriptor = new HierarchyDescriptor()
                        {
                            ID = hierarchy.ID,
                            Title = rootResource.GetTypeName(_metadataReader),
                            Type = rootResource.GetTypeName(_metadataReader)
                        };
                        result.Add(descriptor);
                    }
                }

                return result;
            }
        }

    }
}
