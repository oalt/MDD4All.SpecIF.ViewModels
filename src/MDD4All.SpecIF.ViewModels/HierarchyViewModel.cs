using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Linq;
using MDD4All.SpecIF.DataModels.Manipulation;
using System.Collections.ObjectModel;
using System;
using MDD4All.SpecIF.ViewModels.Cache;

namespace MDD4All.SpecIF.ViewModels
{
    public class HierarchyViewModel : ViewModelBase
    {

        public HierarchyViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter,
                                  Key key)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            HierarchyKey = key;
            InitializeData();
        }

        public HierarchyViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter, 
                                  Node hierarchy)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            _hierarchyNode = hierarchy;
            HierarchyKey = new Key(hierarchy.ID, hierarchy.Revision);
            InitializeData();
        }

        private void InitializeData()
        {
            RootNodes = new ObservableCollection<HierarchyViewModel>();

            if (_hierarchyNode == null)
            {
                _hierarchyNode = _specIfDataReader.GetHierarchyByKey(new Key(HierarchyKey.ID, HierarchyKey.Revision));
            }

            if (_hierarchyNode != null)
            {
                int counter = 0;
                if (_hierarchyNode.NodeReferences != null)
                {
                    foreach (Key nodeReference in _hierarchyNode.NodeReferences)
                    {
                        Node childNode = _specIfDataReader.GetHierarchyByKey(nodeReference);

                        HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, childNode);
                        hierarchyViewModel.Index = counter;
                        hierarchyViewModel.Depth = 1;
                        hierarchyViewModel.HierarchyKey = HierarchyKey;
                        RootNodes.Add(hierarchyViewModel);

                        counter++;
                    }

                    //InitializeBootstrapTreeDataModel();
                }

                ReferencedResource = CachedViewModelFactory.GetResourceViewModel(_hierarchyNode.ResourceReference,
                                                                                 MetadataReader,
                                                                                 DataReader,
                                                                                 DataWriter);
                //_hierarchy = _specIfDataReader.GetResourceByKey(_hierarchyNode.ResourceReference);
            }
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

        private string _editType = "edit";

        public string EditType
        {
            get { return _editType; }
            set { _editType = value; }
        }



        public Key HierarchyKey { get; set; }

        public ResourceViewModel ReferencedResource { get; set; }


        public string Type
        {
            get
            {
                string result = "";
                result = ReferencedResource.Resource.GetTypeName(_metadataReader);
                return result;
            }
        }

        private List<HierarchyViewModel> _linearResourceList;

        public List<HierarchyViewModel> LinearResourceList
        {
            get
            {
                _linearResourceList = new List<HierarchyViewModel>();

                foreach (HierarchyViewModel rootNode in RootNodes)
                {
                    _linearResourceList.Add(rootNode);
                    InitilizeLinearResourceListRecursively(rootNode.Children);
                }

                return _linearResourceList;
            }
        }



        private void InitilizeLinearResourceListRecursively(ObservableCollection<HierarchyViewModel> children)
        {
            foreach (HierarchyViewModel child in children)
            {
                child.HierarchyKey = HierarchyKey;
                _linearResourceList.Add(child);

                if (child.Children != null)
                {
                    InitilizeLinearResourceListRecursively(child.Children);
                }

            }
        }


        public ObservableCollection<HierarchyViewModel> RootNodes { get; set; }

        private Node _hierarchyNode;

        public Node HierarchyNode
        {
            get
            {
                return _hierarchyNode;
            }
        }

        //private Resource _hierarchy;

        //public Resource Hierarchy
        //{
        //    get
        //    {
        //        return _hierarchy;
        //    }
        //}

        public string Title
        {
            get
            {
                string result = "";
                if (ReferencedResource != null)
                {
                    result = ReferencedResource.Resource.GetTypeName(_metadataReader);
                }
                return result;
            }
        }

        private int _index = 0;

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
            }
        }

        public string Level
        {
            get
            {
                string result = "";

                result = "" + (Index + 1);

                HierarchyViewModel item = this;

                while (item.Parent != null)
                {
                    result = (item.Parent.Index + 1) + "." + result;
                    item = item.Parent;
                }

                return result;
            }
        }

        private HierarchyViewModel _parent = null;

        public HierarchyViewModel Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                _parent = value;
            }
        }

        private Node _node;

        public Node Node
        {
            get { return _node; }
        }

        private string _nodeID = "";

        public string NodeID
        {
            get
            {
                string result = "";
                if (_node != null)
                {
                    result = _node.ID;
                }
                else
                {
                    result = _nodeID;
                }
                return result;
            }

            set
            {
                _nodeID = value;
            }
        }

        public int Depth { get; set; }

        public ObservableCollection<HierarchyViewModel> Children
        {
            get
            {
                ObservableCollection<HierarchyViewModel> result = new ObservableCollection<HierarchyViewModel>();

                int counter = 0;
                if (_node.Nodes != null)
                {
                    foreach (Node child in _node.Nodes)
                    {
                        HierarchyViewModel resourceViewModel = new HierarchyViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, child);

                        resourceViewModel.Parent = this;
                        resourceViewModel.Index = counter;
                        resourceViewModel.Depth = Depth + 1;
                        counter++;

                        result.Add(resourceViewModel);
                    }
                }

                return result;
            }
        }

        public bool IsSelected { get; set; } = false;

        public bool IsExpanded { get; set; } = false;

        private static List<ResourceClass> _resourceTypes = null;

        public List<ResourceClass> ResourceTypes
        {
            get
            {
                if (_resourceTypes == null)
                {
                    _resourceTypes = _metadataReader.GetAllResourceClasses();
                }
                return _resourceTypes;
            }
        }

        public bool CanDelete(string nodeID)
        {
            bool result = true;

            if (HierarchyNode.Nodes.Count() == 1 && HierarchyNode.Nodes[0].ID == nodeID)
            {
                result = false;
            }

            return result;
        }


        //private BootstrapTreeNode _bootstrapTreeDataModel;

        //public BootstrapTreeNode BootstrapTreeDataModel
        //{
        //    get
        //    {
        //        return _bootstrapTreeDataModel;
        //    }
        //}


        #region COMMAND_IMPLEMENTATIONS

        public void DeleteNode(string nodeID)
        {
            if (CanDelete(nodeID) == false)
            {

            }
            else
            {
                Node nodeToDelete = HierarchyNode.Nodes?.FirstOrDefault(n => n.ID == nodeID);

                if (nodeToDelete != null)
                {
                    HierarchyNode.Nodes.Remove(nodeToDelete);
                    _specIfDataWriter.UpdateHierarchy(HierarchyNode);
                }
                else
                {

                    foreach (Node node in HierarchyNode.Nodes)
                    {
                        if (node != null)
                        {
                            DeleteNodeRecursively(node, nodeID);
                        }
                    }
                }
            }
        }

        private void DeleteNodeRecursively(Node parent, string nodeID)
        {
            throw new NotImplementedException();
            //Node nodeToDelete = parent?.Nodes?.FirstOrDefault(n => n.ID == nodeID);

            //if (nodeToDelete != null)
            //{
            //	parent.Nodes.Remove(nodeToDelete);
            //	_specIfDataWriter.UpdateNode(parent);
            //}
            //else
            //{
            //	if (parent.Nodes != null)
            //	{
            //		foreach (Node child in parent.Nodes)
            //		{
            //			if (child != null)
            //			{
            //				DeleteNodeRecursively(child, nodeID);
            //			}
            //		}
            //	}

            //}
        }
        #endregion
    }
}
