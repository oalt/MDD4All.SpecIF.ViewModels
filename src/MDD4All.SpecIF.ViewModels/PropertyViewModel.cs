using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Models;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels
{
    public class PropertyViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _specIfMetadataReader;

        public PropertyViewModel(ISpecIfMetadataReader specIfMetadataReader,
                                 Property property)
        {
            _specIfMetadataReader = specIfMetadataReader;
            Property = property;

            InitailizeEnumerationOptions();
        }

        public Property Property { get; set; }

        public string Title
        {
            get
            {
                string result = "<Unknown>";

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(Property.Class);

                if (propertyClass != null)
                {
                    result = propertyClass.Title;
                }

                return result;
            }
        }

        public string Value
        {
            get
            {
                return Property.GetStringValue(_specIfMetadataReader);
            }
            set
            {
                if (DataTypeType == "xs:string")
                {
                    Value val = new Value(new MultilanguageText
                    {
                        Text = value,
                        Format = Format
                    });

                    if (Property.Values.Count > 0)
                    {
                        Property.Values[0] = val;
                    }
                    else
                    {
                        Property.Values.Add(val);
                    }
                }
                else
                {
                    if (Property.Values.Count > 0)
                    {
                        Property.Values[0].StringValue = value;
                    }
                    else
                    {
                        Property.Values.Add(new Value(value));
                    }
                }
            }
        }

        public string Unit
        {
            get
            {
                string result = "";

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(Property.Class);

                if (propertyClass != null)
                {
                    result = propertyClass.Unit;
                }

                return result;
            }
        }

        public DataType DataType
        {
            get
            {
                DataType result = null;

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(Property.Class);
                if (propertyClass != null)
                {
                    result = _specIfMetadataReader.GetDataTypeByKey(propertyClass.DataType);
                }

                return result;
            }
        }

        public string DataTypeType
        {
            get
            {
                string result = "xs:string";

                DataType dataType = DataType;

                if (dataType != null)
                {
                    result = dataType.Type;
                }

                return result;
            }
        }

        public string Format
        {
            get
            {
                string result = "plain";

                string defaultFormat = "plain";

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(Property.Class);

                if (propertyClass != null)
                {
                    if (!string.IsNullOrEmpty(propertyClass.Format))
                    {
                        defaultFormat = propertyClass.Format;
                    }

                    if (Property.Values.Count > 0)
                    {
                        if (Property.Values[0].IsStringValue)
                        {
                            result = defaultFormat;
                        }
                        else
                        {
                            Value firstValue = Property.Values[0];

                            string format = firstValue.MultilanguageText[0].Format;

                            if (!string.IsNullOrEmpty(format))
                            {
                                result = format;
                            }
                            else
                            {
                                result = defaultFormat;
                            }
                        }
                    }
                    else
                    {
                        result = defaultFormat;
                    }

                }

                return result;
            }
        }

        public bool IsEnumeration
        {
            get
            {
                bool result = false;

                DataType dataType = DataType;

                if(dataType != null && dataType.Enumeration != null && dataType.Enumeration.Count > 0)
                {
                    result = true;
                }

                return result;
            }
        }

        public List<EnumSelectItem> EnumerationOptions { get; set; } = new List<EnumSelectItem>();

        private void InitailizeEnumerationOptions()
        {
            if(IsEnumeration)
            {
                if(DataType.Enumeration != null)
                {
                    foreach(EnumerationValue enumerationValue in DataType.Enumeration)
                    {
                        EnumSelectItem enumSelectItem = new EnumSelectItem
                        {
                            Title = enumerationValue.Value[0].Text,
                            Value = enumerationValue.ID
                        };

                        EnumerationOptions.Add(enumSelectItem);
                    }
                }
            }
        }


    }
}
