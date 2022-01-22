using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using System.Linq;
using MDD4All.SpecIF.DataModels.Manipulation;
using System.Collections.ObjectModel;
using MDD4All.SpecIF.ViewModels.Tree;
using MDD4All.SpecIF.ViewModels.Models.JsTree;
using System;

namespace MDD4All.SpecIF.ViewModels
{
    public class HierarchyViewModel : ViewModelBase
    {

        public HierarchyViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 Key id)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            _hierarchyId = id;
            InitializeData();
        }

        public HierarchyViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter, Node hierarchy)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            _hierarchyNode = hierarchy;
            _hierarchyId = new Key(hierarchy.ID, hierarchy.Revision);
            InitializeData();
        }

        private void InitializeData()
        {
            RootNodes = new ObservableCollection<ResourceViewModel>();

            if (_hierarchyNode == null)
            {
                _hierarchyNode = _specIfDataReader.GetHierarchyByKey(new Key(_hierarchyId.ID, _hierarchyId.Revision));
            }

            if (_hierarchyNode != null)
            {
                int counter = 0;
                if (_hierarchyNode.NodeReferences != null)
                {
                    foreach (Key nodeReference in _hierarchyNode.NodeReferences)
                    {
                        Node childNode = _specIfDataReader.GetHierarchyByKey(nodeReference);

                        ResourceViewModel resourceViewModel = new ResourceViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, childNode);
                        resourceViewModel.Index = counter;
                        resourceViewModel.Depth = 1;
                        resourceViewModel.HierarchyID = _hierarchyId;
                        RootNodes.Add(resourceViewModel);

                        counter++;
                    }

                    //InitializeBootstrapTreeDataModel();
                }

                _hierarchy = _specIfDataReader.GetResourceByKey(_hierarchyNode.ResourceReference);
            }
        }



        //private void InitializeBootstrapTreeDataModel()
        //{

        //    _bootstrapTreeDataModel = new BootstrapTreeNode()
        //    {
        //        Text = _hierarchy.Title
        //    };

        //    foreach(ResourceViewModel resourceViewModel in RootNodes)
        //    {
        //        InitializeBootstrapTreeNodesRecursively(_bootstrapTreeDataModel, resourceViewModel);
        //    }
        //}

        //private void InitializeBootstrapTreeNodesRecursively(BootstrapTreeNode parentTreeNode, ResourceViewModel parentResource)
        //{
        //    foreach(ResourceViewModel childModel in parentResource.Children)
        //    {
        //        BootstrapTreeNode treeNode = new BootstrapTreeNode()
        //        {
        //            Text = childModel.Title
        //        };

        //        parentTreeNode.Nodes.Add(treeNode);

        //        InitializeBootstrapTreeNodesRecursively(treeNode, childModel);
        //    }
        //}


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

        private Key _hierarchyId;

        public Key HierarchyId
        {
            get
            {
                return _hierarchyId;
            }
        }

        public string Type
        {
            get
            {
                string result = "";
                result = Hierarchy.GetTypeName(_metadataReader);
                return result;
            }
        }

        private List<ResourceViewModel> _linearResourceList;

        public List<ResourceViewModel> LinearResourceList
        {
            get
            {
                _linearResourceList = new List<ResourceViewModel>();

                foreach (ResourceViewModel rootNode in RootNodes)
                {
                    _linearResourceList.Add(rootNode);
                    InitilizeLinearResourceListRecursively(rootNode.Children);
                }

                return _linearResourceList;
            }
        }



        private void InitilizeLinearResourceListRecursively(ObservableCollection<ResourceViewModel> children)
        {
            foreach (ResourceViewModel child in children)
            {
                child.HierarchyID = HierarchyId;
                _linearResourceList.Add(child);

                if (child.Children != null)
                {
                    InitilizeLinearResourceListRecursively(child.Children);
                }

            }
        }


        public ObservableCollection<ResourceViewModel> RootNodes { get; set; }

        private Node _hierarchyNode;

        public Node HierarchyNode
        {
            get
            {
                return _hierarchyNode;
            }
        }

        private Resource _hierarchy;

        public Resource Hierarchy
        {
            get
            {
                return _hierarchy;
            }
        }

        public string Title
        {
            get
            {
                string result = "";
                if (_hierarchy != null)
                {
                    result = _hierarchy.GetTypeName(_metadataReader);
                }
                return result;
            }
        }

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


        private BootstrapTreeNode _bootstrapTreeDataModel;

        public BootstrapTreeNode BootstrapTreeDataModel
        {
            get
            {
                return _bootstrapTreeDataModel;
            }
        }




        public List<JsTreeNode> JsTreeDataModel
        {

            get
            {
                List<JsTreeNode> result = new List<JsTreeNode>();

                foreach (ResourceViewModel rootNode in RootNodes)
                {
                    result.Add(new JsTreeNode()
                    {
                        Parent = "#",
                        ID = rootNode.NodeID,
                        Text = rootNode.Level + " " + rootNode.LongTitle
                    });
                    InitalizeJsTreeDataRecursively(rootNode, result);
                }

                return result;
            }
        }

        private void InitalizeJsTreeDataRecursively(ResourceViewModel parentResource, List<JsTreeNode> result)
        {
            foreach (ResourceViewModel child in parentResource.Children)
            {
                JsTreeNode childNode = new JsTreeNode()
                {
                    Parent = parentResource.NodeID,
                    ID = child.NodeID,
                    Text = child.Level + " " + child.LongTitle
                };

                result.Add(childNode);

                InitalizeJsTreeDataRecursively(child, result);
            }
        }

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
