using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _metadataReader;
        private ISpecIfDataWriter _dataWriter;
        private ISpecIfDataReader _dataReader;

        public ProjectViewModel(ISpecIfMetadataReader metadataReader,
                                ISpecIfDataWriter specIfDataWriter,
                                ISpecIfDataReader specIfDataReader)
        {
            _metadataReader = metadataReader;
            _dataWriter = specIfDataWriter;
            _dataReader = specIfDataReader;

            CreateNewHierarchyCommand = new RelayCommand<Resource>(ExecuteCreateNewHierarchy);

            InitializeHierarchies();
        }

        private void InitializeHierarchies()
        {
            List<Node> hierarchies = _dataReader.GetAllHierarchies();

            foreach(Node node in hierarchies)
            {
                HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
                                                                               _dataReader,
                                                                               _dataWriter,
                                                                               node);
                Hierarchies.Add(hierarchyViewModel);
            }
        }

        public ObservableCollection<HierarchyViewModel> Hierarchies { get; set; } = new ObservableCollection<HierarchyViewModel>();

        public ICommand CreateNewHierarchyCommand { get; private set; }

        private void ExecuteCreateNewHierarchy(Resource resource)
        {
            _dataWriter.AddResource(resource);

            Node rootNode = new Node
            {
                ResourceReference = new Key(resource.ID, resource.Revision)
            };

            _dataWriter.AddHierarchy(rootNode);

            HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
                                                                           _dataReader,
                                                                           _dataWriter,
                                                                           new Key(rootNode.ID, rootNode.Revision));

            Hierarchies.Add(hierarchyViewModel);
        }
    }
}
