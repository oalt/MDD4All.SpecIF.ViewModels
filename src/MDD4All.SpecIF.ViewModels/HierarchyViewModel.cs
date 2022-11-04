﻿using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Linq;
using MDD4All.SpecIF.DataModels.Manipulation;
using System.Collections.ObjectModel;
using MDD4All.SpecIF.ViewModels.Cache;
using MDD4All.UI.DataModels.Tree;

namespace MDD4All.SpecIF.ViewModels
{
    public class HierarchyViewModel : ViewModelBase, ITreeNode
    {

        private bool _rootNodesInitialized = false;

        public HierarchyViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter,
                                  Key key)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            _hierarchyNode = _specIfDataReader.GetHierarchyByKey(key);
            _rootNodes.Add(this);
            InitializeReferencedResource(_hierarchyNode.ResourceReference);
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
            _rootNodes.Add(this);

            InitializeReferencedResource(_hierarchyNode.ResourceReference);
        }




        private void InitializeData()
        {

            if (_hierarchyNode != null)
            {
                int counter = 0;
                if (_hierarchyNode.NodeReferences != null)
                {
                    foreach (Key nodeReference in _hierarchyNode.NodeReferences)
                    {
                        Node childNode = _specIfDataReader.GetNodeByKey(nodeReference);

                        HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, childNode);
                        hierarchyViewModel.Index = counter;
                        hierarchyViewModel.Depth = 1;
                        hierarchyViewModel.Parent = this;

                        counter++;
                    }


                }

            }
        }

        private void InitializeReferencedResource(Key key)
        {
            ReferencedResource = CachedViewModelFactory.GetResourceViewModel(_hierarchyNode.ResourceReference,
                                                                             MetadataReader,
                                                                             DataReader,
                                                                             DataWriter);
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

        public Key RootResourceClassKey
        {
            get
            {
                Key result = null;
                if (ReferencedResource != null)
                {
                    result = ReferencedResource.Resource.Class;
                }

                return result;
            }
        }

        public Key HierarchyKey
        {
            get
            {
                Key result = null;

                if (HierarchyNode != null)
                {
                    result = new Key(HierarchyNode.ID, HierarchyNode.Revision);
                }

                return result;
            }
        }

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



        private void InitilizeLinearResourceListRecursively(ObservableCollection<ITreeNode> children)
        {
            foreach (HierarchyViewModel child in children)
            {
                //child.HierarchyKey = HierarchyKey;
                _linearResourceList.Add(child);

                if (child.Children != null)
                {
                    InitilizeLinearResourceListRecursively(child.Children);
                }

            }
        }


        private ObservableCollection<HierarchyViewModel> _rootNodes = new ObservableCollection<HierarchyViewModel>();

        public ObservableCollection<HierarchyViewModel> RootNodes
        {
            get
            {
                if (!_rootNodesInitialized)
                {
                    InitializeData();
                    _rootNodesInitialized = true;
                }
                return _rootNodes;
            }
        }

        private HierarchyEditorViewModel _editorViewModel;

        public HierarchyEditorViewModel EditorViewModel
        {
            get
            {
                HierarchyEditorViewModel result = null;
                if (Parent == null)
                {
                    result = _editorViewModel;
                }
                else
                {
                    ITreeNode node = this;
                    while (node.Parent != null)
                    {
                        node = node.Parent;
                    }
                    result = (node as HierarchyViewModel).EditorViewModel;

                }
                return result;
            }

            set
            {
                _editorViewModel = value;
            }

        }

        private Node _hierarchyNode;

        public Node HierarchyNode
        {
            get
            {
                return _hierarchyNode;
            }
        }

        public string Title
        {
            get
            {
                string result = "";
                if (ReferencedResource != null)
                {
                    result = ReferencedResource.Title;
                }
                return result;
            }
        }

        public string ClassTitle
        {
            get
            {
                string result = "";
                if (ReferencedResource != null)
                {
                    result = ReferencedResource.Type;
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

                ITreeNode item = this;

                while (item.Parent != null)
                {
                    result = (item.Parent.Index + 1) + "." + result;
                    item = item.Parent;
                }

                return result;
            }
        }

        private ITreeNode _parent = null;

        public ITreeNode Parent
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

        private string _nodeID = "";

        public string NodeID
        {
            get
            {
                string result = "";
                if (_hierarchyNode != null)
                {
                    result = _hierarchyNode.ID;
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

        private ObservableCollection<ITreeNode> _children = null;

        public ObservableCollection<ITreeNode> Children
        {
            get
            {
                ObservableCollection<ITreeNode> result = null;

                if (_children == null && _hierarchyNode.Nodes != null)
                {
                    _children = new ObservableCollection<ITreeNode>();
                    int counter = 0;


                    foreach (Node child in _hierarchyNode.Nodes)
                    {
                        HierarchyViewModel resourceViewModel = new HierarchyViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, child);

                        resourceViewModel.Parent = this;
                        resourceViewModel.Index = counter;
                        resourceViewModel.Depth = Depth + 1;
                        counter++;

                        _children.Add(resourceViewModel);
                    }

                }

                result = _children;

                return result;
            }
        }

        public bool IsSelected
        {
            get
            {
                bool result = false;
                if (EditorViewModel.SelectedNode == this)
                {
                    result = true;
                }
                return result;
            }

            set
            {
                
            }
        }

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

        public bool HasChildNodes
        {
            get
            {
                return Children?.Any() ?? false;
            }
        }

        public bool IsLoading
        {
            get
            {
                bool result = ReferencedResource == null;
                return result;
            }

        }


        public bool IsDisabled
        {
            get
            {
                return false;

            }

            set
            { }
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
    }
}
