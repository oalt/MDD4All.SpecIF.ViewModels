using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Cache;

namespace MDD4All.SpecIF.ViewModels
{
    public class StatementViewModel : ResourceViewModel
    {

        private Statement _statement;



        public StatementViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter) : base(metadataReader, dataReader, dataWriter)
        {

        }

        public StatementViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter, Statement statement) : base(metadataReader, dataReader, dataWriter)
        {

            Resource = statement;

            _statement = statement;

            InitializeSubjectAndObject();
        }


        public StatementViewModel(ISpecIfMetadataReader metadataReader,
                                  ISpecIfDataReader dataReader,
                                  ISpecIfDataWriter dataWriter,
                                  string resourceId,
                                  string revision) : this(metadataReader, dataReader, dataWriter)
        {
            _statement = DataReader.GetStatementByKey(
                new Key() { ID = resourceId, Revision = revision }
                );

            Resource = _statement;

            InitializeSubjectAndObject();
        }

        private void InitializeSubjectAndObject()
        {
            _subjectResource = CachedViewModelFactory.GetResourceViewModel(_statement.StatementSubject,
                                                                           MetadataReader, 
                                                                           DataReader, 
                                                                           DataWriter);

            _objectResource = CachedViewModelFactory.GetResourceViewModel(_statement.StatementObject,
                                                                          MetadataReader, 
                                                                          DataReader, 
                                                                          DataWriter);
        }

        public string StatementID
        {
            get
            {
                return _statement.ID;
            }
        }

        public string StatementRevision
        {
            get
            {
                return _statement.Revision;
            }
        }

        private ResourceViewModel _subjectResource;

        public ResourceViewModel SubjectResource
        {
            get
            {
                return _subjectResource;
            }
        }

        private ResourceViewModel _objectResource;

        public ResourceViewModel ObjectResource
        {
            get
            {
                return _objectResource;
            }
        }

        public bool IsLoop
        {
            get
            {
                return _statement.StatementObject.ID == _statement.StatementSubject.ID;
            }
        }

    }
}
