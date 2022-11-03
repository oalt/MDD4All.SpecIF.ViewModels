using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Linq;
using MDD4All.SpecIF.DataModels.Manipulation;
using System.Collections.ObjectModel;
using System;
using MDD4All.SpecIF.ViewModels.Cache;
using MDD4All.UI.DataModels.Tree;
using MDD4All.SpecIF.DataModels.Helpers;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

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

            HierarchyKey = key;

            _hierarchyNode = _specIfDataReader.GetHierarchyByKey(key);
            _rootNodes.Add(this);
            InitializeReferencedResource(_hierarchyNode.ResourceReference);

            InitializeCommands();
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
            HierarchyKey = new Key(hierarchy.ID, hierarchy.Revision);
            InitializeReferencedResource(_hierarchyNode.ResourceReference);

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            EditResourceCommand = new RelayCommand(ExecuteEditResource);

            AddNewResourceAboveCommand = new RelayCommand<Key>(ExecuteAddNewResourceAbove);
            AddNewResourceAsChildCommand = new RelayCommand<Key>(ExecuteAddResourceAsChild);
        }


        private void InitializeData()
        {
            //_rootNodes = new ObservableCollection<HierarchyViewModel>();

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
                        hierarchyViewModel.HierarchyKey = HierarchyKey;
                        hierarchyViewModel.Parent = this;

                        counter++;
                    }

                    //InitializeBootstrapTreeDataModel();
                }


                //_hierarchy = _specIfDataReader.GetResourceByKey(_hierarchyNode.ResourceReference);
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



        private void InitilizeLinearResourceListRecursively(ObservableCollection<ITreeNode> children)
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

        private ITreeNode _selectedNode = null;

        public ITreeNode SelectedNode
        {
            get
            {
                // get the value always from the root node
                ITreeNode result = null;
                if (Parent == null)
                {
                    result = _selectedNode;
                }
                else
                {
                    ITreeNode node = this;
                    while (node.Parent != null)
                    {
                        node = node.Parent;

                    }
                    result = node.SelectedNode;
                }

                return result;
            }


            set
            {
                if (Parent == null)
                {
                    _selectedNode = value;
                }
                else
                {
                    ITreeNode node = this;
                    while (node.Parent != null)
                    {
                        node = node.Parent;

                    }
                    node.SelectedNode = value;

                }
                RaisePropertyChanged("SelectedNode");
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
                if (SelectedNode == this)
                {
                    result = true;
                }
                return result;
            }

            set { }
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


        //private BootstrapTreeNode _bootstrapTreeDataModel;

        //public BootstrapTreeNode BootstrapTreeDataModel
        //{
        //    get
        //    {
        //        return _bootstrapTreeDataModel;
        //    }
        //}

        private ITreeNode RootNode
        {
            get
            {
                ITreeNode result = this;

                while (result.Parent != null)
                {
                    result = result.Parent;
                }

                return result;
            }
        }

        private bool _editorActive = false;

        public bool EditorActive 
        {
            get
            {
                // get the value always from the root node
                bool result = false;
                if (Parent == null)
                {
                    result = _editorActive;
                }
                else
                {
                    ITreeNode node = this;
                    while (node.Parent != null)
                    {
                        node = node.Parent;

                    }
                    result = ((HierarchyViewModel)node).EditorActive;
                }

                return result;
            }


            set
            {
                if (Parent == null)
                {
                    _editorActive = value;
                }
                else
                {
                    ITreeNode node = this;
                    while (node.Parent != null)
                    {
                        node = node.Parent;

                    }
                    ((HierarchyViewModel)node).EditorActive = value;

                }
                RaisePropertyChanged("EditorActive");
            }
        }

        #region COMMAND_DEFINITIONS

        public ICommand EditResourceCommand { get; private set; }

        public ICommand AddNewResourceAboveCommand { get; private set; }

        public ICommand AddNewResourceAsChildCommand { get; private set; }

        public ICommand AddNewResourceBelowCommand { get; private set; }

        public ICommand DeleteResourceCommand { get; private set; }

        #endregion

        #region COMMAND_IMPLEMENTATIONS

        private void ExecuteEditResource()
        {


            //HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
            //															   _specIfDataReader,
            //															   _specIfDataWriter,
            //															   nodeKey);

            //Node nodeToEdit = hierarchyViewModel.HierarchyNode.GetNodeByKey(nodeKey);

            //Resource.ID = nodeToEdit.ResourceReference.ID;

            //Revision latestRevision = _specIfDataReader.GetLatestResourceRevision(Resource.ID);

            //Revision newRevision = new Revision();

            ////Revision.BranchName = latestRevision.BranchName;
            //Revision.RevsionNumber = latestRevision.RevsionNumber + 1;

            //_specIfDataWriter.AddResource(Resource);

            //nodeToEdit.ResourceReference.Revision = Revision;

            //_specIfDataWriter.UpdateNode(nodeToEdit);
        }

        private void ExecuteAddNewResourceAbove(Key nodeKey)
        {
            //_specIfDataWriter.AddResource(Resource);

            //HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
            //															   _specIfDataReader,
            //															   _specIfDataWriter,
            //															   nodeKey);

            //Node nodeToEdit = hierarchyViewModel.HierarchyNode.GetNodeByKey(nodeKey);

            //Node newNode = CreateNewNodeForAddition();

            //_specIfDataWriter.AddNode(newNode);

            //if (Parent == null)
            //{
            //	hierarchyViewModel.HierarchyNode.Nodes.Insert(Index, newNode);

            //	_specIfDataWriter.UpdateHierarchy(hierarchyViewModel.HierarchyNode);
            //}
            //else
            //{
            //	Node parentNode = Parent.Node;

            //	parentNode.Nodes.Insert(Index, newNode);

            //	_specIfDataWriter.UpdateNode(Parent.Node);
            //}

        }

        private void ExecuteAddResourceAsChild(Key nodeKey)
        {
            //_specIfDataWriter.AddResource(Resource);

            //HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
            //															   _specIfDataReader,
            //															   _specIfDataWriter,
            //															   nodeKey);

            //Node nodeToEdit = hierarchyViewModel.HierarchyNode.GetNodeByKey(nodeKey);

            //Node newNode = CreateNewNodeForAddition();

            //_specIfDataWriter.AddNode(newNode);

            //if (nodeToEdit.Nodes == null)
            //{
            //	nodeToEdit.Nodes = new List<Node>();

            //	nodeToEdit.Nodes.Add(newNode);
            //}
            //else
            //{
            //	nodeToEdit.Nodes.Insert(0, newNode);
            //}

            //_specIfDataWriter.UpdateNode(nodeToEdit);

        }

        private void ExecuteAddResourceBelow(Key nodeKey)
        {
            //_specIfDataWriter.AddResource(Resource);

            //HierarchyViewModel hierarchyViewModel = new HierarchyViewModel(_metadataReader,
            //															   _specIfDataReader,
            //															   _specIfDataWriter,
            //															   nodeKey);

            //Node nodeToEdit = hierarchyViewModel.HierarchyNode.GetNodeByKey(nodeKey);

            //Node newNode = CreateNewNodeForAddition();

            //_specIfDataWriter.AddNode(newNode);

            //ResourceViewModel parentViewModel = Parent;

            //if (parentViewModel == null)
            //{
            //	if (Index + 1 == hierarchyViewModel.HierarchyNode.Nodes.Count)
            //	{
            //		hierarchyViewModel.HierarchyNode.Nodes.Add(newNode);
            //	}
            //	else
            //	{
            //		hierarchyViewModel.HierarchyNode.Nodes.Insert(Index + 1, newNode);
            //	}

            //	_specIfDataWriter.UpdateHierarchy(hierarchyViewModel.HierarchyNode);
            //}
            //else
            //{
            //	Node parentNode = parentViewModel.Node;

            //	if (Index + 1 == parentNode.Nodes.Count)
            //	{
            //		parentNode.Nodes.Add(newNode);
            //	}
            //	else
            //	{
            //		parentNode.Nodes.Insert(Index + 1, newNode);
            //	}

            //	_specIfDataWriter.UpdateNode(parentNode);
            //}

        }

        private Node CreateNewNodeForAddition()
        {
            Node result = new Node()
            {
                ID = SpecIfGuidGenerator.CreateNewSpecIfGUID(),
                Revision = SpecIfGuidGenerator.CreateNewRevsionGUID(),
                ChangedAt = DateTime.Now,
                ResourceReference = new Key()
                {
                    ID = ReferencedResource.Resource.ID,
                    Revision = SpecIfGuidGenerator.CreateNewRevsionGUID()
                }

            };

            return result;
        }

        private void ExecuteDeleteResource()
        {

        }

        #endregion


        //#region COMMAND_IMPLEMENTATIONS

        //public void DeleteNode(string nodeID)
        //{
        //    if (CanDelete(nodeID) == false)
        //    {

        //    }
        //    else
        //    {
        //        Node nodeToDelete = HierarchyNode.Nodes?.FirstOrDefault(n => n.ID == nodeID);

        //        if (nodeToDelete != null)
        //        {
        //            HierarchyNode.Nodes.Remove(nodeToDelete);
        //            _specIfDataWriter.UpdateHierarchy(HierarchyNode);
        //        }
        //        else
        //        {

        //            foreach (Node node in HierarchyNode.Nodes)
        //            {
        //                if (node != null)
        //                {
        //                    DeleteNodeRecursively(node, nodeID);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void DeleteNodeRecursively(Node parent, string nodeID)
        //{
        //    throw new NotImplementedException();
        //    //Node nodeToDelete = parent?.Nodes?.FirstOrDefault(n => n.ID == nodeID);

        //    //if (nodeToDelete != null)
        //    //{
        //    //	parent.Nodes.Remove(nodeToDelete);
        //    //	_specIfDataWriter.UpdateNode(parent);
        //    //}
        //    //else
        //    //{
        //    //	if (parent.Nodes != null)
        //    //	{
        //    //		foreach (Node child in parent.Nodes)
        //    //		{
        //    //			if (child != null)
        //    //			{
        //    //				DeleteNodeRecursively(child, nodeID);
        //    //			}
        //    //		}
        //    //	}

        //    //}
        //}
        //#endregion
    }
}
