using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels.Cache
{
    public class CachedViewModelFactory
    {
        private static ConcurrentDictionary<Key, ResourceViewModel> _resourceViewModelCache = new ConcurrentDictionary<Key, ResourceViewModel>();
        private static ConcurrentDictionary<Key, List<StatementViewModel>> _statementViewModelCache = new ConcurrentDictionary<Key, List<StatementViewModel>>();

        public static ResourceViewModel GetResourceViewModel(Key key,
                                                             ISpecIfMetadataReader metadataReader,
                                                             ISpecIfDataReader dataReader,
                                                             ISpecIfDataWriter dataWriter)
        {
            ResourceViewModel result = null;

            if(_resourceViewModelCache.ContainsKey(key))
            {
                result = _resourceViewModelCache[key];
            }
            else
            {
                
                Resource resource = dataReader.GetResourceByKey(key);

                if(resource != null)
                {
                    ResourceViewModel resourceViewModel = new ResourceViewModel(metadataReader,
                                                                                dataReader,
                                                                                dataWriter,
                                                                                resource);

                    _resourceViewModelCache.TryAdd(new Key(resource.ID, resource.Revision), resourceViewModel);

                    result = resourceViewModel;
                }

            }
            

            return result;
        }

        public static List<StatementViewModel> GetStatementViewModels(Key resourceKey,
                                                                      ISpecIfMetadataReader metadataReader,
                                                                      ISpecIfDataReader dataReader,
                                                                      ISpecIfDataWriter dataWriter)
        {
            List<StatementViewModel> result = null;

            if (_statementViewModelCache.ContainsKey(resourceKey))
            {
                result = _statementViewModelCache[resourceKey];
            }
            else
            {
                List<Statement> statements = dataReader.GetAllStatementsForResource(resourceKey);

                if (statements != null)
                {
                    List<StatementViewModel> viewModels = new List<StatementViewModel>();
                    foreach (Statement statement in statements)
                    {
                        StatementViewModel resourceViewModel = new StatementViewModel(metadataReader,
                                                                                      dataReader,
                                                                                      dataWriter,
                                                                                      statement);

                        viewModels.Add(resourceViewModel);
                    }
                    _statementViewModelCache.TryAdd(resourceKey, viewModels);

                    result = viewModels;
                }

            }

            return result;
        }
    }
}
