// Copyright (c) 2007 - 2010 Glenn Jones
// Refactored by Daniel Correia (2012)

namespace UfXtract
{
    using System;
    using System.Web;
    using System.Collections;
    using HtmlAgilityPack;

    using UfXtract.Describers;

    /// <summary>
    /// Loads a webpage and parse the results
    /// </summary>
    public class UfWebRequest
    {
        private string _userAgent = "";
        private Urls _parsedUrls = new Urls();
        private UfDataNode _data = new UfDataNode();
        private ArrayList _formatDescriberArray = new ArrayList();
        private UfFormatDescriber _formatDescriber = new UfFormatDescriber();

        /// <summary>
        /// Loads a webpage and parse the results
        /// </summary>
        public UfWebRequest() {}

        /// <summary>
        /// Loads a single Html pages and does a microformat parse
        /// </summary>
        /// <param name="url">The Url of the webpage to be pasred</param>
        /// <param name="formatDescriber">A format describer for microformat to be parsed</param>
        public void Load(string url, UfFormatDescriber formatDescriber)
        {
            _formatDescriber = formatDescriber;

            try {
                if (url != string.Empty) {
                    // Check for issues with url
                    url = url.Trim();
                    url = HttpUtility.UrlDecode(url);

                    UfWebPage webPage = LoadHtmlDoc(url);

                    if (webPage != null)
                    {
                        Url urlReport = new Url
                        {
                            Address = webPage.Url, 
                            Status = webPage.StatusCode
                        };

                        _parsedUrls.Add(urlReport);
                        DateTime started = DateTime.Now;

                        if (webPage.StatusCode == 200 && webPage.Html != null)
                            ParseUf(webPage.Html, url, formatDescriber, false, urlReport);

                        if (webPage.StatusCode != 200)
                            throw (new Exception("Could not load url: " + url + " " + webPage.StatusCode));


                        DateTime ended = DateTime.Now;
                        urlReport.LoadTime = ended.Subtract(started);
                        Urls.Add(urlReport);
                    }

                } else {
                    throw new Exception("No Url given");
                }

            } catch (Exception ex) {
                if (ex.Message == string.Empty) {
                    throw new Exception("Could not load Url: " + url);
                }
                throw;
            }
            
        }



        /// <summary>
        /// Loads a single Html pages and runs multiple microformat parses
        /// </summary>
        /// <param name="url">A full web page address</param>
        /// <param name="formatDescriberArray">An array of format describers</param>
        public void Load(string url, ArrayList formatDescriberArray)
        {
            this._formatDescriberArray = formatDescriberArray;

            try
            {
                if (url != string.Empty)
                {
                    url = url.Trim();
                    UfWebPage webPage = LoadHtmlDoc(url);
                    if (webPage != null)
                    {
                        Url urlReport = new Url();
                        urlReport.Address = webPage.Url;
                        DateTime started = DateTime.Now;
                        urlReport.Status = webPage.StatusCode;

                        // Process many time
                        foreach (UfFormatDescriber format in formatDescriberArray)
                        {
                            
                            _parsedUrls.Add(urlReport);

                            if (webPage.StatusCode == 200 && webPage.Html != null)
                                ParseUf(webPage.Html, webPage.Url, format, true, urlReport);

                            if (webPage.StatusCode != 200 )
                                throw (new Exception("Could not load url: " + url + " " + webPage.StatusCode));
                            
                        }

                        DateTime ended = DateTime.Now;
                        urlReport.LoadTime = ended.Subtract(started);
                        Urls.Clear();
                        Urls.Add(urlReport);
                    }
                }
                else
                {
                    throw (new Exception("No Url given"));
                }

            }
            catch (Exception ex)
            {
                if (ex.Message != string.Empty)
                    throw (new Exception(ex.Message));
                else
                    throw (new Exception("Could not load Url: " + url));
            }

        }


        /// <summary>
        /// Load a exteranl html document using webPage
        /// </summary>
        /// <param name="url">A full web page address</param>
        /// <returns></returns>
        private UfWebPage LoadHtmlDoc(string url)
        {
            UfWebPage webPage = new UfWebPage();
            if (_userAgent != "")
                webPage.UserAgent = _userAgent;

            try
            {
                if (url != string.Empty)
                {
                    // Check for issues with url
                    url = url.Trim();
                    if (url.StartsWith("http://") == false && url.StartsWith("https://") == false && url.StartsWith("file://") == false)
                        url = "http://" + url;

                    // Load page once
                    Uri uri = new Uri(url);
                    webPage.DocumentContentType = UfWebPage.ContentType.Html;
                    webPage.DocumentRequestType = UfWebPage.RequestType.Get;
                    webPage.Load(uri);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != string.Empty)
                    throw (new Exception(ex.Message));
                else
                    throw (new Exception("Could not load Url: " + url));
            }
            return webPage;
        }



        // Parse uf
        private void ParseUf(HtmlDocument htmlDoc, string url, UfFormatDescriber format, bool multiples, Url urlReport)
        {

            UfParse ufparse = new UfParse();
            ufparse.Load(htmlDoc, url, format);
            if (multiples)
                _data.Nodes.Add(ufparse.Data);
            else
                _data = ufparse.Data;

            urlReport.HtmlPageTitle = ufparse.HtmlPageTitle;

        }


        /// <summary>
        /// The useragent string to use for request. Default is Firefox 3.6
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

   
        /// <summary>
        /// Collection of parsed Urls
        /// </summary>
        public Urls Urls
        {
            get { return _parsedUrls; }
            set { _parsedUrls = value; }
        }

        /// <summary>
        /// The resulting data structure from a parse 
        /// </summary>
        public UfDataNode Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// The FormatDescriber used for single parse
        /// </summary>
        public UfFormatDescriber FormatDescriber
        {
            get { return _formatDescriber; }
            set { _formatDescriber = value; }
        }

        /// <summary>
        /// The FormatDescriber used for single parse
        /// </summary>
        public ArrayList FormatDescriberArray
        {
            get { return _formatDescriberArray; }
            set { _formatDescriberArray = value; }
        }


    }

}
