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

        public StatementClass StatementClass
        {
            get { return (StatementClass)base.ResourceClass; }
        }

    }
}
