using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Linq;
using Vis = VisNetwork.Blazor.Models;
using MDD4All.SpecIF.DataModels.Manipulation;

namespace MDD4All.SpecIF.ViewModels.Revisioning
{
    public abstract class RevisionViewModel : ViewModelBase
    {
        protected ISpecIfMetadataReader _metadataReader;
        protected ISpecIfDataReader _dataReader;
        protected ISpecIfDataWriter _dataWriter;

        public RevisionViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 Key currentRevision)
        {
            _metadataReader = metadataReader;
            _dataReader = dataReader;
            _dataWriter = dataWriter;
            CurrentRevision = currentRevision;

            InitializeResourceViewModels();
            GenerateRevisionVisGraph();
        }

        protected abstract void InitializeResourceViewModels();

        public List<ResourceViewModel> ResourceViewModels { get; set; }

        public Key CurrentRevision { get; set; }

        public ResourceViewModel GetSelectedResourceRevision(string revision)
        {
            ResourceViewModel result = null;

            result = ResourceViewModels.Find(resource => resource.Revision == revision);

            return result;
        }

        public List<PropertyDiffViewModel> GetPropertyDiffs(string revisionTwo)
        {
            List<PropertyDiffViewModel> result = new List<PropertyDiffViewModel>();

            string revisionOne = CurrentRevision.Revision;

            ResourceViewModel resourceOne = ResourceViewModels.Find(resource => resource.Revision == revisionOne);
            ResourceViewModel resourceTwo = ResourceViewModels.Find(resource => resource.Revision == revisionTwo);

            if(resourceOne != null && resourceTwo != null)
            {
                foreach(PropertyViewModel propertyViewModelOne in resourceOne.Properties)
                {
                    PropertyDiffViewModel propertyDiffViewModel = new PropertyDiffViewModel();
                    propertyDiffViewModel.Title = propertyViewModelOne.Title;
                    propertyDiffViewModel.PropertyRevisionOne = propertyViewModelOne;

                    PropertyViewModel propertyViewModelTwo = resourceTwo.Properties.Find(vmTwo => vmTwo.PropertyClassKey.Equals(propertyViewModelOne.PropertyClassKey));
                    if(propertyViewModelTwo == null)
                    {
                        result.Add(propertyDiffViewModel);
                    }
                    else
                    {
                        if(propertyViewModelOne.Property.IsDifferentTo(propertyViewModelTwo.Property))
                        {
                            propertyDiffViewModel.PropertyRevisionTwo = propertyViewModelTwo;
                            result.Add(propertyDiffViewModel);
                        }
                    }
                }
            }

            return result;
        }

        public Vis.NetworkData RevisionGraph { get; set; } = new Vis.NetworkData();

        public Vis.NetworkOptions NetworkOptions
        {
            get
            {
                return new Vis.NetworkOptions
                {
                    AutoResize = true,
                    Height = "100px",
                    Width = "100%",
                    Nodes = new Vis.NodeOption
                    {
                        BorderWidth = 1,
                        Color = new Vis.NodeColorType
                        {
                            Background = "#AAAAAA"
                        }
                    },
                    Layout = new Vis.LayoutOptions
                    {
                        Hierarchical = new Vis.HierarchicalOptions
                        {
                            Enabled = true,
                            Direction = "LR"
                        }
                    },
                    Physics = new Vis.PhysicsOptions
                    {
                        Enabled = false
                    },
                    Interaction = new Vis.InteractionOptions
                    {
                        Multiselect = true
                    }
                };
            }
        }

        protected void GenerateRevisionVisGraph()
        {
            RevisionGraph = new Vis.NetworkData();

            if (ResourceViewModels != null && ResourceViewModels.Any())
            {
                Dictionary<string, Vis.Node> nodeDictionary = new Dictionary<string, Vis.Node>();

                List<Vis.Node> nodes = new List<Vis.Node>();

                List<ResourceViewModel> sortedList = ResourceViewModels.OrderBy(x => x.Resource.ChangedAt).ToList();



                // Add the nodes
                int level = 1;
                foreach (ResourceViewModel resourceViewModel in sortedList)
                {
                    Vis.Node node = new Vis.Node();

                    node.Id = resourceViewModel.Resource.Revision;
                    node.Label = resourceViewModel.Resource.ChangedAt.ToShortDateString() + "\n"
                                 + resourceViewModel.Resource.ChangedAt.ToString("HH:mm:ss");
                    node.Shape = "dot";
                    node.Level = level;

                    if (resourceViewModel.Resource.Revision == CurrentRevision.Revision)
                    {
                        node.Shape = "star";
                        node.Color = new Vis.NodeColorType()
                        {
                            Background = "#5CB400"
                        };
                    }

                    if (resourceViewModel.Resource.Replaces == null || resourceViewModel.Resource.Replaces.Count == 0)
                    {
                        node.Shape = "diamond";
                    }

                    nodeDictionary.Add(resourceViewModel.Resource.Revision, node);
                    nodes.Insert(0, node);

                    level++;
                }

                RevisionGraph.Nodes = nodes;

                // Add the edges
                List<Vis.Edge> edges = new List<Vis.Edge>();

                foreach (ResourceViewModel resourceViewModel in sortedList)
                {
                    Resource resource = resourceViewModel.Resource;
                    if (resource.Replaces != null)
                    {
                        foreach (string replacement in resource.Replaces)
                        {
                            if (nodeDictionary.ContainsKey(replacement))
                            {
                                Vis.Edge edge = new Vis.Edge(replacement, resource.Revision);
                                edges.Add(edge);
                            }
                        }
                    }
                }

                RevisionGraph.Edges = edges;

            }
        }


    }
}
