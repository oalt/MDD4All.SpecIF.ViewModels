using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Cache;
using System.Collections.Generic;

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

        public Key StatementClassKey
        {
            get
            {
                return _statement.Class;
            }
        }

        public bool IsLoop
        {
            get
            {
                return _statement.StatementObject.ID == _statement.StatementSubject.ID;
            }
        }

        public override List<PropertyClass> PropertyClasses
        {
            get
            {
                List<PropertyClass> result = new List<PropertyClass>();

                StatementClass resourceClass = MetadataReader.GetStatementClassByKey(Resource.Class);

                if (resourceClass != null)
                {
                    GetPropertyClassesFromParentStatementClassRecursively(resourceClass, result);
                }

                return result;
            }
        }

        private void GetPropertyClassesFromParentStatementClassRecursively(StatementClass currentClass, List<PropertyClass> result)
        {
            if (currentClass.PropertyClasses != null)
            {
                foreach (Key propertyClassKey in currentClass.PropertyClasses)
                {
                    PropertyClass propertyClass = MetadataReader.GetPropertyClassByKey(propertyClassKey);

                    result.Add(propertyClass);
                }

                if (currentClass.Extends != null)
                {
                    StatementClass parentClass = MetadataReader.GetStatementClassByKey(currentClass.Extends);

                    GetPropertyClassesFromParentStatementClassRecursively(parentClass, result);
                }
            }
        }
    }
}
