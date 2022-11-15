using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MDD4All.SpecIF.ViewModels.Metadata
{
    public class StatementClassesViewModel : ResourceClassesViewModel
    {
        public StatementClassesViewModel(ISpecIfMetadataReader metadataReader,
                                         ISpecIfMetadataWriter metadataWriter) : base(metadataReader, metadataWriter)
        {

        }

        public List<StatementClassViewModel> StatementClasses
        {
            get
            {
                List<StatementClassViewModel> result = new List<StatementClassViewModel>();

                List<StatementClass> statementClasses = _specIfMetadataReader.GetAllStatementClasses();

                foreach (StatementClass statementClass in statementClasses)
                {
                    result.Add(new StatementClassViewModel(_specIfMetadataReader, _specIfMetadataWriter, statementClass));
                }

                return result;
            }
        }


    }
}
