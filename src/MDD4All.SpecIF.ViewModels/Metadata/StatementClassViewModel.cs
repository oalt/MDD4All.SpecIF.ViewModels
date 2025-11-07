using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class StatementClassViewModel : ResourceClassViewModel
    {
        public StatementClassViewModel(ISpecIfMetadataReader metadataReader,
                                       ISpecIfMetadataWriter metadataWriter,
                                       StatementClass statementClass) 
                                            : base(metadataReader, metadataWriter, statementClass)
        {

        }

        public StatementClassViewModel(ISpecIfMetadataReader metadataReader,
                                       ISpecIfMetadataWriter metadataWriter,
                                       Key statementClassKey) : base(metadataReader, metadataWriter)
        {
            ResourceClass = metadataReader.GetStatementClassByKey(statementClassKey);
        }

        public StatementClass? StatementClass
        {
            get 
            {
                StatementClass? result = null; 
                
                if(base.ResourceClass != null)
                {
                    result = (StatementClass)base.ResourceClass;
                }
                
                return result;
            }
        }

    }
}
