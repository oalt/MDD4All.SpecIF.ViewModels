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

        public PropertyViewModel(ISpecIfMetadataReader specIfMetadataReader,
                                 Property property)
        {
            _specIfMetadataReader = specIfMetadataReader;
            Property = property;

            InitailizeEnumerationOptions();
        }

        public PropertyViewModel(ISpecIfMetadataReader specIfMetadataReader,
                                 Key propertyClass)
        {
            _specIfMetadataReader = specIfMetadataReader;

            Property = new Property(new Key(propertyClass.ID, propertyClass.Revision), "");

            Property.Values = new List<Value>();
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

        public bool HasMultipleValues
        {
            get
            {
                bool result = false;

                PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(Property.Class);

                if (propertyClass != null)
                {
                    if(propertyClass.Multiple.HasValue)
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

        public List<string> Values
        {
            get
            {
                List<string> result = new List<string>();

                result = Property.GetStringValues(_specIfMetadataReader);

                return result;
            }
        }

        public string DurationValue
        {
            get
            {
                string result = "";

                TimeSpan timeSpan = Value.ToTimeSpan();

                if(timeSpan != null)
                {
                    result = timeSpan.ToString();
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
                
                List<List<string>> enumerationValues = Property.GetEnumerationValues(_specIfMetadataReader);
                if (enumerationValues.Count > 0)
                {
                    result = enumerationValues[0][0];
                }
                
                return result;
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

                //PropertyClass propertyClass = _specIfMetadataReader.GetPropertyClassByKey(Property.Class);

                //if (propertyClass != null)
                //{
                //    if (!string.IsNullOrEmpty(propertyClass.Format))
                //    {
                //        defaultFormat = propertyClass.Format;
                //    }

                //    if (Property.Values.Count > 0)
                //    {
                //        if (Property.Values[0].IsStringValue)
                //        {
                //            result = defaultFormat;
                //        }
                //        else
                //        {
                //            Value firstValue = Property.Values[0];

                //            string format = firstValue.MultilanguageTexts[0].Format;

                //            if (!string.IsNullOrEmpty(format))
                //            {
                //                result = format;
                //            }
                //            else
                //            {
                //                result = defaultFormat;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        result = defaultFormat;
                //    }

                //}

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
