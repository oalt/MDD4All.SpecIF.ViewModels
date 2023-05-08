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
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MDD4All.SpecIF.ViewModels
{
    public class HierarchyViewModel : ViewModelBase, ITree
    {
        public HierarchyViewModel(ISpecIfDataProviderFactory specIfDataProviderFactory,
                                  Key key)
        {
            _specIfDataProviderFactory = specIfDataProviderFactory;
            _metadataReader = _specIfDataProviderFactory.MetadataReader;
            _specIfDataWriter = _specIfDataProviderFactory.DataWriter;
            _specIfDataReader = _specIfDataProviderFactory.DataReader;

            InitializeCommands();

            Node rootNode = _specIfDataReader.GetHierarchyByKey(key);
            RootNode = new NodeViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, this, rootNode);
            //RootNode.EditorViewModel = this;

            RootNode.IsExpanded = true;

            _treeRootNodes = new ObservableCollection<ITreeNode>();
            _treeRootNodes.Add(RootNode);

            SelectedNode = RootNode;
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

            StartAddStatementCommand = new RelayCommand(ExecuteStartAddNewStatement);
            ConfirmAddStatementCommand = new RelayCommand(ExecuteConfirmAddNewStatement);
            CancelAddStatementCommand = new RelayCommand(ExecuteCancelAddNewStatement);
        }

        public const string EDIT_EXISTING = "Edit existing";
        public const string NEW_CHILD = "New child";
        public const string NEW_BELOW = "New resource below";
        public const string NEW_ABOVE = "New resource above";

        private ISpecIfDataProviderFactory _specIfDataProviderFactory;

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

        private ITreeNode _selectedNode;

        public ITreeNode SelectedNode
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

        private bool _isMultilanguageEnabled = false;

        public bool IsMultilanguageEnabled
        {
            get { return _isMultilanguageEnabled; }
            set
            {
                _isMultilanguageEnabled = value;
                RaisePropertyChanged("IsMultilanguageEnabled");
            }
        }

        private string _primaryLanguage = "en";

        public string PrimaryLanguage
        {
            get
            {
                return _primaryLanguage;
            }
            set
            {
                _primaryLanguage = value;
                RaisePropertyChanged("PrimaryLanguage");
            }
        }

        private string _secondaryLanguage = "de";

        public string SecondaryLanguage
        {
            get
            {
                return _secondaryLanguage;
            }
            set
            {
                _secondaryLanguage = value;
                RaisePropertyChanged("SecondaryLanguage");
            }
        }
    
        public List<string> Languages 
        { 
            get
            {
                List<string> result = new List<string>();

                result.Add("en");
                result.Add("de");
                result.Add("zh");

                return result;
            }
        }

        private CreateStatementViewModel _createStatementViewModel;

        public CreateStatementViewModel CreateStatementViewModel
        {
            get
            {
                return _createStatementViewModel;
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

        public bool ShowAddStatementDialog { get; set; } = false;

        public ObservableCollection<ITreeNode> TreeRootNodes
        {
            get
            {
                return _treeRootNodes;
            }
        }

        //private ITreeNode _selectedTreeNode;

        //public ITreeNode SelectedTreeNode
        //{
        //    get
        //    {
        //        return _selectedTreeNode;
        //    }

        //    set
        //    {
        //        _selectedTreeNode = value;
        //        RaisePropertyChanged("SelectedTreeNode");
        //    }
        //}

        private List<NodeViewModel> _linearResourceList;

        public List<NodeViewModel> LinearResourceList
        {
            get
            {
                _linearResourceList = new List<NodeViewModel>();

                _linearResourceList.Add(RootNode);
                InitilizeLinearResourceListRecursively(RootNode.Children);

                return _linearResourceList;
            }
        }

        private void InitilizeLinearResourceListRecursively(ObservableCollection<ITreeNode> children)
        {
            foreach (NodeViewModel child in children)
            {
                //child.HierarchyKey = HierarchyKey;
                _linearResourceList.Add(child);

                if (child.Children != null)
                {
                    InitilizeLinearResourceListRecursively(child.Children);
                }

            }
        }

        private ObservableCollection<ITreeNode> _treeRootNodes = new ObservableCollection<ITreeNode>();

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

        public ICommand StartAddStatementCommand { get; private set; }

        public ICommand ConfirmAddStatementCommand { get; private set; }

        public ICommand CancelAddStatementCommand { get; private set; }


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

                NodeViewModel selectedElement = SelectedNode as NodeViewModel;

                selectedElement.ReferencedResource = ResourceUnderEdit;

                selectedElement.HierarchyNode.ResourceReference.Revision = ResourceUnderEdit.Resource.Revision;

                _specIfDataWriter.UpdateHierarchy(selectedElement.HierarchyNode);
            }
            else
            {
                _specIfDataWriter.AddResource(ResourceUnderEdit.Resource);

                Node selectedNode = ((NodeViewModel)SelectedNode).HierarchyNode;

                Node newNode = CreateNewNodeForAddition();

                NodeViewModel newTreeNodeViewModel = new NodeViewModel(_metadataReader,
                                                                       _specIfDataReader,
                                                                       _specIfDataWriter,
                                                                       this,
                                                                       newNode);

                //newTreeNodeViewModel.EditorViewModel = this;

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

                        if ((SelectedNode.Index + 1) == ((NodeViewModel)SelectedNode).HierarchyNode.Nodes.Count)
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
                ITreeNode selectedNodeAfterDelete;

                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int index = SelectedNode.Index;

                if(index > 0)
                {
                    selectedNodeAfterDelete = parentViewModel.Children[index - 1];
                }
                else
                {
                    selectedNodeAfterDelete = parentViewModel;
                }

                parentViewModel.Children.RemoveAt(index);
                parentViewModel.HierarchyNode.Nodes.RemoveAt(index);
                _specIfDataWriter.UpdateHierarchy(parentViewModel.HierarchyNode);

                SelectedNode = selectedNodeAfterDelete;
            }


        }

        private void ExecuteCancelDeleteResource()
        {
            ShowDeleteConfirm = false;
        }

        private void ExecuteMoveNodeDown()
        {
            if (SelectedNode != null && SelectedNode.Parent != null)
            {
                Node selectedNode = ((NodeViewModel)SelectedNode).HierarchyNode;

                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int childCount = parentViewModel.Children.Count;

                int currentIndex = SelectedNode.Index;

                if (SelectedNode.Index < childCount - 1)
                {
                    parentViewModel.Children.RemoveAt(currentIndex);
                    parentViewModel.Children.Insert(currentIndex + 1, SelectedNode);

                    NodeViewModel newSibling = parentViewModel.Children[currentIndex] as NodeViewModel;

                    _specIfDataWriter.MoveNode(((NodeViewModel)SelectedNode).HierarchyNode.ID, parentViewModel.HierarchyNode.ID, newSibling.HierarchyNode.ID);

                    parentViewModel.HierarchyNode.Nodes.RemoveAt(currentIndex);
                    parentViewModel.HierarchyNode.Nodes.Insert(currentIndex + 1, selectedNode);

                    StateChanged = true;
                }
            }
        }

        private void ExecuteMoveNodeUp()
        {
            if (SelectedNode != null && SelectedNode.Parent != null)
            {
                Node selectedNode = ((NodeViewModel)SelectedNode).HierarchyNode;

                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int childCount = parentViewModel.Children.Count;

                int currentIndex = SelectedNode.Index;

                if (SelectedNode.Index > 0)
                {
                    parentViewModel.Children.RemoveAt(currentIndex);
                    parentViewModel.Children.Insert(currentIndex - 1, SelectedNode);

                    

                    string newSiblingId = null;

                    if (currentIndex - 1 > 0)
                    {
                        NodeViewModel newSibling = parentViewModel.Children[currentIndex - 2] as NodeViewModel;

                        newSiblingId = newSibling.HierarchyNode.ID;
                    }


                    _specIfDataWriter.MoveNode(((NodeViewModel)SelectedNode).HierarchyNode.ID, parentViewModel.HierarchyNode.ID, newSiblingId);

                    parentViewModel.HierarchyNode.Nodes.RemoveAt(currentIndex);
                    parentViewModel.HierarchyNode.Nodes.Insert(currentIndex - 1, selectedNode);

                    StateChanged = true;
                }
            }
        }

        private void ExecuteNodeOneLevelHigher()
        {
            if (SelectedNode != null && SelectedNode.Parent != null && SelectedNode.Index > 0)
            {
                Node selectedNode = ((NodeViewModel)SelectedNode).HierarchyNode;

                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int currentIndex = SelectedNode.Index;

                NodeViewModel newParent = parentViewModel.Children[currentIndex - 1] as NodeViewModel;

                string siblingID = null;
                int newIndex = 0;
                if (newParent.Children.Count > 0)
                {
                    NodeViewModel newSibling = newParent.Children[newParent.Children.Count - 1] as NodeViewModel;
                    siblingID = newSibling.HierarchyNode.ID;
                    newIndex = newSibling.Index + 1;
                }

                parentViewModel.Children.RemoveAt(currentIndex);
                newParent.Children.Insert(newIndex, SelectedNode);
                SelectedNode.Parent = newParent;

                newParent.IsExpanded = true;

                _specIfDataWriter.MoveNode(((NodeViewModel)SelectedNode).HierarchyNode.ID, newParent.HierarchyNode.ID, siblingID);

                parentViewModel.HierarchyNode.Nodes.RemoveAt(currentIndex);
                newParent.HierarchyNode.Nodes.Insert(newIndex, selectedNode);

                StateChanged = true;
            }
        }

        private void ExecuteNodeOneLevelLower()
        {
            if (SelectedNode != null && SelectedNode.Parent != null && SelectedNode.Parent.Parent != null)
            {
                Node selectedNode = ((NodeViewModel)SelectedNode).HierarchyNode;

                NodeViewModel parentViewModel = SelectedNode.Parent as NodeViewModel;

                int currentIndex = SelectedNode.Index;

                int parentIndex = parentViewModel.Index;

                parentViewModel.Children.RemoveAt(currentIndex);
                parentViewModel.Parent.Children.Insert(parentIndex + 1, SelectedNode);
                SelectedNode.Parent = parentViewModel.Parent;

                _specIfDataWriter.MoveNode(((NodeViewModel)SelectedNode).HierarchyNode.ID, ((NodeViewModel)parentViewModel.Parent).HierarchyNode.ID, parentViewModel.HierarchyNode.ID);

                parentViewModel.HierarchyNode.Nodes.RemoveAt(currentIndex);
                ((NodeViewModel)parentViewModel.Parent).HierarchyNode.Nodes.Insert(parentIndex + 1, selectedNode);

                StateChanged = true;
            }
        }

        private void ExecuteStartAddNewStatement()
        {
            _createStatementViewModel = new CreateStatementViewModel(this, _specIfDataProviderFactory);
            ShowAddStatementDialog = true;
        }

        private void ExecuteConfirmAddNewStatement()
        {
            ShowAddStatementDialog = false;

            if(CreateStatementViewModel.StatementViewModel != null && 
               CreateStatementViewModel.StatementViewModel.Resource != null)
            {
                Statement statement = CreateStatementViewModel.StatementViewModel.Resource as Statement;

                if(statement != null)
                {
                    Task.Run(() =>
                    {
                        _specIfDataWriter.AddStatement(statement);

                        CreateStatementViewModel.SelectedResource.ReinitializeStatementsAsync().Wait();
                        CreateStatementViewModel.OppositeResource.ReinitializeStatementsAsync().Wait();
                    });


                    
                }
            }
        }

        private void ExecuteCancelAddNewStatement()
        {
            ShowAddStatementDialog = false;
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
            if (SelectedNode != null)
            {
                ITreeNode currentNode = SelectedNode.Parent;
                while (currentNode != null)
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
}
