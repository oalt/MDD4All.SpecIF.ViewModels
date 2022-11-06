using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels.Helpers;
using System;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels
{
    public class HierarchyEditorViewModel : ViewModelBase
    {
        public HierarchyEditorViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter,
                                  Key key)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            InitializeCommands();

            Node rootNode = _specIfDataReader.GetHierarchyByKey(key);
            RootNode = new HierarchyViewModel(metadataReader, dataReader, dataWriter, rootNode);
            RootNode.EditorViewModel = this;

            RootNode.IsExpanded = true;
        }

        private void InitializeCommands()
        {
            StartEditResourceCommand = new RelayCommand<string>(ExecuteStartEditResource);
            ConfirmEditResourceCommand = new RelayCommand(ExecuteConfirmEditResource);
            CancelEditResourceCommand = new RelayCommand(ExecuteCancelEditResource);

            AddNewResourceAboveCommand = new RelayCommand<Key>(ExecuteAddNewResourceAbove);
            AddNewResourceAsChildCommand = new RelayCommand<Key>(ExecuteAddResourceAsChild);


        }

        public const string EDIT_EXISTING = "Edit existing";
        public const string NEW_CHILD = "New child";
        public const string NEW_BELOW = "New resource below";
        public const string NEW_ABOVE = "New resource above";

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

        public HierarchyViewModel RootNode { get; set; }

        private HierarchyViewModel _selectedNode;

        public HierarchyViewModel SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;
                RaisePropertyChanged("SelectedNode");
                RaisePropertyChanged("StateChanged");
            }
        }

        private bool _editorActive = false;

        public bool EditorActive
        {
            get
            {
                // get the value always from the root node
                bool result = _editorActive;


                return result;
            }


            set
            {
                _editorActive = value;
                RaisePropertyChanged("EditorActive");
            }
        }

        private ResourceViewModel _resourceUnderEdit;

        public ResourceViewModel ResourceUnderEdit
        {
            get
            {
                return _resourceUnderEdit;
            }
            set
            {
                _resourceUnderEdit = value;
            }
        }

        private string _editType;

        public string EditType
        {
            get { return _editType; }
            set { _editType = value; }
        }

        public string EditDialogTitleKey
        {
            get
            {
                string result = "Title.EditResource";

                switch (EditType)
                {
                    case EDIT_EXISTING:
                        result = "Title.EditResource";
                        break;

                    case NEW_CHILD:
                        result = "Title.NewResource";
                        break;

                    case NEW_BELOW:
                        result = "Title.NewResource";
                        break;

                    case NEW_ABOVE:
                        result = "Title.NewResource";
                        break;
                }

                return result;
            }
        }


        public bool StateChanged
        {
            set
            {
                if (value)
                {
                    RaisePropertyChanged("StateChanged");
                }
            }
        }

        public Key SelectedResourceClassKey 
        { 
            get; 
            
            set; 
        
        }

        #region COMMAND_DEFINITIONS

        public ICommand StartEditResourceCommand { get; private set; }

        public ICommand CancelEditResourceCommand { get; private set; }

        public ICommand ConfirmEditResourceCommand { get; private set; }

        public ICommand StartCreateNewResourceCommand { get; private set; }

        public ICommand AddNewResourceAboveCommand { get; private set; }

        public ICommand AddNewResourceAsChildCommand { get; private set; }

        public ICommand AddNewResourceBelowCommand { get; private set; }

        public ICommand DeleteResourceCommand { get; private set; }

        #endregion

        #region COMMAND_IMPLEMENTATIONS

        private void ExecuteStartEditResource(string editType)
        {
            EditType = editType;

            if (editType == EDIT_EXISTING)
            {
                HierarchyViewModel selectedElement = ((HierarchyViewModel)SelectedNode);

                Resource clonedResource = selectedElement.ReferencedResource.Resource.CreateNewRevisionForEdit(_metadataReader);
                ResourceUnderEdit = new ResourceViewModel(_metadataReader,
                                                          _specIfDataReader,
                                                          _specIfDataWriter,
                                                          clonedResource);
            }
            else // create new resource
            {
                if (SelectedResourceClassKey != null)
                {
                    Resource newResource = SpecIfDataFactory.CreateResource(SelectedResourceClassKey, _metadataReader);
                    ResourceUnderEdit = new ResourceViewModel(_metadataReader,
                                                              _specIfDataReader,
                                                              _specIfDataWriter,
                                                              newResource);
                }

            }

            if (ResourceUnderEdit != null)
            {
                ResourceUnderEdit.IsInEditMode = true;

                EditorActive = true;
            }
        }

        private void ExecuteConfirmEditResource()
        {
            if (EditType == EDIT_EXISTING)
            {
                _specIfDataWriter.AddResource(ResourceUnderEdit.Resource);

                HierarchyViewModel selectedElement = SelectedNode;

                selectedElement.ReferencedResource = ResourceUnderEdit;

                selectedElement.HierarchyNode.ResourceReference.Revision = ResourceUnderEdit.Resource.Revision;

                _specIfDataWriter.UpdateHierarchy(selectedElement.HierarchyNode);
            }
            else if (EditType == NEW_CHILD)
            {
                _specIfDataWriter.AddResource(ResourceUnderEdit.Resource);

                Node nodeToEdit = SelectedNode.HierarchyNode; 

                Node newNode = CreateNewNodeForAddition();

                HierarchyViewModel childViewModel = new HierarchyViewModel(_metadataReader,
                                                                           _specIfDataReader,
                                                                           _specIfDataWriter,
                                                                           newNode);
                childViewModel.Parent = SelectedNode;
                childViewModel.EditorViewModel = this;

                _specIfDataWriter.AddNodeAsFirstChild(nodeToEdit.ID, newNode);

                if (nodeToEdit.Nodes == null)
                {
                    nodeToEdit.Nodes = new List<Node>();

                    nodeToEdit.Nodes.Add(newNode);

                    SelectedNode.Children.Add(childViewModel);
                }
                else
                {
                    nodeToEdit.Nodes.Insert(0, newNode);
                    SelectedNode.Children.Insert(0, childViewModel);
                }
                SelectedNode.IsExpanded = true;

            }
            else if (EditType == NEW_BELOW)
            {

            }
            else if (EditType == NEW_ABOVE)
            {

            }

            ResourceUnderEdit = null;
            EditorActive = false;

            StateChanged = true;
        }

        private void ExecuteCancelEditResource()
        {
            ResourceUnderEdit = null;
            EditorActive = false;
        }

        private void ExecuteStartCreateNewResource()
        {

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
                IsHierarchyRoot = false,

            };

            Key resourceKey = new Key(ResourceUnderEdit.Resource.ID, ResourceUnderEdit.Resource.Revision);

            result.ResourceReference = resourceKey;

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
