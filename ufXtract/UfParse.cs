//Copyright (c) 2007 - 2010 Glenn Jones

using System;
using System.Web;
using System.Net;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using UfXtract.Utilities;

namespace UfXtract
{
    using UfXtract.Describers;

    /// <summary>
    /// The main parser functions.
    /// </summary>
    public class UfParse
    {

        //Copyright (c) 2007 Glenn Jones

        private HtmlDocument document;
        private string url = "";
        private string baseUrl = "";
        private UfFormatDescriber formatDescriber = new UfFormatDescriber();
        private UfDataNode data = new UfDataNode();
        private HtmlNode startNode = null;
        private string htmlPageTitle = "";
  

        public UfParse(){ }


        #region "Load functions"
        //-----------------------------------------------------------------------


        /// <summary>
        /// Load and parse a Html string.
        /// </summary>
        /// <param name="htmlString">Html string</param>
        /// <param name="url">A Url for relative path operations</param>
        /// <param name="formatDescriber">The microformat format describer</param>
        public void Load(string htmlString, UfFormatDescriber formatDescriber)
        {
            Load(htmlString, "", formatDescriber);
        }


        /// <summary>
        /// Load and parse a Html document.
        /// </summary>
        /// <param name="document">HtmlAgilityPack Htmldocument object</param>
        /// <param name="formatDescriber">The microformat format describer</param>
        public void Load(HtmlDocument document, UfFormatDescriber formatDescriber)
        {
            Load(document, "", formatDescriber);
        }


        /// <summary>
        /// Load and parse a Html string.
        /// </summary>
        /// <param name="htmlString">Html string</param>
        /// <param name="url">A Url for relative path operations</param>
        /// <param name="formatDescriber">The microformat format describer</param>
        public void Load(string htmlString, string url, UfFormatDescriber formatDescriber)
        {
            // Temp fix xhtml strict issue
            htmlString = htmlString.Replace("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">", "");
            htmlString = htmlString.Replace("<meta content=\"text/html; charset => utf-8\" http-equiv=\"Content-Type\" />", "");

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlString);
            this.Load(document, url, formatDescriber);
        }


        /// <summary>
        /// Load and parse a Html document.
        /// </summary>
        /// <param name="document">HtmlAgilityPack Htmldocument object</param>
        /// <param name="url">The source Url of the document</param>
        /// <param name="formatDescriber">The microformat format describer</param>
        public void Load(HtmlDocument document, string url, UfFormatDescriber formatDescriber)
        {

            if (document == null)
                throw new ArgumentNullException("document");
            
            this.url = url;
            this.formatDescriber = formatDescriber;
            this.document = document;

            // Add in the whole html string from the page into the top data node
            data.OuterHtml = this.document.DocumentNode.OuterHtml;

            HtmlNodeCollection nodes;

            this.baseUrl = FindDocumentNodeAttributeValue("//html", "xml:base");
            this.baseUrl = FindDocumentNodeAttributeValue("//body", "xml:base");
            this.baseUrl = FindDocumentNodeAttributeValue("//base", "href");

            // Find the html page title
            nodes = this.document.DocumentNode.SelectNodes("//title");
            if (nodes != null)
                foreach (HtmlNode node in nodes)
                    this.htmlPageTitle = node.InnerText;

      
            
            // Start with document node
            this.startNode = document.DocumentNode;

            //// Find any fragment select
            //// <a name="profile"> html nodes </a>
            if (url != "")
            {
                Uri uri = new Uri(url);
                string frag = uri.Fragment;
                if (frag != string.Empty)
                {
                    try
                    {
                        // A name based fragment selection
                        nodes = this.document.DocumentNode.SelectNodes("//a[@name='" + frag.Replace("#", "") + "']");
                        if (nodes != null)
                        {
                            this.startNode = nodes[0];
                        }
                        else
                        {
                            // ID based fragment selection
                            nodes = this.document.DocumentNode.SelectNodes("//*[@id='" + frag.Replace("#", "") + "']");
                            this.startNode = nodes[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Could not find name fragment" + frag);
                    }
                }
            }


            // Starts recursion
            ParseUfElement(this.startNode, this.FormatDescriber.BaseElement, this.Data, true);

            UfHelpers.RunNodeOptimization(this.Data);

        }



        /// <summary>
        /// Finds compound uf between parent containing node and found reference node
        /// </summary>
        private bool HasCompound(HtmlNode containingNode, HtmlNode node, bool hasCompound)
        {
            // Not the same object
            if (System.Object.ReferenceEquals(containingNode, node) == false)
            {
                // test
                if (node.Attributes["class"] != null)
                {
                    string classvalue = node.Attributes["class"].Value.ToLower();
                    if (classvalue.IndexOf("vcard") > -1 || classvalue.IndexOf("vevent") > -1)
                    {
                        hasCompound = true;
                    }
                }

                // call test on parent node
                if ((object)node.ParentNode != null && (object)containingNode != null) 
                    hasCompound = HasCompound(containingNode, node.ParentNode, hasCompound);

            }
            return hasCompound;
        }


        private string FindDocumentNodeAttributeValue(string xPath, string attribute)
        {
            string output = "";
            HtmlNodeCollection nodes;
            //Find base the xml:base attribute and sets base url
            nodes = this.document.DocumentNode.SelectNodes(xPath);
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    HtmlAttribute relHref = node.Attributes[attribute];
                    if (relHref != null)
                        output = relHref.Value.ToLower();
                }
            }
            return output;
        }


