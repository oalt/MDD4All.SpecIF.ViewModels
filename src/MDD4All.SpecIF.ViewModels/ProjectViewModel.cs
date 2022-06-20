using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using System.Collections.Generic;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.ObjectModel;

namespace MDD4All.SpecIF.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        private ProjectDescriptor _projectDescriptor;
        private ISpecIfMetadataReader _metadataReader;
        private ISpecIfDataWriter _dataWriter;
        private ISpecIfDataReader _dataReader;

        public ProjectViewModel(ProjectDescriptor projectDescriptor, 
                                ISpecIfMetadataReader metadataReader,
                                ISpecIfDataWriter specIfDataWriter,
                                ISpecIfDataReader specIfDataReader)
        {
            _projectDescriptor = projectDescriptor;

            _metadataReader = metadataReader;
            _dataWriter = specIfDataWriter;
            _dataReader = specIfDataReader;

            InitializeHierarchies();
        }

        private void InitializeHierarchies()
        {
            List<Node> projectHierarchies = _dataReader.GetAllHierarchyRootNodes(_projectDescriptor.ID);

            foreach (Node node in projectHierarchies)
            {
                HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
                                                                               _dataReader,
                                                                               _dataWriter,
                                                                               node);
                Hierarchies.Add(hierarchyViewModel);
            }
        }

        public ObservableCollection<HierarchyViewModel> Hierarchies { get; set; } = new ObservableCollection<HierarchyViewModel>();

        public string ProjectID
        {
            get 
            { 
                return _projectDescriptor.ID; 
            }
        }

        public string ProjectTitle
        {
            get 
            {
                string result = _projectDescriptor.Title.GetStringValue();
                if(string.IsNullOrEmpty(result))
                {
                    result = "<UNTITELED PROJECT>";
                }
                return result; 
            }
            
        }

    }
}
