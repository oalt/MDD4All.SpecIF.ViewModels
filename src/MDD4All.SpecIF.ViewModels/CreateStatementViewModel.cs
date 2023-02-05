using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Metadata;
using MDD4All.SpecIF.ViewModels.Models;
using MDD4All.SpecIF.ViewModels.Search;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class CreateStatementViewModel : ViewModelBase
    {
        private HierarchyViewModel _hierarchyViewModel;

        private ISpecIfDataProviderFactory _dataProviderFactory;

        public CreateStatementViewModel(HierarchyViewModel hierarchyViewModel, ISpecIfDataProviderFactory dataProviderFactory)
        {
            _hierarchyViewModel = hierarchyViewModel;
            _dataProviderFactory = dataProviderFactory;

            SearchDataContext = new ResourceSearchViewModel();
            SearchDataContext.CurrentHierarchy = _hierarchyViewModel;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            StartBrowseForOppositeResourceCommand = new RelayCommand(ExecuteStartBrowseForOppositeResource);
            EndBrowseForOppositeResourceCommand = new RelayCommand(ExecuteEndBrowseForOppositeResource);
        }

        public bool ShowSearchDialog { get; set; } = false;

        public ResourceSearchViewModel SearchDataContext { get; set; }

        public ResourceViewModel SelectedResource
        {
            get
            {
                return ((NodeViewModel)_hierarchyViewModel.SelectedNode).ReferencedResource;
            }
        }

        public ResourceViewModel OppositeResource { get; set; } = null;

        public StatementDirection DesiredDirection { get; set; } = StatementDirection.Out;

        private Key _selectedStatementClass = null;

        public Key SelectedStatementClassKey
        {
            get
            {
                return _selectedStatementClass;
            }
            set
            {
                if(value == null)
                {
                    _selectedStatementClass = null;
                }
                else if(!value.Equals(_selectedStatementClass))
                {
                    _selectedStatementClass = value;
                    InitializeStatementData();
                }
            }
        }

        public StatementClassViewModel StatementClassViewModel { get; set; }

        public StatementViewModel StatementViewModel { get; set; }

        private void InitializeStatementData()
        {
            if (_selectedStatementClass != null)
            {
                StatementClassViewModel = new StatementClassViewModel(_dataProviderFactory.MetadataReader,
                                                                      _dataProviderFactory.MetadataWriter,
                                                                      SelectedStatementClassKey);

                Key subjectKey;
                Key objectKey;

                if(DesiredDirection == StatementDirection.Out)
                {
                    subjectKey = SelectedResource.Key;
                    objectKey = OppositeResource.Key;
                }
                else
                {
                    objectKey = SelectedResource.Key;
                    subjectKey = OppositeResource.Key;
                }

                Statement statement = SpecIfDataFactory.CreateStatement(SelectedStatementClassKey,
                                                                        subjectKey,
                                                                        objectKey,
                                                                        _dataProviderFactory.MetadataReader);

                StatementViewModel = new StatementViewModel(_dataProviderFactory.MetadataReader,
                                                            _dataProviderFactory.DataReader,
                                                            _dataProviderFactory.DataWriter,
                                                            statement);
                StatementViewModel.IsInEditMode = true;

                CanConfirmEditOperation = true;
            }
            else
            {
                StatementClassViewModel = null;
                StatementViewModel = null;
            }
        }

        public bool CanConfirmEditOperation
        {
            get
            {
                bool result = false;

                if(StatementClassViewModel != null)
                {
                    result = true;
                }

                return result;
            }

            set 
            {
                
            }
        }

        #region COMMAND_DEFINITIONS

        public ICommand StartBrowseForOppositeResourceCommand { get; private set; }

        public ICommand EndBrowseForOppositeResourceCommand { get; private set; }
        #endregion

        private void ExecuteStartBrowseForOppositeResource()
        {
            OppositeResource = null;
            SelectedStatementClassKey = null;
            StatementClassViewModel = null;
            SearchDataContext = new ResourceSearchViewModel();
            SearchDataContext.CurrentHierarchy = _hierarchyViewModel;

            ShowSearchDialog = true;

        }

        private void ExecuteEndBrowseForOppositeResource()
        {
            ShowSearchDialog= false;
            OppositeResource = SearchDataContext.SelectedResource;
        }
    }
}
