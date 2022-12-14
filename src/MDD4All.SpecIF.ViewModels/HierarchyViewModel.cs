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
using MDD4All.UI.DataModels.Tree;

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

            InitializeCommands();

            Node rootNode = _specIfDataReader.GetHierarchyByKey(key);
            RootNode = new NodeViewModel(metadataReader, dataReader, dataWriter, rootNode);
            RootNode.EditorViewModel = this;

            RootNode.IsExpanded = true;
        }

        private void InitializeCommands()
        {
            StartEditResourceCommand = new RelayCommand<string>(ExecuteStartEditResource);
            ConfirmEditResourceCommand = new RelayCommand(ExecuteConfirmEditResource);
            CancelEditResourceCommand = new RelayCommand(ExecuteCancelEditResource);

            StartDeleteResourceCommand = new RelayCommand(ExecuteStartDeleteResource);
            DeleteResourceCommand = new RelayCommand(ExecuteDeleteResource);
            CancelDeleteResourceCommand = new RelayCommand(ExecuteCancelDeleteResource);

            MoveNodeUpCommand = new RelayCommand(ExecuteMoveNodeUp);
            MoveNodeDownCommand = new RelayCommand(ExecuteMoveNodeDown);
            NodeOneLevelHigherCommand = new RelayCommand(ExecuteNodeOneLevelHigher);
            NodeOneLevelLowerCommand = new RelayCommand(ExecuteNodeOneLevelLower);
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

        public NodeViewModel RootNode { get; set; }

        private NodeViewModel _selectedNode;

        public NodeViewModel SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;
                ExpandSelectedNodeToRoot();
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

        public bool ShowDeleteConfirm { get; set; } = false;

        #region COMMAND_DEFINITIONS

        public ICommand StartEditResourceCommand { get; private set; }

        public ICommand CancelEditResourceCommand { get; private set; }

        public ICommand ConfirmEditResourceCommand { get; private set; }

        public ICommand StartCreateNewResourceCommand { get; private set; }

        public ICommand AddNewResourceAboveCommand { get; private set; }

        public ICommand AddNewResourceAsChildCommand { get; private set; }

        public ICommand AddNewResourceBelowCommand { get; private set; }

        public ICommand StartDeleteResourceCommand { get; private set; }

        public ICommand DeleteResourceCommand { get; private set; }

        public ICommand CancelDeleteResourceCommand { get; private set; }

        public ICommand MoveNodeUpCommand { get; private set; }

        public ICommand MoveNodeDownCommand { get; private set; }

        public ICommand NodeOneLevelHigherCommand { get; private set; }

        public ICommand NodeOneLevelLowerCommand { get; private set; }

        #endregion

        #region COMMAND_IMPLEMENTATIONS

        private void ExecuteStartEditResource(string editType)
        {
            EditType = editType;

            if (editType == EDIT_EXISTING)
            {
                NodeViewModel selectedElement = ((NodeViewModel)SelectedNode);

                Resource clonedResource = selectedElement.ReferencedResource.Resource.CreateNewRevisionForEdit(_metadataReader);
                ResourceUnderEdit = new ResourceViewModel(_metadataReader,
                                                          _specIfDataReader,
                                                          _specIfDataWriter,
                                                          clonedResource);
            }
            else // create new resource
            {
                if (SelectedResourceClassKey != null && ((SelectedNode.Parent != null && (editType == NEW_ABOVE || editType == NEW_BELOW)) || editType == NEW_CHILD))
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

                NodeViewModel selectedElement = SelectedNode;

                selectedElement.ReferencedResource = ResourceUnderEdit;

                selectedElement.HierarchyNode.ResourceReference.Revision = ResourceUnderEdit.Resource.Revision;

                _specIfDataWriter.UpdateHierarchy(selectedElement.HierarchyNode);
            }
            else
            {
                _specIfDataWriter.AddResource(ResourceUnderEdit.Resource);

                Node selectedNode = SelectedNode.HierarchyNode;

                Node newNode = CreateNewNodeForAddition();

                NodeViewModel newTreeNodeViewModel = new NodeViewModel(_metadataReader,
                                                                           _specIfDataReader,
                                                                           _specIfDataWriter,
                                                                           newNode);

                newTreeNodeViewModel.EditorViewModel = this;

                if (EditType == NEW_CHILD)
                {
                    // integrate in view model
                    newTreeNodeViewModel.Parent = SelectedNode;


                    if (selectedNode.Nodes == null)
                    {
                        selectedNode.Nodes = new List<Node>();

                        selectedNode.Nodes.Add(newNode);

                        SelectedNode.Children.Add(newTreeNodeViewModel);
                    }
                    else
                    {
                        selectedNode.Nodes.Insert(0, newNode);
                        SelectedNode.Children.Insert(0, newTreeNodeViewModel);
                    }

                    // add to persistent storage
                    _specIfDataWriter.AddNodeAsFirstChild(selectedNode.ID, newNode);

                    SelectedNode.IsExpanded = true;

                }
                else if (EditType == NEW_BELOW)
                {
                    if (SelectedNode.Parent != null)
                    {
                        newTreeNodeViewModel.Parent = SelectedNode.Parent;

                        NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                        if ((SelectedNode.Index + 1) == SelectedNode.HierarchyNode.Nodes.Count)
                        {
                            parentViewModel.HierarchyNode.Nodes.Add(newNode);

                            parentViewModel.Children.Add(newTreeNodeViewModel);
                        }
                        else
                        {
                            parentViewModel.HierarchyNode.Nodes.Insert(SelectedNode.Index + 1, newNode);

                            parentViewModel.Children.Insert(SelectedNode.Index + 1, newTreeNodeViewModel);
                        }

                        // add to persistent storage
                        _specIfDataWriter.AddNodeAsPredecessor(selectedNode.ID, newNode);
                    }

                }
                else if (EditType == NEW_ABOVE)
                {
                    if (SelectedNode.Parent != null)
                    {
                        newTreeNodeViewModel.Parent = SelectedNode.Parent;

                        NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                        if (SelectedNode.Index == 0) // the selected node is the first
                        {
                            parentViewModel.HierarchyNode.Nodes.Insert(0, newNode);

                            parentViewModel.Children.Insert(0, newTreeNodeViewModel);

                            // add to persistent storage
                            _specIfDataWriter.AddNodeAsFirstChild(parentViewModel.HierarchyNode.ID, newNode);
                        }
                        else // the selected node index > 0
                        {
                            Node predecessorNode = parentViewModel.HierarchyNode.Nodes[SelectedNode.Index - 1];

                            parentViewModel.HierarchyNode.Nodes.Insert(SelectedNode.Index, newNode);

                            parentViewModel.Children.Insert(SelectedNode.Index, newTreeNodeViewModel);

                            // add to persistent storage
                            _specIfDataWriter.AddNodeAsPredecessor(predecessorNode.ID, newNode);
                        }



                    }
                }
            }

            ResourceUnderEdit.IsInEditMode = false;
            ResourceUnderEdit = null;
            EditorActive = false;

            StateChanged = true;
        }

        private void ExecuteCancelEditResource()
        {
            ResourceUnderEdit.IsInEditMode = false;
            ResourceUnderEdit = null;
            EditorActive = false;
        }


        private void ExecuteStartDeleteResource()
        {
            if (SelectedNode.Parent != null)
            {
                ShowDeleteConfirm = true;
            }
        }

        private void ExecuteDeleteResource()
        {
            ShowDeleteConfirm = false;

            if (SelectedNode != null)
            {
                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int index = SelectedNode.Index;

                parentViewModel.Children.RemoveAt(index);
                parentViewModel.HierarchyNode.Nodes.RemoveAt(index);
                _specIfDataWriter.UpdateHierarchy(parentViewModel.HierarchyNode);

                SelectedNode = null;
            }


        }

        private void ExecuteCancelDeleteResource()
        {
            ShowDeleteConfirm = false;
        }

        private void ExecuteMoveNodeDown()
        {
            if(SelectedNode != null && SelectedNode.Parent != null)
            {
                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int childCount = parentViewModel.Children.Count;

                int currentIndex = SelectedNode.Index;

                if(SelectedNode.Index < childCount - 1)
                {
                    parentViewModel.Children.RemoveAt(currentIndex);
                    parentViewModel.Children.Insert(currentIndex + 1, SelectedNode);

                    NodeViewModel newSibling = parentViewModel.Children[currentIndex] as NodeViewModel;

                    _specIfDataWriter.MoveNode(SelectedNode.HierarchyNode.ID, parentViewModel.HierarchyNode.ID, newSibling.HierarchyNode.ID);

                    StateChanged = true;
                }
            }
        }

        private void ExecuteMoveNodeUp()
        {
            if (SelectedNode != null && SelectedNode.Parent != null)
            {
                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int childCount = parentViewModel.Children.Count;

                int currentIndex = SelectedNode.Index;

                if (SelectedNode.Index > 0)
                {
                    parentViewModel.Children.RemoveAt(currentIndex);
                    parentViewModel.Children.Insert(currentIndex - 1, SelectedNode);

                    string newSiblingId = null;

                    if(currentIndex - 1 > 0)
                    {
                        NodeViewModel newSibling = parentViewModel.Children[currentIndex - 2] as NodeViewModel;
                        
                        newSiblingId = newSibling.HierarchyNode.ID;
                    }
                    

                    _specIfDataWriter.MoveNode(SelectedNode.HierarchyNode.ID, parentViewModel.HierarchyNode.ID, newSiblingId);

                    StateChanged = true;
                }
            }
        }

        private void ExecuteNodeOneLevelHigher()
        {
            if (SelectedNode != null && SelectedNode.Parent != null && SelectedNode.Index > 0)
            {
                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int currentIndex = SelectedNode.Index;

                NodeViewModel newParent = parentViewModel.Children[currentIndex - 1] as NodeViewModel;

                string siblingID = null;
                int newIndex = 0;
                if(newParent.Children.Count > 0)
                {
                    NodeViewModel newSibling = newParent.Children[newParent.Children.Count - 1] as NodeViewModel;
                    siblingID = newSibling.HierarchyNode.ID;
                    newIndex = newSibling.Index + 1;
                }

                parentViewModel.Children.RemoveAt(currentIndex);
                newParent.Children.Insert(newIndex, SelectedNode);
                SelectedNode.Parent = newParent;

                newParent.IsExpanded = true;

                _specIfDataWriter.MoveNode(SelectedNode.HierarchyNode.ID, newParent.HierarchyNode.ID, siblingID);

                StateChanged = true;
            }
        }

        private void ExecuteNodeOneLevelLower()
        {
            if (SelectedNode != null && SelectedNode.Parent != null && SelectedNode.Parent.Parent != null)
            {
                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int currentIndex = SelectedNode.Index;

                int parentIndex = parentViewModel.Index;

                parentViewModel.Children.RemoveAt(currentIndex);
                parentViewModel.Parent.Children.Insert(parentIndex + 1, SelectedNode);
                SelectedNode.Parent = parentViewModel.Parent;

                _specIfDataWriter.MoveNode(SelectedNode.HierarchyNode.ID, ((NodeViewModel)parentViewModel.Parent).HierarchyNode.ID, parentViewModel.HierarchyNode.ID);

                StateChanged = true;
            }
        }

        #endregion

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

        private void ExpandSelectedNodeToRoot()
        {
            bool stateChanged = false;

            ITreeNode currentNode = SelectedNode.Parent;
            while(currentNode != null)
            {
                if (currentNode.IsExpanded == false)
                {
                    currentNode.IsExpanded = true;
                    stateChanged = true;
                }
                currentNode = currentNode.Parent;
            }

            //StateChanged = stateChanged;
        }
        
    }
}
