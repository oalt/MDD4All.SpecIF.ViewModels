using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.FileAccess.Contracts;
using MDD4All.SpecIF.Converters;
using MDD4All.SpecIF.DataProvider.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class DataImportViewModel : ViewModelBase
    {
        private IHttpClientFactory _httpClientFactory;
        private ISpecIfMetadataReader _metadataReader;
        private ISpecIfMetadataWriter _metadataWriter;
        private ISpecIfDataWriter _dataWriter;
        private IFileLoader _fileLoader;

        public DataImportViewModel(IHttpClientFactory httpClientFactory,
                                   ISpecIfMetadataReader metadataReader,
                                   ISpecIfMetadataWriter metadataWriter,
                                   ISpecIfDataWriter dataWriter,
                                   IFileLoader fileLoader)
        {
            _httpClientFactory = httpClientFactory;
            _metadataReader = metadataReader;
            _metadataWriter = metadataWriter;
            _dataWriter = dataWriter;
            _fileLoader = fileLoader;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ImportSpecIfFromUrlCommand = new RelayCommand(ExecuteImportSpecIfFromUrlCommandAsync);
            SelectFileCommand = new RelayCommand(ExecuteSelectFileCommand);
            ImportSpecIfFromFileCommand = new RelayCommand(ExecuteImportSpecIfFromFileCommandAsync);
        }

        

        // key: localization string, value: URL
        public Dictionary<string, string> MetadataFiles
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                result.Add("File.SpecIF1_1Release", "https://raw.githubusercontent.com/GfSE/SpecIF-Class-Definitions/7098496f05a0fe071ff46ff172aa58004a9303d9/_Packages/SpecIF-Classes-1_1.specif");
                result.Add("File.SpecIF1_2Dev", "https://raw.githubusercontent.com/GfSE/SpecIF-Class-Definitions/dev/_Packages/SpecIF-Classes-1_2.specif");
                result.Add("File.SpecIF1_2DevNonNormative", "https://raw.githubusercontent.com/GfSE/SpecIF-Class-Definitions/dev/_Packages/SpecIF-Classes-1_2_non_normative.specif");

                return result;
            }
        }

        public string MetadataFileURL { get; set; } = "";

        public string LocalFilename { get; set; } = "";

        

        public bool OverrideExistingData 
        { 
            get; 
            set; 
        } = false;

        private bool _isImportingData = false;

        public bool IsImportingData
        {
            get { return _isImportingData; }
            set
            {
                _isImportingData = value;
                RaisePropertyChanged("IsImportingData");
            }
        }

        private string _progressMessageKey = "";

        public string ProgressMessageKey
        {
            get
            {
                return _progressMessageKey;
            }

            set
            {
                _progressMessageKey = value;
                RaisePropertyChanged("ProgressMessageKey");
            }
        }

        private string _successMessageKey;

        public string SuccessMessageKey
        {
            get 
            {
                return _successMessageKey; 
            }

            set
            {
                _successMessageKey = value;
                RaisePropertyChanged("SuccessMessageKey");
            }
        }

        private string _errorMessageKey;

        public string ErrorMessageKey
        {
            get
            {
                return _errorMessageKey;
            }

            set
            {
                _errorMessageKey = value;
                RaisePropertyChanged("ErrorMessageKey");
            }
        }

        public Exception Exception { get; set; }

        #region COMMAND_DEFINITIONS
        public ICommand ImportSpecIfFromUrlCommand { get; private set; }

        public ICommand SelectFileCommand { get; private set; }

        public ICommand ImportSpecIfFromFileCommand { get; private set; }
        #endregion

        private async void ExecuteImportSpecIfFromUrlCommandAsync()
        {
            ClearMessages();

            IsImportingData = true;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, MetadataFileURL);

            HttpClient httpClient = _httpClientFactory.CreateClient();

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    ProgressMessageKey = "Message.Downloading";
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseString))
                    {
                        DataModels.SpecIF specIF = JsonConvert.DeserializeObject<DataModels.SpecIF>(responseString);

                        if (specIF != null)
                        {
                            SpecIfConverter specIfConverter = new SpecIfConverter();
                            ProgressMessageKey = "Message.ImportingData";
                            specIfConverter.ConvertAll(specIF, _dataWriter, _metadataWriter, OverrideExistingData);

                            _metadataReader.NotifyMetadataChanged();

                            SuccessMessageKey = "Message.ImportSuccess";
                        }
                        else
                        {
                            ErrorMessageKey = "Error.SpecIfParsing";
                        }
                    }
                    else
                    {
                        ErrorMessageKey = "Error.Download";
                    }

                }
                else
                {
                    ErrorMessageKey = "Error.Download";
                }
            }
            catch (Exception exception)
            {
                
                Exception = exception;
                ErrorMessageKey = "Error.ImportError";
            }
            finally
            {
                IsImportingData = false;
            }


        }

        private void ExecuteImportSpecIfFromFileCommandAsync()
        {
            ClearMessages();

            IsImportingData = true;

            try
            {
                if (!string.IsNullOrEmpty(LocalFilename))
                {

                    byte[] data = _fileLoader.ReadDataFromFile(LocalFilename);

                    if (data != null)
                    {
                        string json = Encoding.UTF8.GetString(data, 0, data.Length);

                        if (json != null)
                        {
                            DataModels.SpecIF specIF = JsonConvert.DeserializeObject<DataModels.SpecIF>(json);

                            if (specIF != null)
                            {
                                SpecIfConverter specIfConverter = new SpecIfConverter();
                                ProgressMessageKey = "Message.ImportingData";
                                specIfConverter.ConvertAll(specIF, _dataWriter, _metadataWriter, OverrideExistingData);

                                _metadataReader.NotifyMetadataChanged();

                                SuccessMessageKey = "Message.ImportSuccess";
                            }
                            else
                            {
                                ErrorMessageKey = "Error.SpecIfParsing";
                            }
                        }
                    }
                    else
                    {
                        ErrorMessageKey = "Error.FileRead";
                    }
                }
            }
            catch(Exception exception)
            {

            }
            finally
            {
                IsImportingData = false;
            }
        }

        private void ExecuteSelectFileCommand()
        {
            if (_fileLoader != null)
            {
                string filename;
                bool dialogResult = _fileLoader.ShowOpenFileDialog(out filename, LocalFilename, "",
                                                                   "SpecIF (*.specif)|*.specif");

                if (dialogResult)
                {
                    if (!string.IsNullOrEmpty(filename))
                    { 
                        LocalFilename = filename;
                    }
                }
            }
        }

        private void ClearMessages()
        {
            ProgressMessageKey = "";
            ErrorMessageKey = "";
            SuccessMessageKey = "";
            Exception = null;

        }
                
    }
}
