using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
//using MDD4All.SpecIF.Converters;
using MDD4All.SpecIF.DataProvider.Contracts;
//using MDD4All.SpecIF.DataProvider.File;
using System;
using System.Windows.Input;

namespace MDD4All.SpecIF.ViewModels
{
    public class FileViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _metadataReader;
        private ISpecIfMetadataWriter _metadataWriter;
        private ISpecIfDataReader _dataReader;
        private ISpecIfDataWriter _dataWriter;

        public FileViewModel(ISpecIfMetadataReader metadataReader,
                             ISpecIfMetadataWriter metadataWriter,
                             ISpecIfDataReader dataReader,
                             ISpecIfDataWriter dataWriter)
        {
            _metadataReader = metadataReader;
            _metadataWriter = metadataWriter;
            _dataReader = dataReader;
            _dataWriter = dataWriter;

            UploadFileCommand = new RelayCommand(UploadFile);
        }

        public string TempFileName { get; set; }

        public string FileName { get; set; } = "";

        public bool OverrideExistingData { get; set; } = false;

        public bool Success { get; set; } = false;

        public string ErrorMessage { get; set; } = "";

        public ICommand UploadFileCommand { get; private set; }

        private void UploadFile()
        {
            throw new NotImplementedException();
            //if(!string.IsNullOrEmpty(TempFileName))
            //{
            //	SpecIfConverter specIfConverter = new SpecIfConverter();

            //	DataModels.SpecIF specIF = SpecIfFileReaderWriter.ReadDataFromSpecIfFile(TempFileName);

            //	if (specIF != null)
            //	{
            //		specIfConverter.ConvertAll(specIF,
            //								   _dataWriter,
            //								   _metadataWriter,
            //								   OverrideExistingData);
            //		Success = true;
            //	}
            //	else
            //	{
            //		Success = false;
            //		ErrorMessage = "Error reading SpecIF file.";
            //	}
            //}
        }
    }
}
