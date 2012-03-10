// Copyright (c) 2007 - 2010 Glenn Jones
// Refactored by Daniel Correia (2012)

namespace UfXtract.Describers
{
    using System.Xml.Serialization;

    /// <summary>
    /// Microformats format description
    /// </summary>
    [XmlRoot("UfFormatDescriber")]
    public class UfFormatDescriber
    {
        private string _name = string.Empty;
        private string _description = string.Empty;
        private FormatTypes _type = FormatTypes.Elemental;
        private UfElementDescriber _baseElement = new UfElementDescriber();

        /// <summary>
        /// Microformats format description
        /// </summary>
        public UfFormatDescriber() { }

        /// <summary>
        /// The name of the microformats formet
        /// </summary>
        [XmlAttribute("name")] 
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The description of the microformats formet
        /// </summary>
        [XmlIgnoreAttribute] 
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The base element
        /// </summary> 
        [XmlElement("base-element")]
        public UfElementDescriber BaseElement
        {
            get { return _baseElement; }
            set { 
                _baseElement = value;

                if (_baseElement.CompoundName == "") {
                    _baseElement.RootElement = true;
                }
            }
        }

        /// <summary>
        /// The type of microformats format
        /// </summary>
        [XmlElement("type")]
        public FormatTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Microformats format type
        /// </summary>
        public enum FormatTypes
        {
            /// <summary>
            /// Specifies a simple format type
            /// </summary>
            Elemental,

            /// <summary>
            /// Specifies a compount format type
            /// </summary>
            Compound
        }
    }
   
}
