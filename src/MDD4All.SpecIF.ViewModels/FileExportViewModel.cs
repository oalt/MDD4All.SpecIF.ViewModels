using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.FileAccess.Contracts;
using MDD4All.SpecIF.Converters;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Models;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Input;
using MDD4All.SpecIF.DataModels.Manipulation;

namespace MDD4All.SpecIF.ViewModels
{
    public class FileExportViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _specIfMetadataReader;
        private ISpecIfDataReader _specIfDataReader;
        private IFileSaver _fileSaver;

        private NodeViewModel _rootNodeViewModel;

        public FileExportViewModel(string keyString,
                                   ISpecIfDataProviderFactory dataProviderFactory,
                                   IFileSaver fileSaver)
        {
            HierarchyKey = new Key();
            HierarchyKey.InitailizeFromKeyString(keyString);
            _specIfMetadataReader = dataProviderFactory.MetadataReader;
            _specIfDataReader = dataProviderFactory.DataReader;
            _fileSaver = fileSaver;

            _rootNodeViewModel = new NodeViewModel(dataProviderFactory.MetadataReader, dataProviderFactory.DataReader,
                                                   dataProviderFactory.DataWriter, null, HierarchyKey);

            SetDefaultFilenameFromTitle();
            InitializeCommands();
        }

        

        private Key HierarchyKey { get; set; }

        private void SetDefaultFilenameFromTitle()
        {
            string filename = "";

            if(!string.IsNullOrEmpty(Title))
            {
                filename = Title;
                filename = filename.Replace(" ", "_");
                filename.Trim();

                FileName = filename;
            }
        }

        private void InitializeCommands()
        {
            ExportHierarchyCommand = new RelayCommand(ExecuteExportHierarchy);
        }

        #region Properties

        public string Title
        {
            get
            {
                string result = "";

                result = _rootNodeViewModel.Title;

                return result;
            }
        }

        public bool IncludeMetadata { get; set; } = false;

        public bool IncludeRevisions { get; set; } = false;

        public bool IncludeStatements { get; set; } = false;

        public string FileName { get; set; } = "";

        public SpecIfFileFormat FileFormat { get; set; } = SpecIfFileFormat.Specif;

        public string ErrorMessage { get; set; } = null;

        public string SuccessMessageKey { get; set; } = null;

        public string FileExtension
        {
            get
            {
                string result = ".specif";
                switch (FileFormat)
                {
                    case SpecIfFileFormat.Specif:
                        result = ".specif";
                        break;

                    case SpecIfFileFormat.Specifz:
                        result = ".specifz";
                        break;
                }

                return result;
            }
        }

        public System.IO.FileInfo ResultingFile { get; set; } = null;
        #endregion

        #region COMMAND_DEFINITIONS

        public ICommand ExportHierarchyCommand { get; set; }

        #endregion

        #region COMMAND_IMPLEMENTATIONS
        private void ExecuteExportHierarchy()
        {
            ErrorMessage = null;

            if(_fileSaver != null)
            {
                string filename;
                bool dialogResult = _fileSaver.ShowFileSaveDialog(out filename, FileName, "",
                                              "SpecIF (*" + FileExtension + ")|*" + FileExtension);
                
                if(dialogResult)
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        HierarchyExporter hierarchyExporter = new HierarchyExporter(_specIfMetadataReader,
                                                                                    _specIfDataReader);

                        DataModels.SpecIF specIF = hierarchyExporter.ExportHierarchy(HierarchyKey,
                                                                                     IncludeMetadata,
                                                                                     IncludeStatements,
                                                                                     IncludeRevisions);

                        JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };

                        string specifString = JsonConvert.SerializeObject(specIF, Formatting.Indented, serializerSettings);

                        byte[] specifBytes = Encoding.UTF8.GetBytes(specifString);

                        _fileSaver.WriteDataToFile(filename, specifBytes);

                        SuccessMessageKey = "Message.Success";
                    }
                    else
                    {
                        ErrorMessage = "Error.NoFileName";
                    }
                }
            }

        }

        #endregion
    }
}
