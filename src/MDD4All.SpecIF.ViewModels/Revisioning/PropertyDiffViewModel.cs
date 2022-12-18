using GalaSoft.MvvmLight;

namespace MDD4All.SpecIF.ViewModels.Revisioning
{
    public class PropertyDiffViewModel : ViewModelBase
    {
        public string Title { get; set; }

        public PropertyViewModel PropertyRevisionOne { get; set; }

        public PropertyViewModel PropertyRevisionTwo { get; set; }
    }
}
