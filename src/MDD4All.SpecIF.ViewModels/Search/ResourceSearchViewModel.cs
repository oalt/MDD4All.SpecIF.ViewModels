using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels.Search
{
    public class ResourceSearchViewModel : ViewModelBase
    {
        public ResourceSearchViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RunSearchCommand = new RelayCommand(ExecuteRunSearchCommand);
        }

        public HierarchyViewModel CurrentHierarchy { get; set; }

        public ResourceViewModel SelectedResource { get; set; } = null;

        public string SerachTerm { get; set; } = "";

        public List<ResourceViewModel> SearchResults { get; private set; } = new List<ResourceViewModel>();

        public ICommand RunSearchCommand { get; private set; }

        private void ExecuteRunSearchCommand()
        {
            SearchResults = new List<ResourceViewModel>();

            if(CurrentHierarchy != null)
            {
                List<NodeViewModel> resourceList = CurrentHierarchy.LinearResourceList;

                foreach (NodeViewModel node in resourceList)
                {
                    if(node.ReferencedResource != null)
                    {
                        if(string.IsNullOrEmpty(SerachTerm))
                        {
                            SearchResults.Add(node.ReferencedResource);
                        }
                        else
                        {
                            Resource resource = node.ReferencedResource.Resource;

                            bool match = false;
                            if(resource != null)
                            {
                                if(resource.Properties != null)
                                {
                                    foreach(Property property in resource.Properties)
                                    {
                                        foreach(Value value in property.Values)
                                        {
                                            if(!string.IsNullOrEmpty(value.StringValue))
                                            {
                                                
                                                if (value.StringValue.IndexOf(SerachTerm, StringComparison.InvariantCultureIgnoreCase) >= 0)
                                                {
                                                    SearchResults.Add(node.ReferencedResource);
                                                    match = true;
                                                    break;
                                                }

                                            }
                                            else
                                            {
                                                MultilanguageText multilanguageText = value.MultilanguageTexts.Find(text => text.Text.IndexOf(SerachTerm, StringComparison.InvariantCultureIgnoreCase) >= 0);
                                                if(multilanguageText != null)
                                                {
                                                    SearchResults.Add(node.ReferencedResource);
                                                    match = true;
                                                    break;
                                                }
                                            }
                                            
                                        }
                                        if (match)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }


    }
}
