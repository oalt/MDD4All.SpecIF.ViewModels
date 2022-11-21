using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class ProjectsViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _metadataReader;
        private ISpecIfDataWriter _dataWriter;
        private ISpecIfDataReader _dataReader;

        public ProjectsViewModel(ISpecIfMetadataReader metadataReader,
                                ISpecIfDataWriter specIfDataWriter,
                                ISpecIfDataReader specIfDataReader)
        {
            _metadataReader = metadataReader;
            _dataWriter = specIfDataWriter;
            _dataReader = specIfDataReader;

            CreateNewHierarchyCommand = new RelayCommand<Resource>(ExecuteCreateNewHierarchy);

            InitializeProjects();
        }

        private void InitializeProjects()
        {
            Projects = new List<ProjectViewModel>();

            List<ProjectDescriptor> projectDescriptors = _dataReader.GetProjectDescriptions();

            foreach(ProjectDescriptor projectDescriptor in projectDescriptors)
            {
                ProjectViewModel projectViewModel = new ProjectViewModel(projectDescriptor, 
                                                                         _metadataReader,
                                                                         _dataWriter,
                                                                         _dataReader);
                                                                         
                Projects.Add(projectViewModel);

                
            }
           

            
        }

        public List<ProjectViewModel> Projects { get; private set; } = new List<ProjectViewModel>();

        

        public ICommand CreateNewHierarchyCommand { get; private set; }

        private void ExecuteCreateNewHierarchy(Resource resource)
        {
            _dataWriter.AddResource(resource);

            Node rootNode = new Node
            {
                ResourceReference = new Key(resource.ID, resource.Revision)
            };

            _dataWriter.AddHierarchy(rootNode);

            NodeViewModel hierarchyViewModel = new NodeViewModel(_metadataReader,
                                                                           _dataReader,
                                                                           _dataWriter,
                                                                           new Key(rootNode.ID, rootNode.Revision));

            InitializeProjects();
            //Hierarchies.Add(hierarchyViewModel);
        }
    }
}
