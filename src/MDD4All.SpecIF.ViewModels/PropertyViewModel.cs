using GalaSoft.MvvmLight;

namespace MDD4All.SpecIF.ViewModels
{
    public class PropertyViewModel : ViewModelBase
    {
        public string TypeName { get; set; }

        public string Value { get; set; }

        public string PropertyClassID { get; set; }

        public string PropertyClassRevisionString { get; set; }
    }
}