        #endregion




        #region "Parse functions"
       


        private void ParseUfElement(HtmlNode baseNode, UfElementDescriber ufElement, UfDataNode ufData, bool ufTopLevel)
        {

            // Select nodes with required attribute: class, rel or rev
            HtmlNodeCollection nodes = baseNode.SelectNodes(".//@" + ufElement.Attribute );
     
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {

                    // Load the attribute class, rel or rev
                    HtmlAttribute att = node.Attributes[ufElement.Attribute];
                    HtmlAttribute compoundAtt = node.Attributes[ufElement.CompoundAttribute];
                    if (att != null)
                    {
                        // We are dealing with elemental uf like XFN
                        if (ufElement.AttributeValues.Count > 0)
                        {
                            bool found = false;
                            // Search for a mulitple attribute values ie friend or contact in rel
                            foreach (UfAttributeValueDescriber avd in ufElement.AttributeValues)
                            {
                                if (UfHelpers.FindAttributeValue(att.Value.ToLower(), avd.Name))
                                    found = true;

                            }
                            if (found)
                            {
                                // Adds the Html from which uf is parsed
                                // ufData.OuterHtml = node.OuterHtml;
                                ParseUfElementValue(node, ufElement, ufData);
                            }
                        }
                        else
                        {
                            bool found = false;

                            // Search for a single attribute values ie hcard in class
                            if (UfHelpers.FindAttributeValue(att.Value.ToLower(), ufElement.Name) && ufElement.CompoundName == string.Empty)
                                found = true;

                            // Search for a dual attribute values
                            // This is for compound structures, ie reviewer in hreview which is a hcard
                            if (UfHelpers.FindAttributeValue(att.Value.ToLower(), ufElement.Name) && UfHelpers.FindAttributeValue(compoundAtt.Value.ToLower(), ufElement.CompoundName))
                                found = true;

                            if (found)
                            {

                                if (HasCompound(baseNode, node.ParentNode, false) == false || ufElement.RootElement == true)
                                {
                                    HtmlNodeCollection includeRefNodes = null;

                                    includeRefNodes = node.SelectNodes(".//a[@class[contains(.,'include')]]");
                                    if (includeRefNodes != null)
                                    {
                                        foreach (HtmlNode includeRefNode in includeRefNodes)
                                        {
                                            string link = GetAttributeValue(includeRefNode, "href");
                                            if (link.StartsWith("#"))
                                            {
                                                link = link.Replace("#", "");
                                                HtmlNodeCollection includeNodes = node.SelectNodes("//*[@id='" + link + "']");
                                                if (includeNodes != null && includeNodes.Count > 0)
                                                    node.AppendChild(HtmlNode.CreateNode("<div>" + includeNodes[0].OuterHtml + "</div>"));
                                            }
                                        }

                                    }

                                    includeRefNodes = node.SelectNodes(".//object[@class[contains(.,'include')]]");
                                    if (includeRefNodes != null)
                                    {
                                        foreach (HtmlNode includeRefNode in includeRefNodes)
                                        {
                                            string link = GetAttributeValue(includeRefNode, "data");
                                            if (link.StartsWith("#"))
                                            {
                                                link = link.Replace("#", "");
                                                HtmlNodeCollection includeNodes = node.SelectNodes("//*[@id='" + link + "']");
                                                if (includeNodes != null && includeNodes.Count > 0)
                                                    node.AppendChild(HtmlNode.CreateNode("<div>" + includeNodes[0].OuterHtml + "</div>"));
                                            }
                                        }
                                    }

                                    // For TD
                                    // Finds table head include pattern and appends node collection
                                    if (node.Name == "td" && GetAttributeValue(node, "headers") != string.Empty)
                                    {
                                        string link = GetAttributeValue(node, "headers");
                                        string[] itemArray = new string[1];
                                        itemArray[0] = link;
                                        if (link.IndexOf(' ') > -1)
                                            itemArray = link.Split(' ');

                                        for (int i = 0; i < itemArray.Length; i++)
                                        {
                                            HtmlNodeCollection includeNodes = null;
                                            includeNodes = this.startNode.SelectNodes("//node()[@id='" + itemArray[i].Trim() + "']");
                                            if (includeNodes != null && includeNodes.Count > 0)
                                            {
                                                // Appends fresh node to avoid overload issues
                                                foreach (HtmlNode childNode in includeNodes)
                                                    node.AppendChild(HtmlNode.CreateNode("<div>" + childNode.OuterHtml + "</div>"));

                                            }
                                        }
                                    }

                                    // For TR
                                    // Finds table head include pattern and appends node collection
                                    if (node.Name == "tr")
                                    {
                                        foreach (HtmlNode child in node.ChildNodes)
                                        {
                                            if (child.Name == "td" && GetAttributeValue(child, "headers") != string.Empty)
                                            {
                                                string link = GetAttributeValue(child, "headers");
                                                string[] itemArray = new string[1];
                                                itemArray[0] = link;
                                                if (link.IndexOf(' ') > -1)
                                                    itemArray = link.Split(' ');

                                                for (int i = 0; i < itemArray.Length; i++)
                                                {
                                                    HtmlNodeCollection includeNodes = null;
                                                    includeNodes = this.startNode.SelectNodes("//node()[@id='" + itemArray[i].Trim() + "']");
                                                    if (includeNodes != null && includeNodes.Count > 0)
                                                    {
                                                        // Appends fresh node to avoid overload issues
                                                        foreach (HtmlNode childNode in includeNodes)
                                                            child.AppendChild(HtmlNode.CreateNode("<div>" + childNode.OuterHtml + "</div>"));

                                                    }
                                                }
                                            }
                                        }
                                    }


                                    // Adds the Html from which uf is parsed
                                    foreach (HtmlNode childNode in node.ChildNodes)
                                        ufData.OuterHtml += childNode.OuterHtml;
           

                                    // Recursion
                                    if (ufElement.Multiples || ufElement.ConcatenateValues)
                                        ParseUfElementValue(node, ufElement, ufData);
                                    else
                                        // Dont add a second data node for a format decription that does not support either
                                        // multiples or concatenation of values
                                        if (ufData.Nodes.Exists(ufElement.Name) == false)
                                            ParseUfElementValue(node, ufElement, ufData);

                                }
                            }
                        }
                    }
                }
            }
        }







