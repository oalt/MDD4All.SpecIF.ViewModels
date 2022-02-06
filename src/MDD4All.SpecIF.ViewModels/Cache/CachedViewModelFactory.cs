using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels.Cache
{
    public class CachedViewModelFactory
    {
        private static Dictionary<Key, ResourceViewModel> _resourceViewModelCache = new Dictionary<Key, ResourceViewModel>();

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

                    _resourceViewModelCache.Add(new Key(resource.ID, resource.Revision), resourceViewModel);

                    result = resourceViewModel;
                }

            }

            return result;
        }
    }
}
