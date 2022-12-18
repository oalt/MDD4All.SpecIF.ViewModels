using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels.Revisioning
{
    public class ResourceRevisionViewModel : RevisionViewModel
    {
        public ResourceRevisionViewModel(ISpecIfMetadataReader metadataReader,
                                         ISpecIfDataReader dataReader,
                                         ISpecIfDataWriter dataWriter,
                                         Key currentRevision) : base(metadataReader,
                                                                     dataReader, 
                                                                     dataWriter, 
                                                                     currentRevision)
        {

        }

        protected override void InitializeResourceViewModels()
        {
            ISpecIfDataReader dataReader = _dataReader;

            List<Resource> resourceRevisions = dataReader.GetAllResourceRevisions(CurrentRevision.ID);

            if(resourceRevisions != null)
            {
                ResourceViewModels = new List<ResourceViewModel>();

                foreach(Resource resource in resourceRevisions)
                {
                    ResourceViewModel resourceViewModel = new ResourceViewModel(_metadataReader,
                                                                                _dataReader,
                                                                                _dataWriter,
                                                                                resource);

                    ResourceViewModels.Add(resourceViewModel);
                }
            }
        }
    }
}
