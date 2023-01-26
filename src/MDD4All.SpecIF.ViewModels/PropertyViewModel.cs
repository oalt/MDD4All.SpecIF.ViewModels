using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Models;
using System;
using System.Collections.Generic;

namespace MDD4All.SpecIF.ViewModels
{
    public class PropertyViewModel : ViewModelBase
    {
        private ISpecIfMetadataReader _specIfMetadataReader;
        private ResourceViewModel _resourceViewModel;

        public PropertyViewModel(ISpecIfMetadataReader specIfMetadataReader,
                                 Property property)
        {
            _specIfMetadataReader = specIfMetadataReader;
            Property = property;

            PropertyClassKey = property.Class;

            PropertyClass = _specIfMetadataReader.GetPropertyClassByKey(PropertyClassKey);

            InitailizeEnumerationOptions();
        }

        public PropertyViewModel(ISpecIfMetadataReader specIfMetadataReader,
                                 Key propertyClass,
                                 ResourceViewModel resourceViewModel)
        {
            _specIfMetadataReader = specIfMetadataReader;
            _resourceViewModel = resourceViewModel;

            PropertyClassKey = propertyClass;

            PropertyClass = _specIfMetadataReader.GetPropertyClassByKey(propertyClass);
        }

        public Key PropertyClassKey { get; set; }

        public Property Property
        {
            get;
            set;
        }

        public PropertyClass PropertyClass { get; set; }

        public string Title
        {
            get
            {
                string result = "<Unknown>";

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(PropertyClassKey);

                if (propertyClass != null)
                {
                    result = propertyClass.Title;
                }

                return result;
            }
        }

        public bool HasMultipleValues
        {
            get
            {
                bool result = false;

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(PropertyClassKey);

                if (propertyClass != null)
                {
                    if (propertyClass.Multiple.HasValue)
                    {
                        result = propertyClass.Multiple.Value;
                    }

                }

                return result;
            }
        }

        public string Value
        {
            get
            {
                string result = "";
                if (Property != null)
                {
                    result = Property.GetStringValue(_specIfMetadataReader);
                }
                return result;
            }

            set
            {
                if (value != null)
                {

                    if (DataTypeType == "xs:string")
                    {
                        MultilanguageText multilanguageTextValue = new MultilanguageText
                        {
                            Text = value,
                            Format = Format
                        };

                        if (Property == null)
                        {
                            Property = new Property(PropertyClassKey, multilanguageTextValue);
                            if(_resourceViewModel != null)
                            {
                                _resourceViewModel.Resource.Properties.Add(Property);
                            }
                        }
                        else
                        {
                            Value val = new Value(multilanguageTextValue);
                            if (Property.Values.Count > 0)
                            {
                                Property.Values[0] = val;
                            }
                            else
                            {
                                Property.Values.Add(val);
                            }
                        }
                    }
                    else
                    {
                        if (Property == null)
                        {
                            Property = new Property(PropertyClassKey, value);
                            if (_resourceViewModel != null)
                            {
                                _resourceViewModel.Resource.Properties.Add(Property);
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
            }
        }

        public List<string> Values
        {
            get
            {
                List<string> result = new List<string>();

                result = Property.GetStringValues(_specIfMetadataReader);

                return result;
            }
        }

        public void SetSingleEnumerationValue(string valueID,
                                              int index = 0)
        {
            if(Property == null)
            {
                Property = new Property(PropertyClassKey, new List<Value>());
                if (_resourceViewModel != null)
                {
                    _resourceViewModel.Resource.Properties.Add(Property);
                }
            }
            
            // Add empty values if the index can not be used in current property structure
            if (index > Property.Values.Count - 1)
            {
                for (int counter = Property.Values.Count; counter <= index; counter++)
                {
                    Property.Values.Add(new Value());
                }
            }

            Property.Values[index].StringValue = valueID;
        }

        public void SetMultipleEnumerationValue(List<string> valueIds,
                                                int index = 0)
        {
            if (Property == null)
            {
                Property = new Property(PropertyClassKey, new List<Value>());
                if (_resourceViewModel != null)
                {
                    _resourceViewModel.Resource.Properties.Add(Property);
                }
            }
            

            // Add empty values if the index can not be used in current property structure
            if (index > Property.Values.Count - 1)
            {
                for (int counter = Property.Values.Count; counter <= index; counter++)
                {
                    Property.Values.Add(new Value());
                }
            }

            string multiValue = "";

            if (valueIds.Count > 0)
            {
                for (int counter = 0; counter < valueIds.Count; counter++)
                {
                    multiValue += valueIds[counter];

                    if (counter < valueIds.Count - 1)
                    {
                        multiValue += ",";
                    }
                }


                Property.Values[index].StringValue = multiValue;
            }
            else
            {
                Property.Values[index].StringValue = null;
            }
        }

        public string DurationValue
        {
            get
            {
                string result = "";
                
                string value = Value;

                if (!string.IsNullOrEmpty(value))
                {
                    TimeSpan timeSpan = Value.ToTimeSpan();

                    if (timeSpan != null)
                    {
                        result = timeSpan.ToString();
                    }
                }


                return result;
            }
        }


        public List<List<string>> EnumerationValues
        {
            get
            {
                return Property.GetEnumerationValues(_specIfMetadataReader);
            }
        }

        public string EnumerationValue
        {
            get
            {
                string result = "";
                if (Property != null)
                {
                    List<List<string>> enumerationValues = Property.GetEnumerationValues(_specIfMetadataReader);
                    if (enumerationValues.Count > 0)
                    {
                        result = enumerationValues[0][0];
                    }
                }

                return result;
            }
        }

        public string Unit
        {
            get
            {
                string result = "";

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(PropertyClassKey);

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

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(PropertyClassKey);
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
                string result = TextFormat.Plain;

                //string defaultFormat = "plain";

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(PropertyClassKey);

                if (propertyClass != null)
                {
                    if (!string.IsNullOrEmpty(propertyClass.Format))
                    {
                        result = propertyClass.Format;
                    }

                    //if (Property.Values.Count > 0)
                    //{
                    //    if (Property.Values[0].IsStringValue)
                    //    {
                    //        result = defaultFormat;
                    //    }
                    //    else
                    //    {
                    //        Value firstValue = Property.Values[0];

                    //        string format = firstValue.MultilanguageTexts[0].Format;

                    //        if (!string.IsNullOrEmpty(format))
                    //        {
                    //            result = format;
                    //        }
                    //        else
                    //        {
                    //            result = defaultFormat;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    result = defaultFormat;
                    //}

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

                if (dataType != null && dataType.Enumeration != null && dataType.Enumeration.Count > 0)
                {
                    result = true;
                }

                return result;
            }
        }

        public bool IsMultipleEnumeration
        {
            get
            {
                bool result = false;

                DataType dataType = DataType;

                if (dataType != null && dataType.Multiple.HasValue)
                {
                    result = dataType.Multiple.Value;
                }

                return result;
            }
        }

        public List<EnumSelectItem> EnumerationOptions { get; set; } = new List<EnumSelectItem>();

        private void InitailizeEnumerationOptions()
        {
            if (IsEnumeration)
            {
                if (DataType.Enumeration != null)
                {
                    foreach (EnumerationValue enumerationValue in DataType.Enumeration)
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
