using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.Converters;
using MDD4All.SpecIF.DataProvider.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class DataImportViewModel : ViewModelBase
    {
        private IHttpClientFactory _httpClientFactory;
        private ISpecIfMetadataWriter _metadataWriter;
        private ISpecIfDataWriter _dataWriter;

        public DataImportViewModel(IHttpClientFactory httpClientFactory,
                                   ISpecIfMetadataWriter metadataWriter,
                                   ISpecIfDataWriter dataWriter)
        {
            _httpClientFactory = httpClientFactory;
            _metadataWriter = metadataWriter;
            _dataWriter = dataWriter;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ImportSpecIfFromUrlCommand = new RelayCommand(ExecuteImportSpecIfFromUrlCommandAsync);
        }

        // key: localization string, value: URL
        public Dictionary<string, string> MetadataFiles
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                result.Add("File.SpecIF1.1Release", "https://raw.githubusercontent.com/GfSE/SpecIF-Class-Definitions/7098496f05a0fe071ff46ff172aa58004a9303d9/_Packages/SpecIF-Classes-1_1.specif");
                result.Add("File.SpecIF1.2Dev", "https://raw.githubusercontent.com/GfSE/SpecIF-Class-Definitions/dev/_Packages/SpecIF-Classes-1_2.specif");
                result.Add("File.SpecIF1.2DevNonNormative", "https://raw.githubusercontent.com/GfSE/SpecIF-Class-Definitions/dev/_Packages/SpecIF-Classes-1_2_non_normative.specif");

                return result;
            }
        }

        public string MetadataFileURL { get; set; } = "";

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

        public Exception Exception { get; set; }

        public ICommand ImportSpecIfFromUrlCommand { get; private set; }

        private async void ExecuteImportSpecIfFromUrlCommandAsync()
        {
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
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                Exception = exception;
            }
            finally
            {
                IsImportingData = false;
            }


        }
    }
}
