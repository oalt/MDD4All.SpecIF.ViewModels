using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels.Helpers;
using Newtonsoft.Json;

namespace MDD4All.SpecIF.ViewModels
{
    public class ResourceViewModel : ViewModelBase
    {
        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter)
        {
            _metadataReader = metadataReader;
            _specIfDataWriter = dataWriter;
            _specIfDataReader = dataReader;

            _node = null;

            
        }


        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 Node node) : this(metadataReader, dataReader, dataWriter)
        {
            _node = node;

            _resource = _specIfDataReader.GetResourceByKey(node.ResourceReference);

        }

        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 Resource resource) : this(metadataReader, dataReader, dataWriter)
        {
            _resource = resource;
        }

        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 string resourceId) : this(metadataReader, dataReader, dataWriter)
        {
            _resource = _specIfDataReader.GetResourceByKey(
                new Key(resourceId)
                );
        }

        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 string resourceId,
                                 string revision) : this(metadataReader, dataReader, dataWriter)
        {
            _resource = _specIfDataReader.GetResourceByKey(
                new Key() { ID = resourceId, Revision = revision }
                );

        }

        



        public Key HierarchyID { get; set; }

        private bool _isNew = false;

        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }

        public bool IsInEditMode { get; set; }

        private string _viewSource = "";

        public string ViewSource
        {
            get { return _viewSource; }
            set { _viewSource = value; }
        }

        private ISpecIfMetadataReader _metadataReader;

        public ISpecIfMetadataReader MetadataReader
        {
            get { return _metadataReader; }

        }



        private ISpecIfDataReader _specIfDataReader;

        public ISpecIfDataReader DataReader
        {
            get
            {
                return _specIfDataReader;
            }
        }

        private ISpecIfDataWriter _specIfDataWriter;

        public ISpecIfDataWriter DataWriter
        {
            get
            {
                return _specIfDataWriter;
            }
        }

        private Resource _resource;

        public Resource Resource
        {
            get
            {
                return _resource;
            }

            set
            {
                _resource = value;
            }
        }


        public string Type
        {
            get
            {
                string result = "";
                result = Resource.GetTypeName(_metadataReader);

                return result;
            }
        }

        private Node _node;

        public Node Node
        {
            get { return _node; }
            set { _node = value; }
        }





        public string Status
        {
            get
            {
                return _resource.Properties.Find(prop => prop.GetClassTitle(_metadataReader) == "SpecIF:Status")?.GetStringValue(_metadataReader);
            }

            set
            {

            }
        }

        public string Title
        {
            get
            {
                string result = "";

                if (_resource != null && _resource.Properties != null)
                {
                    result = _resource?.Properties?.Find(prop => prop.GetClassTitle(_metadataReader) == "dcterms:title")?.GetStringValue(_metadataReader);
                }

                if (string.IsNullOrEmpty(result) && _resource != null && _resource.GetTypeName(_metadataReader) != null)
                {
                    result = "[" + _resource.GetTypeName(_metadataReader) + "]";
                }

                return result;
            }
        }

        public string Description
        {
            get
            {
                string result = "";

                if (_resource != null && _resource.Properties != null)
                {
                    result = Resource.GetPropertyValue("dcterms:description", _metadataReader);
                }

                return result;
            }

            set
            {
                Resource.SetPropertyValue("dcterms:description", value, _metadataReader, TextFormat.XHTML);
            }
        }

        public string Diagram
        {
            get
            {
                string result = "";

                if (_resource != null)
                {
                    string diagram = _resource.GetPropertyValue("SpecIF:Diagram", _metadataReader);

                    if (!string.IsNullOrWhiteSpace(diagram))
                    {
                        result = diagram;
                    }

                }

                return result;
            }
        }

        public string Creator
        {
            get
            {
                string result = "";

                //List<Value> creator = Resource.GetPropertyValue(new Key("PC-Creator", "1"));


                result = _resource?.Properties?.Find(prop => prop.GetClassTitle(_metadataReader) == "dcterms:creator")?.GetStringValue(_metadataReader);


                if (string.IsNullOrEmpty(result))
                {
                    ;
                }

                return result;
            }
        }

        public string Identifier
        {
            get
            {
                string result = "";

                result = _resource?.Properties?.Find(prop => prop.GetClassTitle(_metadataReader) == "dcterms:identifier")?.GetStringValue(_metadataReader);

                return result;
            }
        }

        public string ClassifierName
        {
            get
            {
                string result = "not implemented";

                //Statement typeStatement = _specIfDataReader.GetAllStatementsForResource(new Key(_resource.ID, _resource.Revision)).Find(statement => statement.StatementSubject.ID == _resource.ID
                //                                                                                               && statement.Title.ToSimpleTextString() == "rdf:type");

                //if(typeStatement != null)
                //{
                //    Resource classifierResource = _specIfDataReader.GetResourceByKey(new Key(typeStatement.StatementObject.ID, typeStatement.Revision));

                //    if(classifierResource != null)
                //    {
                //        result = classifierResource.GetPropertyValue("dcterms:title", _metadataReader);
                //    }
                //}

                return result;
            }
        }

        public string Stereotype
        {
            get
            {
                string result = "";

                result = _resource?.Properties?.Find(prop => prop.GetClassTitle(_metadataReader) == "UML:Stereotype")?.GetStringValue(_metadataReader);

                if (!string.IsNullOrEmpty(result))
                {
                    result = "«" + result + "»";
                }

                return result;
            }
        }

        public string LongTitle
        {
            get
            {
                string result = "";

                if (!string.IsNullOrEmpty(Kind))
                {
                    result += "[" + Kind + "] ";
                }

                if (!string.IsNullOrEmpty(Stereotype))
                {
                    result += Stereotype + " ";
                }

                result += Title;

                if (string.IsNullOrEmpty(result))
                {
                    result = "<UNNAMED>";
                }

                string classifier = ClassifierName;

                if (!string.IsNullOrEmpty(classifier))
                {
                    result += " :" + classifier;
                }

                return result;
            }
        }

        public string Timestamp
        {
            get
            {
                string result = Resource.ChangedAt.ToString();

                return result;
            }
        }

        public string Icon
        {
            get
            {
                string result = "";
                string icon = Resource.GetResourceType(_metadataReader)?.Icon;

                if (icon != null)
                {
                    result = icon;
                }

                return result;
            }
        }

        public bool IsSafetyRelevant
        {
            get
            {
                bool result = false;

                string disciplines = Resource.GetPropertyValue("Discipline", _metadataReader);

                if (disciplines.Contains("Safety"))
                {
                    result = true;
                }

                return result;

            }
        }

        public bool IsSoftwareRelevant
        {
            get
            {
                bool result = false;

                string disciplines = Resource.GetPropertyValue("Discipline", _metadataReader);

                if (disciplines.Contains("Software"))
                {
                    result = true;
                }

                return result;

            }
        }

        public bool IsMechanicsRelevant
        {
            get
            {
                bool result = false;

                string disciplines = Resource.GetPropertyValue("Discipline", _metadataReader);

                if (disciplines.Contains("Mechanics"))
                {
                    result = true;
                }

                return result;

            }
        }

        public bool IsElectronicsRelevant
        {
            get
            {
                bool result = false;

                string disciplines = Resource.GetPropertyValue("Discipline", _metadataReader);

                if (disciplines.Contains("Electronics"))
                {
                    result = true;
                }

                return result;

            }
        }

        public string Revision
        {
            get
            {
                return Resource.Revision;
            }
        }

        public List<PropertyViewModel> Properties
        {
            get
            {
                List<PropertyViewModel> result = new List<PropertyViewModel>();

                if (Resource != null && Resource.Properties != null)
                {
                    foreach (PropertyClass propertyClass in PropertyClasses)
                    {
                        Property property = Resource.Properties.Find(p => p.GetClassTitle(_metadataReader) == propertyClass.Title);

                        PropertyViewModel propertyViewModel;

                        if (property == null)
                        {
                            propertyViewModel = new PropertyViewModel(MetadataReader, new Key(propertyClass.ID, 
                                                                                              propertyClass.Revision));
                        }
                        else
                        {
                            propertyViewModel = new PropertyViewModel(MetadataReader, property);
                        }

                         

                        result.Add(propertyViewModel);
                    }

                }

                return result;
            }
        }

        public List<PropertyClass> PropertyClasses
        {
            get
            {
                List<PropertyClass> result = new List<PropertyClass>();

                ResourceClass resourceClass = _metadataReader.GetResourceClassByKey(Resource.Class);

                if (resourceClass != null)
                {
                    GetPropertyClassesFromParentResourceClassRecursively(resourceClass, result);
                }

                return result;
            }
        }

        private void GetPropertyClassesFromParentResourceClassRecursively(ResourceClass currentClass, List<PropertyClass> result)
        {
            foreach (Key propertyClassKey in currentClass.PropertyClasses)
            {
                PropertyClass propertyClass = _metadataReader.GetPropertyClassByKey(propertyClassKey);

                result.Add(propertyClass);
            }

            if (currentClass.Extends != null)
            {
                ResourceClass parentClass = _metadataReader.GetResourceClassByKey(currentClass.Extends);

                GetPropertyClassesFromParentResourceClassRecursively(parentClass, result);
            }
        }



        public string ResourceClassID
        {
            get
            {
                return _metadataReader.GetResourceClassByKey(Resource.Class).ID;
            }
        }

        public string Kind
        {
            get
            {
                return GetKindForResource();
            }
        }

        public Key Key
        {
            get
            {
                Key result = new Key();

                if (_resource != null)
                {
                    result = new Key(_resource.ID, _resource.Revision);
                }

                return result;
            }

        }

        

        public string FormattedJson
        {
            get
            {
                string result = "";

                if (Resource != null)
                {
                    result = JsonConvert.SerializeObject(Resource, Formatting.Indented);
                }
                return result;
            }
        }

        public List<NodeViewModel> HierarchiesForResource
        {
            get
            {
                List<NodeViewModel> result = new List<NodeViewModel>();

                if (_resource != null)
                {
                    List<Node> hierarchyRoots = _specIfDataReader.GetContainingHierarchyRoots(new Key(_resource.ID, _resource.Revision));

                    foreach (Node node in hierarchyRoots)
                    {
                        NodeViewModel resourceViewModel = new NodeViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, node);

                        result.Add(resourceViewModel);
                    }
                }

                return result;
            }
        }

        private List<StatementViewModel> _incomingStatements = null;

        public List<StatementViewModel> IncomingStatements
        {
            get
            {
                List<StatementViewModel> result = new List<StatementViewModel>();

                if (_incomingStatements == null)
                {
                    InitializeStatements();
                }

                return _incomingStatements;
            }
        }

        private List<StatementViewModel> _outgoingStatements = null;

        public List<StatementViewModel> OutgoingStatements
        {
            get
            {
                List<StatementViewModel> result = new List<StatementViewModel>();

                if (_outgoingStatements == null)
                {
                    InitializeStatements();
                }

                return _outgoingStatements;
            }
        }

        private void InitializeStatements()
        {
            _incomingStatements = new List<StatementViewModel>();
            _outgoingStatements = new List<StatementViewModel>();

            List<Statement> allStatements = _specIfDataReader.GetAllStatementsForResource(new Key(_resource.ID, _resource.Revision));

            foreach (Statement inStatement in allStatements.FindAll(stm => stm.StatementObject.ID == _resource.ID))
            {
                _incomingStatements.Add(new StatementViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, inStatement));
            }

            foreach (Statement outStatement in allStatements.FindAll(stm => stm.StatementSubject.ID == _resource.ID))
            {
                _outgoingStatements.Add(new StatementViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, outStatement));
            }
        }

        private string GetKindForResource()
        {
            string result = "";

            string typeName = _resource.GetTypeName(_metadataReader);

            switch (typeName)
            {
                case "UML:Package":
                    result = "UML:Package";
                    break;



                case "SpecIF:Diagram":
                    string notation = _resource.GetPropertyValue("SpecIF:Notation", _metadataReader);

                    if (notation.StartsWith("OMG:"))
                    {
                        char[] seperator = { ':' };

                        string[] tokens = notation.Split(seperator);

                        notation = tokens[tokens.Length - 1];

                        result = "UML:" + notation;
                    }
                    else
                    {
                        result = notation;
                    }

                    break;

                case "UML:ActiveElement":
                case "UML:PassiveElement":
                default:
                    string type = _resource.GetPropertyValue("dcterms:type", _metadataReader);

                    if (type.StartsWith("OMG:"))
                    {
                        char[] seperator = { ':' };

                        string[] tokens = type.Split(seperator);

                        type = tokens[tokens.Length - 1];

                        result = "UML:" + type;
                    }
                    else
                    {
                        result = type;
                    }


                    break;
            }

            return result;
        }



    }


}
