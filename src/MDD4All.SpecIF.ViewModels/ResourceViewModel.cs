using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using System.Collections.Generic;
using Newtonsoft.Json;
using MDD4All.SpecIF.ViewModels.Revisioning;
using Vis = VisNetwork.Blazor.Models;
using MDD4All.SpecIF.ViewModels.Cache;
using System.Threading.Tasks;

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
            Task.Run(() => InitializeStatementsAsync());

        }

        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 Resource resource) : this(metadataReader, dataReader, dataWriter)
        {
            _resource = resource;
            Task.Run(() => InitializeStatementsAsync());
        }

        public ResourceViewModel(ISpecIfMetadataReader metadataReader,
                                 ISpecIfDataReader dataReader,
                                 ISpecIfDataWriter dataWriter,
                                 string resourceId) : this(metadataReader, dataReader, dataWriter)
        {
            _resource = _specIfDataReader.GetResourceByKey(new Key(resourceId));
            Task.Run(() => InitializeStatementsAsync());
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
            Task.Run(() => InitializeStatementsAsync());

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

        public string Priority
        {
            get
            {
                string result = "";
                PropertyViewModel propertyViewModel = Properties.Find(property => property.Title == "SpecIF:Priority");
                if(propertyViewModel != null)
                {
                    result = propertyViewModel.EnumerationValue;
                }
                return result;
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
                string result = "";

                if (OutgoingStatements != null)
                {
                    StatementViewModel typeStatement = OutgoingStatements.Find(statement => statement.Type == "rdf:type");

                    if (typeStatement != null)
                    {
                        result = typeStatement.ObjectResource.Title;
                    }
                }
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

        public string LegacyType
        {
            get
            {
                string result = "";

                result = _resource?.Properties?.Find(prop => prop.GetClassTitle(_metadataReader) == "dcterms:type")?.GetStringValue(_metadataReader);

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

        public string StatusColor
        {
            get
            {
                string result = "transparent";

                string status = Resource.GetPropertyValue("SpecIF:LifeCycleStatus", _metadataReader);

                if (status == "V-Status-0")
                {
                    result = "#888888";
                }
                else if (status == "V-Status-1")
                {
                    result = "#FF0000";
                }
                else if (status == "V-Status-2")
                {
                    result = "#FFFFFF";
                }
                else if (status == "V-Status-3")
                {
                    result = "#AAAAAA";
                }
                else if (status == "V-Status-4")
                {
                    result = "#0000FF";
                }
                else if (status == "V-Status-5")
                {
                    result = "#7771F0";
                }
                else if (status == "V-Status-6")
                {
                    result = "#00FF00";
                }
                else if (status == "V-Status-7")
                {
                    result = "#64A98A";
                }

                return result;
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

        //public List<NodeViewModel> HierarchiesForResource
        //{
        //    get
        //    {
        //        List<NodeViewModel> result = new List<NodeViewModel>();

        //        if (_resource != null)
        //        {
        //            List<Node> hierarchyRoots = _specIfDataReader.GetContainingHierarchyRoots(new Key(_resource.ID, _resource.Revision));

        //            foreach (Node node in hierarchyRoots)
        //            {
        //                NodeViewModel resourceViewModel = new NodeViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, node);

        //                result.Add(resourceViewModel);
        //            }
        //        }

        //        return result;
        //    }
        //}



        public bool StatementsInitialized
        {
            get
            {
                return !(_incomingStatements == null && _outgoingStatements == null);
            }

            set
            {
                RaisePropertyChanged("StatementsInitialized");
            }
        }


        public List<StatementViewModel> Statements
        {
            get
            {
                if (!StatementsInitialized)
                {
                    Task.Run(InitializeStatementsAsync).Wait();
                }
                List<StatementViewModel> result = new List<StatementViewModel>();

                result.AddRange(IncomingStatements);
                result.AddRange(OutgoingStatements);

                return result;
            }
        }

        private List<StatementViewModel> _incomingStatements = null;

        public List<StatementViewModel> IncomingStatements
        {
            get
            {
                return _incomingStatements;
            }
        }

        private List<StatementViewModel> _outgoingStatements = null;

        public List<StatementViewModel> OutgoingStatements
        {
            get
            {
                return _outgoingStatements;
            }
        }

        private async Task InitializeStatementsAsync()
        {
            // as singleton
            if (_incomingStatements == null || _outgoingStatements == null)
            {
                _incomingStatements = new List<StatementViewModel>();
                _outgoingStatements = new List<StatementViewModel>();


                List<StatementViewModel> allStatements = new List<StatementViewModel>();

                await Task.Run(() =>
                {
                    allStatements = CachedViewModelFactory.GetStatementViewModels(Key,
                                                                                  _metadataReader,
                                                                                  _specIfDataReader,
                                                                                  _specIfDataWriter);

                });

                _incomingStatements.AddRange(allStatements.FindAll(statement => statement.ObjectResource.Key.Equals(Key)));
                _outgoingStatements.AddRange(allStatements.FindAll(statement => statement.SubjectResource.Key.Equals(Key)));

                StatementsInitialized = true;
            }
        }


        private Vis.NetworkData _statementGraph = null;

        public Vis.NetworkData StatementGraph
        {
            get
            {
                if (_statementGraph == null && StatementsInitialized)
                {
                    InitailizeStatementGraph();
                }
                return _statementGraph;
            }
        }

        private void InitailizeStatementGraph()
        {
            List<Vis.Node> statementNodes = new List<Vis.Node>();
            List<Vis.Edge> statementEdges = new List<Vis.Edge>();

            Vis.Node resourceNode = new Vis.Node(Key.ToString(), CalculateLabelForStatementGraphNode(this), 2, "box");
            resourceNode.Font = new Vis.NodeFontOption
            {
                Multi = true
            };
            resourceNode.Color = new Vis.NodeColorType()
            {
                Background = "#FFE690"
            };

            statementNodes.Add(resourceNode);

            foreach (StatementViewModel incommingStatement in IncomingStatements)
            {
                if (!incommingStatement.IsLoop)
                {
                    Vis.Node node = new Vis.Node(incommingStatement.SubjectResource.Key.ToString(),
                                                 CalculateLabelForStatementGraphNode(incommingStatement.SubjectResource), 1, "box");

                    node.Font = new Vis.NodeFontOption
                    {
                        Multi = true
                    };
                    statementNodes.Add(node);

                    Vis.Edge edge = new Vis.Edge(incommingStatement.SubjectResource.Key.ToString(), Key.ToString());

                    edge.Arrows = new Vis.Arrows()
                    {
                        To = new Vis.ArrowsOptions
                        {
                            Enabled = true,
                            Type = "arrow"
                        }
                    };
                    edge.Label = incommingStatement.Type;

                    statementEdges.Add(edge);
                }
            }

            foreach (StatementViewModel outgoingStatement in OutgoingStatements)
            {
                if (!outgoingStatement.IsLoop)
                {
                    Vis.Node node = new Vis.Node(outgoingStatement.ObjectResource.Key.ToString(), 
                                                 CalculateLabelForStatementGraphNode(outgoingStatement.ObjectResource), 
                                                 3, 
                                                 "box");

                    node.Font = new Vis.NodeFontOption
                    {
                        Multi = true
                    };
                    statementNodes.Add(node);

                    Vis.Edge edge = new Vis.Edge(Key.ToString(), outgoingStatement.ObjectResource.Key.ToString());
                    edge.Arrows = new Vis.Arrows()
                    {
                        To = new Vis.ArrowsOptions
                        {
                            Enabled = true,
                            Type = "arrow"
                        }
                    };
                    edge.Label = outgoingStatement.Type;

                    statementEdges.Add(edge);
                }
            }


            _statementGraph = new Vis.NetworkData();

            _statementGraph.Nodes = statementNodes;
            _statementGraph.Edges = statementEdges;

        }

        public Vis.NetworkOptions StatementGraphOptions
        {
            get
            {
                return new Vis.NetworkOptions
                {
                    AutoResize = true,
                    Height = "700px",
                    Width = "100%",
                    Nodes = new Vis.NodeOption
                    {
                        BorderWidth = 1,
                        Color = new Vis.NodeColorType
                        {
                            Background = "#AAAAAA"
                        }
                    },
                    Layout = new Vis.LayoutOptions
                    {
                        Hierarchical = new Vis.HierarchicalOptions
                        {
                            Enabled = true,
                            Direction = "UD",
                            SortMethod = "directed"
                        }
                    },
                    Physics = new Vis.PhysicsOptions
                    {
                        //Enabled = false,
                        HierarchicalRepulsion = new Vis.HierarchicalRepulsionOption
                        {
                            AvoidOverlap = 1
                        }
                    },
                    Interaction = new Vis.InteractionOptions
                    {
                        Multiselect = false
                    },
                    Manipulation = new Vis.ManipulationOptions
                    {
                        Enabled = false
                    }
                };
            }
        }

        private string CalculateLabelForStatementGraphNode(ResourceViewModel resourceViewModel)
        {

            string result = "";

            //if (resourceViewModel.Icon != "")
            //{
            //    result += "<b>" + Icon + "</b>";
            //}
            //else
            //{
            result += "[" + resourceViewModel.Type + "]\r\n";
            //}

            if (!string.IsNullOrEmpty(resourceViewModel.Stereotype))
            {

                result += resourceViewModel.Stereotype + "\r\n";
            }

            result += "<b>" + resourceViewModel.Title;

            if (!string.IsNullOrEmpty(resourceViewModel.ClassifierName))
            {

                result += " :" + resourceViewModel.ClassifierName;
            }

            result += "</b>";

            if (!string.IsNullOrEmpty(resourceViewModel.LegacyType))
            {
                result += "\r\n(" + resourceViewModel.LegacyType + ")";
            }

            return result;

        }

        private ResourceRevisionViewModel _resourceRevisionViewModel = null;

        public ResourceRevisionViewModel ResourceRevisionViewModel
        {
            get
            {
                if (_resourceRevisionViewModel == null)
                {
                    _resourceRevisionViewModel = new ResourceRevisionViewModel(MetadataReader, DataReader, DataWriter, Key);
                }
                return _resourceRevisionViewModel;
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