        private void ParseUfElementValue(HtmlNode baseNode, UfElementDescriber ufElement, UfDataNode ufData)
        {

            // Create a single data node for whatever data insertion is needed.
            UfDataNode ufd = new UfDataNode();
            if(ufElement.CompoundName != string.Empty)
                ufd.ParentNodeNames = ufData.ParentNodeNames + ufElement.CompoundName + " ";
            else
                ufd.ParentNodeNames = ufData.ParentNodeNames + ufElement.Name + " ";


           ufd.ElementId = GetAttributeValue(baseNode, "id");


            // A parent node in the data schema
            if (ufElement.Elements.Count > 0)
            {
                if (ufElement.CompoundName == string.Empty)
                {
                    // Add a emtpy structural node
                    ufd.Name = ufElement.Name;
                }
                else
                {
                    // This is for compound structures, ie reviewer in hreview is a hcard
                    // Need to find a second attribute value to do this
                    HtmlAttribute att = baseNode.Attributes[ufElement.CompoundAttribute];
                    if (att != null)
                    {
                        if (UfHelpers.FindAttributeValue(att.Value.ToLower(), ufElement.CompoundName))
                        {
                            // Add a emtpy structural node using compound name
                            ufd.Name = ufElement.CompoundName;
                        }
                    }
                }

                // Recursion through the dom structure
                foreach (UfElementDescriber ufChildElement in ufElement.Elements)
                    ParseUfElement(baseNode, ufChildElement, ufd, false);

            }


            // A value needs to be found
            if (ufElement.Type != UfElementDescriber.PropertyTypes.None)
            {

                // Find child nodes with "value" or "value-title" classes
                HtmlNodeCollection valueNodes = null;
                HtmlNodeCollection valueTitleNodes = null;

                // The value pattern
                if (ufElement.Elements["value"] == null && ufElement.Name != "value")
                {
                    valueNodes = baseNode.SelectNodes(".//*[contains(concat(' ', @class, ' '),' value ')]");
                }


                // The value-title pattern is only allow for some property types ie dates 
                // or name properties ie type, duration, geo, latitude and longitude
                if (ufElement.Type == UfElementDescriber.PropertyTypes.Date || 
                    ufElement.Name == "type" || 
                    ufElement.Name == "duration" || 
                    ufElement.Name == "geo" || 
                    ufElement.Name == "latitude" || 
                    ufElement.Name == "longitude")
                {
                    valueTitleNodes = baseNode.SelectNodes(".//*[contains(concat(' ', @class, ' '),' value-title ')]");
                }






                if (ufElement.Type == UfElementDescriber.PropertyTypes.UrlTextAttribute || ufElement.Type == UfElementDescriber.PropertyTypes.UrlTextTag || ufElement.Type == UfElementDescriber.PropertyTypes.UrlText)
                {

                    string text = UfHelpers.HtmlToText(baseNode, false);
                    string link = UfHelpers.GetAbsoluteUrl(GetAttributeValue(baseNode, "href"), this.baseUrl, url);
                    string att = GetAttributeValue(baseNode, ufElement.Attribute);
                    ufd.Name = ufElement.Name;

                    UfDataNode ufd1 = new UfDataNode();
                    UfDataNode ufd2 = new UfDataNode();
                    UfDataNode ufd3 = new UfDataNode();

                    ufd1.Name = "text";
                    ufd1.Value = text;
                    ufd.Nodes.Add(ufd1);

                    ufd2.Name = "link";
                    ufd2.Value = link;
                    ufd.Nodes.Add(ufd2);

                    // Add the attribute value used for XFN like structures
                    if (ufElement.Type == UfElementDescriber.PropertyTypes.UrlTextAttribute)
                    {
                        ufd3.Name = ufElement.Attribute;
                        ufd3.Value = att;
                        ufd.Nodes.Add(ufd3);
                    }

                    // Add the tag element of the url
                    if (ufElement.Type == UfElementDescriber.PropertyTypes.UrlTextTag)
                    {
                        ufd3.Name = "tag";
                        ufd3.Value = UfHelpers.GetTagFromUrl(link);
                        ufd.Nodes.Add(ufd3);
                    }

                    if (ufElement.CompoundName == string.Empty)
                    {
                        ufData.Nodes.Add(ufd);
                    }
                    else
                    {
                        HtmlAttribute att1 = baseNode.Attributes[ufElement.CompoundAttribute];
                        if (att1 != null)
                        {
                            if (UfHelpers.FindAttributeValue(att1.Value.ToLower(), ufElement.CompoundName))
                            {
                                ufd.Name = ufElement.CompoundName;
                                ufData.Nodes.Add(ufd);
                            }
                        }
                    }
                }

                // The value excerpting pattern
                else if (valueNodes != null)
                {
                    string text = string.Empty ;
                    foreach (HtmlNode node in valueNodes)
                    {
                        if (node.Name == "img" || node.Name == "area")
                        {
                            if (ufElement.Type == UfElementDescriber.PropertyTypes.Date)
                                text += GetAttributeValue(node, "title").Replace(" ", "") + " ";
                            else
                                text += GetAttributeValue(node, "title");
                        }
                        else if (node.Name == "abbr")
                        {
                            if (ufElement.Type == UfElementDescriber.PropertyTypes.Date)
                                text += GetAttributeValue(node, "title").Replace(" ", "") + " ";
                            else
                                text += GetAttributeValue(node, "title");
                        }
                        else
                        {
                            if (ufElement.Type == UfElementDescriber.PropertyTypes.Date)
                                text += UfHelpers.HtmlToText(node, false).Replace(" ", "") + " ";
                            else
                                text += UfHelpers.HtmlToText(node, false) + " ";
                        }
                    }

                    if(ufElement.Type == UfElementDescriber.PropertyTypes.Date)
                    {
                        // Take the fagmented bits and create a true ISODateTime string
                        ISODateTime isoDateTime = new ISODateTime();
                        text = isoDateTime.ParseUFFragmented(text);
                    }

                    ufd.Name = ufElement.Name;
                    ufd.Value = text.Trim();
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // The value-title excerpting pattern
                else if (valueTitleNodes != null)
                {
                    string text = GetAttributeValue(valueTitleNodes[0], "title");
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Url from "a" or "link"
                else if ((baseNode.Name == "a" || baseNode.Name == "link") && GetAttributeValue(baseNode, "href") != string.Empty && ufElement.Type == UfElementDescriber.PropertyTypes.Url)
                {
                    string link = UfHelpers.GetAbsoluteUrl(GetAttributeValue(baseNode, "href"), this.baseUrl, url);
                    ufd.Name = ufElement.Name;
                    ufd.Value = link;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Url from "img"
                else if ((baseNode.Name == "img" || baseNode.Name == "area") && GetAttributeValue(baseNode, "src") != string.Empty && ufElement.Type == UfElementDescriber.PropertyTypes.Url)
                {
                    string link = UfHelpers.GetAbsoluteUrl(GetAttributeValue(baseNode, "src"), this.baseUrl, url);
                    ufd.Name = ufElement.Name;
                    ufd.Value = link;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Email from "a" or "link"
                else if (baseNode.Name == "a" && GetAttributeValue(baseNode, "href") != string.Empty && ufElement.Type == UfElementDescriber.PropertyTypes.Email)
                {
                    string address = UfHelpers.CleanEmailAddress(GetAttributeValue(baseNode, "href"));
                    ufd.Name = ufElement.Name;
                    ufd.Value = address;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Tel from "object"
                else if (baseNode.Name == "object" && (GetAttributeValue(baseNode, "data") != "") && ufElement.Name == "tel")
                {
                    UfHelpers.TelOptimization(ufd, GetAttributeValue(baseNode, "data"));
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Date from "time"
                else if (baseNode.Name == "time" && GetAttributeValue(baseNode, "datetime") != "" && ufElement.Type == UfElementDescriber.PropertyTypes.Date)
                {
                    string text = GetAttributeValue(baseNode, "datetime");
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Date from "abbr"
                else if (baseNode.Name == "abbr" && GetAttributeValue(baseNode, "title") != string.Empty && ufElement.Type == UfElementDescriber.PropertyTypes.Date)
                {
                    string text = GetAttributeValue(baseNode, "title");
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Text from "abbr"
                else if (baseNode.Name == "abbr" || baseNode.Name == "acronym" && GetAttributeValue(baseNode, "title") != string.Empty)
                {
                    string text = GetAttributeValue(baseNode, "title");
                    ufd.Name = ufElement.Name;

                    // This is for geo been used as a location in hcalandar
                    if (ufElement.CompoundName != string.Empty)
                        ufd.Name = ufElement.CompoundName;

                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Text from "input"
                else if (baseNode.Name == "input" && GetAttributeValue(baseNode, "value") != string.Empty)
                {
                    string text = GetAttributeValue(baseNode, "value");
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Tel from "area"
                else if (baseNode.Name == "area" && (GetAttributeValue(baseNode, "href") != "") && ufElement.Name == "tel")
                {
                    UfHelpers.TelOptimization(ufd, GetAttributeValue(baseNode, "href"));
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Text and url from "area"
                else if (baseNode.Name == "area" && (GetAttributeValue(baseNode, "href") != string.Empty || GetAttributeValue(baseNode, "alt") != string.Empty))
                {
                    if ((ufElement.Type == UfElementDescriber.PropertyTypes.Url || ufElement.Type == UfElementDescriber.PropertyTypes.Email) && GetAttributeValue(baseNode, "href") != string.Empty)
                    {
                        string text = GetAttributeValue(baseNode, "href");

                        if (ufElement.Type == UfElementDescriber.PropertyTypes.Email)
                            text = UfHelpers.CleanEmailAddress(text);

                        if (ufElement.Type == UfElementDescriber.PropertyTypes.Url)
                            text = UfHelpers.GetAbsoluteUrl(text, this.baseUrl, url);

                        ufd.Name = ufElement.Name;
                        ufd.Value = text;
                        AddNewDateNode(baseNode, ufData, ufd, ufElement);
                    }
                    else if (GetAttributeValue(baseNode, "alt") != string.Empty)
                    {
                        string text = GetAttributeValue(baseNode, "alt");
                        ufd.Name = ufElement.Name;
                        ufd.Value = text;
                        AddNewDateNode(baseNode, ufData, ufd, ufElement);
                    }

                }

                // Url/Image from "object"
                else if (baseNode.Name == "object" && GetAttributeValue(baseNode, "data") != string.Empty && (ufElement.Type == UfElementDescriber.PropertyTypes.Url || ufElement.Type == UfElementDescriber.PropertyTypes.Image))
                {
                    string text = UfHelpers.GetAbsoluteUrl(GetAttributeValue(baseNode, "data"), this.baseUrl, url);
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Image from "img" or "area"
                else if ((baseNode.Name == "img" || baseNode.Name == "area") && GetAttributeValue(baseNode, "src") != string.Empty && ufElement.Type == UfElementDescriber.PropertyTypes.Image)
                {
                    string text = UfHelpers.GetAbsoluteUrl(GetAttributeValue(baseNode, "src"), this.baseUrl, url);
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                // Text from "img" longdesc attribute
                else if (baseNode.Name == "img" && GetAttributeValue(baseNode, "longdesc") != string.Empty)
                {
                    string text = GetAttributeValue(baseNode, "longdesc");
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement); ;
                }


                // Text from "img" alt attribute
                else if (baseNode.Name == "img" && GetAttributeValue(baseNode, "alt") != string.Empty)
                {
                    string text = GetAttributeValue(baseNode, "alt");
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }


                // Text for type/value structures with no found children
                else if (ufElement.NodeType == UfElementDescriber.StructureTypes.TypeValuePair)
                {
                    // if no chidren nodes ie type/value are found use text
                    // the calls for a children node type and value are alway both thier parent
                    if (ufd.Nodes.Count == 0)
                    {
                        // Add text from node value
                        string text = UfHelpers.HtmlToText(baseNode, false);
                        ufd.Name = ufElement.Name;
                        ufd.Value = text;
                        AddNewDateNode(baseNode, ufData, ufd, ufElement);
                    }
                    else
                    {
                        // Add child type/value pair
                        ufd.Name = ufElement.Name;
                        AddNewDateNode(baseNode, ufData, ufd, ufElement);
                    }
                }

                // Text from Html node collect
                else if (ufElement.Type == UfElementDescriber.PropertyTypes.FormattedText)
                {
                    string text = UfHelpers.HtmlToText(baseNode, true);
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }

                else 
                {
                    // Text from node value
                    //string text = FindValuePattern(baseNode, ufElement);
                    //if(text == string.Empty)
                    //    text = HtmlToText(baseNode, false);

                    string text = UfHelpers.HtmlToText(baseNode, false);
                    ufd.Name = ufElement.Name;
                    ufd.Value = text;
                    AddNewDateNode(baseNode, ufData, ufd, ufElement);
                }
            }
            else
            {
                AddNewDateNode(baseNode, ufData, ufd, ufElement);
            }
          
        }


        /// <summary>
        /// Adds a new data node to the tree
        /// </summary>
        /// <param name="ufData">Parent node</param>
        /// <param name="ufNewDataNode">Node to be added</param>
        /// <param name="ufElement">The uF element describer</param>
        private void AddNewDateNode(HtmlNode baseNode, UfDataNode ufData, UfDataNode ufNewDataNode, UfElementDescriber ufElement)
        {
            if (IsDuplicateNode(ufData, ufNewDataNode) == false)
            {
                ufNewDataNode.OuterHtml = baseNode.OuterHtml;

                // This function deal both with the concatenation of multiple values
                // and the validation of multiple flag

                // If the structure is a value/type pair change the insert point
                if (ufElement.Elements["value"] != null && ufElement.Elements["type"] != null)
                {
                    // Add to child value node
                    UfDataNode ufdatanode = new UfDataNode("value", ufNewDataNode.Value);
                    ufNewDataNode.Nodes.Add(ufdatanode);
                    ufNewDataNode.Value = "";
                }

                // Concatenation of values
                if (ufElement.ConcatenateValues)
                {
                    // Create a new node or add to the existing one
                    if (ufData.Nodes[ufNewDataNode.Name] == null)
                        ufData.Nodes.Add(ufNewDataNode);
                    else
                        ufData.Nodes[ufNewDataNode.Name].Value += ufNewDataNode.Value;

                }
                else if (ufElement.Multiples == false)
                {
                    // Singluar - only take first instance
                    if (ufData.Nodes[ufNewDataNode.Name] == null)
                        ufData.Nodes.Add(ufNewDataNode);
                
                }
                else if (ufElement.Multiples == true)
                {
                    // Multiples
                    ufData.Nodes.Add(ufNewDataNode);
                }
            }
        }


        /// <summary>
        /// Finds double value entry
        /// This can happen as xPath will find the legal use of more than one class/rel attribute on a single element
        /// </summary>
        /// <param name="ufData">Parent node</param>
        /// <param name="ufNewDataNode">Node to be added</param>
        private bool IsDuplicateNode(UfDataNode ufData, UfDataNode ufNewDataNode)
        {
            if (ufData.Nodes.Count > 0)
            {
                UfDataNode lastNode = ufData.Nodes[ufData.Nodes.Count-1];
                if (ufNewDataNode.Value != ""  && 
                    ufNewDataNode.Value == lastNode.Value 
                    && ufNewDataNode.ParentNodeNames == lastNode.ParentNodeNames)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Returns the text value of a node
        /// </summary>
        /// <param name="node">HtmlAgilityPack html node</param>
        private string GetNodeValue(HtmlNode node)
        {
            return UfHelpers.HtmlToText(node, false);
        }


        /// <summary>
        /// Returns the value of a given node attribute
        /// </summary>
        /// <param name="node">HtmlAgilityPack html node</param>
        /// <param name="attName">The attribute name</param>
        private string GetAttributeValue(HtmlNode node, string attName)
        {
            string output = string.Empty;
            HtmlAttribute att = node.Attributes[attName];
            if (att != null)
                output =  att.Value;
            return output;
        }






        #endregion


     


        #region "Properties"
        //-----------------------------------------------------------------------


        /// <summary>
        /// Gets and sets the Url of the document been parsed
        /// </summary>
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        /// <summary>
        /// Gets and sets the base Url of the document been parsed as definded in a basehref tag
        /// </summary>
        public string UrlBase
        {
            get { return this.baseUrl; }
            set { this.baseUrl = value; }
        }


        /// <summary>
        /// Gets and sets the microformats format describer 
        /// </summary>
        public UfFormatDescriber FormatDescriber
        {
            get { return this.formatDescriber; }
            set { this.formatDescriber = value; }
        }


        /// <summary>
        /// Gets and sets the resulting data structure from a parse 
        /// </summary>
        public UfDataNode Data
        {
            get { return this.data; }
            set { this.data = value; }
        }


        /// <summary>
        /// Gets the Html page title
        /// </summary>
        public string HtmlPageTitle
        {
            get { return htmlPageTitle; }
        }

   


        #endregion

    }
}
