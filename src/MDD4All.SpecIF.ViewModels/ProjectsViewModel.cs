using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;
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


            InitializeProjects();
            InitializeCommands();
        }

        private void InitializeProjects()
        {
            Projects = new List<ProjectViewModel>();

            List<ProjectDescriptor> projectDescriptors = _dataReader.GetProjectDescriptions();

            foreach (ProjectDescriptor projectDescriptor in projectDescriptors)
            {
                ProjectViewModel projectViewModel = new ProjectViewModel(projectDescriptor,
                                                                         _metadataReader,
                                                                         _dataWriter,
                                                                         _dataReader);

                Projects.Add(projectViewModel);
                Projects.Sort((x, y) => string.Compare(x.ProjectTitle, y.ProjectTitle, StringComparison.OrdinalIgnoreCase));
            }
        }
        private void InitializeCommands()
        {
            AddNewProjectCommand = new RelayCommand<List<string>>(ExecuteAddNewProject);
            EditProjectCommand = new RelayCommand<List<string>>(ExecuteEditProject);
        }
        public ICommand AddNewProjectCommand { get; private set; }
        public ICommand EditProjectCommand { get; private set; }
        public List<ProjectViewModel> Projects { get; private set; } = new List<ProjectViewModel>();

        private void ExecuteAddNewProject(List<string> parameters)
        {
            if (parameters != null && parameters.Count == 2)
            {
                ISpecIfMetadataWriter _metadataWriter = _dataWriter as ISpecIfMetadataWriter;
                Guid newGuid = Guid.NewGuid();

                SpecIF.DataModels.SpecIF newProject = new DataModels.SpecIF();

                MultilanguageText newTitle = new MultilanguageText(parameters[0]);
                MultilanguageText newDescription = new MultilanguageText(parameters[1]);
                string newProjectID = "PRJ-" + parameters[0].Replace(" ", "").Substring(0, 4).ToUpper() + "-" + newGuid.ToString().Substring(0, 4).ToUpper();

                newProject.Title = new List<MultilanguageText> { newTitle };
                newProject.Description = new List<MultilanguageText> { newDescription };
                newProject.ID = newProjectID;

                ProjectViewModel newProjectViewModel = new ProjectViewModel(newProject,
                                                                            _metadataReader,
                                                                            _dataWriter,
                                                                            _dataReader);

                _dataWriter.AddProject(_metadataWriter, newProject, newProjectID);

                Projects.Add(newProjectViewModel);
                Projects.Sort((x, y) => string.Compare(x.ProjectTitle, y.ProjectTitle, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new System.ArgumentException("Invalid parameters for adding a new project. Expected two parameters: title and description.");
            }
        }
        private void ExecuteEditProject(List<string> parameters)
        {
            if (string.IsNullOrEmpty(parameters[2]) || parameters == null || parameters.Count != 3)
            {
                throw new ArgumentException("Invalid parameters for editing a project.");
            }
            ISpecIfMetadataWriter _metadataWriter = _dataWriter as ISpecIfMetadataWriter;
            SpecIF.DataModels.SpecIF editedProject = new DataModels.SpecIF();
            ProjectDescriptor projectDescriptor = new ProjectDescriptor
            {
                ID = parameters[2],
                Title = new List<MultilanguageText> { new MultilanguageText(parameters[0]) },
                Description = new List<MultilanguageText> { new MultilanguageText(parameters[1]) }
            };
            ProjectViewModel editedProjectViewModel = new ProjectViewModel(projectDescriptor,
                                                                         _metadataReader,
                                                                         _dataWriter,
                                                                         _dataReader);
            foreach (ProjectViewModel projectViewModel in Projects)
            {
                if (projectViewModel.ProjectID == parameters[2])
                {
                    editedProjectViewModel = projectViewModel;
                    editedProjectViewModel.SetProjectDescriptor(projectDescriptor);
                    Projects.Remove(projectViewModel);
                    editedProject.Title = new List<MultilanguageText> { new MultilanguageText(parameters[0]) };
                    editedProject.Description = new List<MultilanguageText> { new MultilanguageText(parameters[1]) };
                    editedProject.ID = parameters[2];
                    break;
                }
            }
            _dataWriter.AddProject(_metadataWriter, editedProject, parameters[2]);
            Projects.Add(editedProjectViewModel);
            Projects.Sort((x, y) => string.Compare(x.ProjectTitle, y.ProjectTitle, StringComparison.OrdinalIgnoreCase));
        }
    }
}
