using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        private ProjectDescriptor _projectDescriptor;
        private ISpecIfMetadataReader _metadataReader;
        private ISpecIfDataWriter _dataWriter;
        private ISpecIfDataReader _dataReader;
        private ISpecIfMetadataWriter _metadataWriter;

        public ProjectViewModel(ProjectDescriptor projectDescriptor,
                                ISpecIfMetadataReader metadataReader,
                                ISpecIfDataWriter specIfDataWriter,
                                ISpecIfDataReader specIfDataReader)
        {
            _projectDescriptor = projectDescriptor;

            _metadataReader = metadataReader;
            _dataWriter = specIfDataWriter;
            _dataReader = specIfDataReader;

            CreateNewHierarchyCommand = new RelayCommand<Resource>(ExecuteCreateNewHierarchy);

            InitializeHierarchies();
            InitializeCommands();
        }

        private void InitializeHierarchies()
        {
            List<Node> projectHierarchies = _dataReader.GetAllHierarchyRootNodes(_projectDescriptor.ID);

            foreach (Node node in projectHierarchies)
            {
                NodeViewModel hierarchyViewModel = new NodeViewModel(_metadataReader,
                                                                               _dataReader,
                                                                               _dataWriter,
                                                                               null,
                                                                               node);
                Hierarchies.Add(hierarchyViewModel);
            }
        }

        private void InitializeCommands()
        {
            DeleteHierarchyCommand = new RelayCommand<string>(ExecuteDeleteHierarchy);
        }

        public ObservableCollection<NodeViewModel> Hierarchies { get; set; } = new ObservableCollection<NodeViewModel>();

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
                string result = _projectDescriptor.Title.GetDefaultStringValue();
                if (string.IsNullOrEmpty(result))
                {
                    result = "<UNTITELED PROJECT>";
                }
                return result;
            }
        }

        public string ProjectDescription
        {
            get
            {
                string result = _projectDescriptor.Description.GetDefaultStringValue();
                if (string.IsNullOrEmpty(result))
                {
                    result = "<NO DESCRIPTION>";
                }
                return result;
            }
        }
        public ICommand CreateNewHierarchyCommand { get; private set; }
        public ICommand DeleteHierarchyCommand { get; private set; }

        private void ExecuteCreateNewHierarchy(Resource resource)
        {
            _dataWriter.AddResource(resource);

            Node rootNode = new Node
            {
                ResourceReference = new Key(resource.ID, resource.Revision)
            };

            _dataWriter.AddHierarchy(rootNode, ProjectID);

        }

        private void ExecuteDeleteHierarchy(string hierarchyKeyString)
        {
            Key key = new Key();
            key.InitailizeFromKeyString(hierarchyKeyString);
            NodeViewModel NodeToDelete = null;

            foreach (NodeViewModel hierarchy in Hierarchies)
            {
                if (hierarchy.HierarchyKey.Equals(key))
                {
                    NodeToDelete = hierarchy;
                    break;
                }
            }
            _dataWriter.DeleteNode(NodeToDelete.NodeID, ProjectID);
            Hierarchies.Remove(NodeToDelete);
        }
        public void SetProjectDescriptor(ProjectDescriptor projectDescriptor)
        {
            _projectDescriptor = projectDescriptor;
        }
    }
}
