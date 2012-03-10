// Copyright (c) 2007 - 2010 Glenn Jones
// Refactored by Daniel Correia (2012)

namespace UfXtract.Describers
{
    using System.Collections;
    using System.Collections.Generic;

    ///<summary>
    /// A set of standard microformats
    ///</summary>
    public class UfFormats
    {

        private UfFormats() { }

        #region "Compound formats"
        //-----------------------------------------------------------------------


        public static UfFormatDescriber HCard()
        {

            // Construct base hCard
            UfFormatDescriber uFormat = BaseHCard();

            // Add first level of agent
            UfFormatDescriber agenthCard = BaseHCard();
            agenthCard.BaseElement.CompoundName = "agent";
            agenthCard.BaseElement.CompoundAttribute = "class";
            agenthCard.BaseElement.ConcatenateValues = false;
            agenthCard.BaseElement.Multiples = true;
            uFormat.BaseElement.Elements.Add(agenthCard.BaseElement);
            agenthCard.BaseElement.RootElement = false;

            // Add second level of agent
            UfFormatDescriber agenthCard2 = BaseHCard();
            agenthCard2.BaseElement.CompoundName = "agent";
            agenthCard2.BaseElement.CompoundAttribute = "class";
            agenthCard2.BaseElement.ConcatenateValues = false;
            agenthCard2.BaseElement.Multiples = true;
            agenthCard.BaseElement.Elements.Add(agenthCard2.BaseElement);
            agenthCard2.BaseElement.RootElement = false;

            // FallBack text only
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("agent", false, true, UfElementDescriber.PropertyTypes.Text));

            return uFormat;

        }

        private static UfFormatDescriber BaseHCard()
        {

            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hCard";
            uFormat.Description = "hCard";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;

            uFormat.BaseElement = new UfElementDescriber("vcard", false, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("fn", false, false, UfElementDescriber.PropertyTypes.Text));

            uFormat.BaseElement.Elements.Add(Name().BaseElement);
            uFormat.BaseElement.Elements.Add(Adr().BaseElement);
            uFormat.BaseElement.Elements.Add(Email().BaseElement);
            uFormat.BaseElement.Elements.Add(Tel().BaseElement);

            UfFormatDescriber cat = Tag();
            cat.BaseElement.CompoundName = "category";
            cat.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(cat.BaseElement);

            UfFormatDescriber org = Org();
            org.BaseElement.Multiples = true;
            uFormat.BaseElement.Elements.Add(org.BaseElement);

            UfFormatDescriber geo = Geo();
            geo.BaseElement.Multiples = false;
            uFormat.BaseElement.Elements.Add(geo.BaseElement);

            //uFormat.BaseElement.Elements.Add(new UfElementDescriber("agent", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("bday", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("class", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("key", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("label", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("logo", false, true, UfElementDescriber.PropertyTypes.Image));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("mailer", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("nickname", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("note", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("photo", false, true, UfElementDescriber.PropertyTypes.Image));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("rev", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("role", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("sort-string", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("sound", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("title", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("tz", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("uid", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("url", false, true, UfElementDescriber.PropertyTypes.Url));

            return uFormat;

        }

        public static UfFormatDescriber HReview()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hreview";
            uFormat.Description = "A review";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("hreview", true, true, UfElementDescriber.PropertyTypes.None);

            UfFormatDescriber item = HCard();
            item.BaseElement.CompoundName = "item";
            item.BaseElement.CompoundAttribute = "class";
            item.BaseElement.Multiples = false;
            uFormat.BaseElement.Elements.Add(item.BaseElement);

            UfFormatDescriber reviewer = HCard();
            reviewer.BaseElement.CompoundName = "reviewer";
            reviewer.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(reviewer.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("dtreviewed", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("summary", false, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("description", false, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, false, "product,business,event,person,place,website,url"));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("rating", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("best", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("worst", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("version", false, false, UfElementDescriber.PropertyTypes.Text));

            uFormat.BaseElement.Elements.Add(License().BaseElement); // license
            uFormat.BaseElement.Elements.Add(Tag().BaseElement); // tag

            UfFormatDescriber bookmark = Bookmark();
            bookmark.BaseElement.CompoundName = "permalink";
            bookmark.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(bookmark.BaseElement);

            return uFormat;

        }

        public static UfFormatDescriber HCalendar()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hcalendar";
            uFormat.Description = "A event";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("vevent", true, true, UfElementDescriber.PropertyTypes.None);

            UfFormatDescriber cat = Tag();
            cat.BaseElement.CompoundName = "category";
            cat.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(cat.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("summary", true, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("description", false, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("class", false, false, UfElementDescriber.PropertyTypes.Text));

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("dtstart", true, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("dtend", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("dtstamp", true, false, UfElementDescriber.PropertyTypes.Date));

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("duration", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("status", false, false, UfElementDescriber.PropertyTypes.Text));

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("uid", true, false, UfElementDescriber.PropertyTypes.Uid));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("url", false, false, UfElementDescriber.PropertyTypes.Url));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("last-modified", false, false, UfElementDescriber.PropertyTypes.Url));


            UfFormatDescriber geo = Geo();
            geo.BaseElement.Multiples = false;
            uFormat.BaseElement.Elements.Add(geo.BaseElement);

   
            
            // Location can be Text, hCard, Adr or Geo
            // Looks for the highest resolution frist
            // --------------

            UfFormatDescriber locationhCard = HCard();
            locationhCard.BaseElement.CompoundName = "location";
            locationhCard.BaseElement.CompoundAttribute = "class";
            locationhCard.BaseElement.ConcatenateValues = false;
            uFormat.BaseElement.Elements.Add(locationhCard.BaseElement);

            UfFormatDescriber locationAdr = Adr();
            locationAdr.BaseElement.CompoundName = "location";
            locationAdr.BaseElement.CompoundAttribute = "class";
            locationAdr.BaseElement.ConcatenateValues = false;
            uFormat.BaseElement.Elements.Add(locationAdr.BaseElement);

            UfFormatDescriber locationGeo = Geo();
            locationGeo.BaseElement.CompoundName = "location";
            locationGeo.BaseElement.CompoundAttribute = "class";
            locationGeo.BaseElement.ConcatenateValues = false;
            uFormat.BaseElement.Elements.Add(locationGeo.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("location", false, false, false, UfElementDescriber.PropertyTypes.Text));

            // --------------


            // Optional extensions to spec
            UfFormatDescriber attendee = HCard();
            attendee.BaseElement.CompoundName = "attendee";
            attendee.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(attendee.BaseElement);

            UfFormatDescriber contact = HCard();
            contact.BaseElement.CompoundName = "contact";
            contact.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(contact.BaseElement);

            UfFormatDescriber organizer = HCard();
            organizer.BaseElement.CompoundName = "organizer";
            organizer.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(organizer.BaseElement);

            uFormat.BaseElement.Elements.Add(RRule().BaseElement);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("rdate", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("tzid", false, false, UfElementDescriber.PropertyTypes.Text));

            return uFormat;

        }

        public static UfFormatDescriber HAtom()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hatom";
            uFormat.Description = "A feed";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("hfeed", true, true, UfElementDescriber.PropertyTypes.None);

            UfFormatDescriber item = HAtomItem();
            uFormat.BaseElement.Elements.Add(item.BaseElement);

            uFormat.BaseElement.Elements.Add(Tag().BaseElement);
            //uFormat.BaseElement.Elements.Add(BuildhAtomItem().BaseElement);

            return uFormat;

        }

        public static UfFormatDescriber HAtomItem()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hentry";
            uFormat.Description = "A entry or feed item";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("hentry", true, true, UfElementDescriber.PropertyTypes.None);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("entry-title", true, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("entry-content", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("entry-summary", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("updated", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("published", false, false, UfElementDescriber.PropertyTypes.Date));

            UfFormatDescriber author = HCard();
            author.BaseElement.CompoundName = "author";
            author.BaseElement.CompoundAttribute = "class";
            author.BaseElement.Multiples = true;
            uFormat.BaseElement.Elements.Add(author.BaseElement);

            uFormat.BaseElement.Elements.Add(Tag().BaseElement);

            UfFormatDescriber mk = Bookmark();
            mk.BaseElement.Mandatory = true;
            uFormat.BaseElement.Elements.Add(mk.BaseElement);

            return uFormat;
        }


        public static UfFormatDescriber HNewsItem()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hentry";
            uFormat.Description = "A entry or feed item gor hNews which supersets hAtom";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("hentry", true, true, UfElementDescriber.PropertyTypes.None);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("entry-title", true, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("entry-content", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("entry-summary", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("updated", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("published", false, false, UfElementDescriber.PropertyTypes.Date));

            UfFormatDescriber author = HCard();
            author.BaseElement.CompoundName = "author";
            author.BaseElement.CompoundAttribute = "class";
            author.BaseElement.Multiples = true;
            uFormat.BaseElement.Elements.Add(author.BaseElement);

            uFormat.BaseElement.Elements.Add(Tag().BaseElement);

            UfFormatDescriber mk = Bookmark();
            mk.BaseElement.Mandatory = true;
            uFormat.BaseElement.Elements.Add(mk.BaseElement);

            UfFormatDescriber sourceOrg = HCard();
            sourceOrg.BaseElement.CompoundName = "source-org";
            sourceOrg.BaseElement.CompoundAttribute = "class";
            sourceOrg.BaseElement.Multiples = false;
            sourceOrg.BaseElement.Mandatory = true;
            uFormat.BaseElement.Elements.Add(sourceOrg.BaseElement);

            UfFormatDescriber byline = HCard();
            byline.BaseElement.CompoundName = "byline";
            byline.BaseElement.CompoundAttribute = "class";
            byline.BaseElement.Multiples = false;
            uFormat.BaseElement.Elements.Add(byline.BaseElement);

            uFormat.BaseElement.Elements.Add(Geo().BaseElement);
            uFormat.BaseElement.Elements.Add(Principles().BaseElement);

            // Dateline can be Text, hCard, Adr, Geo or Date
            // Looks for the highest resolution frist
            // --------------

            UfFormatDescriber datelinehCard = HCard();
            datelinehCard.BaseElement.CompoundName = "dateline";
            datelinehCard.BaseElement.CompoundAttribute = "class";
            datelinehCard.BaseElement.ConcatenateValues = false;
            uFormat.BaseElement.Elements.Add(datelinehCard.BaseElement);

            UfFormatDescriber datelineAdr = Adr();
            datelineAdr.BaseElement.CompoundName = "dateline";
            datelineAdr.BaseElement.CompoundAttribute = "class";
            datelineAdr.BaseElement.ConcatenateValues = false;
            uFormat.BaseElement.Elements.Add(datelineAdr.BaseElement);

            UfFormatDescriber datelineGeo = Geo();
            datelineGeo.BaseElement.CompoundName = "dateline";
            datelineGeo.BaseElement.CompoundAttribute = "class";
            datelineGeo.BaseElement.ConcatenateValues = false;
            uFormat.BaseElement.Elements.Add(datelineGeo.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("dateline", false, false, false, UfElementDescriber.PropertyTypes.Date));

            // --------------


            uFormat.BaseElement.Elements.Add(hNewsLicenseItem().BaseElement);


            return uFormat;
        }


        public static UfFormatDescriber hNewsLicenseItem()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "item";
            uFormat.Description = "Used in a hNews hEntry";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("item", true, true, UfElementDescriber.PropertyTypes.None);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("item-url", true, false, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(ItemLicense().BaseElement);

            UfFormatDescriber attribution = HCard();
            attribution.BaseElement.CompoundName = "attribution";
            attribution.BaseElement.CompoundAttribute = "class";
            attribution.BaseElement.Multiples = true;
            uFormat.BaseElement.Elements.Add(attribution.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("attribution", true, false, UfElementDescriber.PropertyTypes.UrlText));

            return uFormat;
        }



        public static UfFormatDescriber HResume()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hresume";
            uFormat.Description = "A resume";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("hresume", true, true, UfElementDescriber.PropertyTypes.None);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("summary", false, false, UfElementDescriber.PropertyTypes.FormattedText));


            UfFormatDescriber edu = HCalendar();
            edu.BaseElement.CompoundName = "education";
            edu.BaseElement.CompoundAttribute = "class";
            UfFormatDescriber eduhCard = HCard();
            foreach (UfElementDescriber elementDescriber in eduhCard.BaseElement.Elements)
            {
                edu.BaseElement.Elements.Add(elementDescriber);
            }
            uFormat.BaseElement.Elements.Add(edu.BaseElement);


            UfFormatDescriber exp = HCalendar();
            exp.BaseElement.CompoundName = "experience";
            exp.BaseElement.CompoundAttribute = "class";
            UfFormatDescriber exphCard = HCard();
            foreach (UfElementDescriber elementDescriber in exphCard.BaseElement.Elements)
            {
                exp.BaseElement.Elements.Add(elementDescriber);
            }
            uFormat.BaseElement.Elements.Add(exp.BaseElement);


            UfFormatDescriber aff = HCard();
            aff.BaseElement.CompoundName = "affiliation";
            aff.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(aff.BaseElement);

            UfFormatDescriber ski = Tag();
            ski.BaseElement.CompoundName = "skill";
            ski.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(ski.BaseElement);

            UfFormatDescriber con = HCard();
            con.BaseElement.CompoundName = "contact";
            con.BaseElement.CompoundAttribute = "class";
            con.BaseElement.Multiples = false;
            uFormat.BaseElement.Elements.Add(con.BaseElement);

            // Part of draft spec, used on Linked-in
            // http://microformats.org/wiki/hresume-skill-brainstorm
            UfFormatDescriber comp = Competency();
            comp.BaseElement.Multiples = true;
            uFormat.BaseElement.Elements.Add(comp.BaseElement);

            return uFormat;
        }



        public static UfFormatDescriber HRecipe()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hrecipe";
            uFormat.Description = "A recipe";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("hrecipe", true, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("fn", true, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("ingredient", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("yield", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("instructions", false, false, UfElementDescriber.PropertyTypes.FormattedText));
            //NOTE: Added 'directions' for AllRecipes, even though hRecipe spec says instructions
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("directions", false, false, UfElementDescriber.PropertyTypes.FormattedText)); 
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("duration", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("photo", false, true, UfElementDescriber.PropertyTypes.Image));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("summary", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("author", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("published", false, true, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("nutrition", false, true, UfElementDescriber.PropertyTypes.Date));

            uFormat.BaseElement.Elements.Add(Tag().BaseElement);
            return uFormat;
        }


        // This currently not used because of issue with type/value
        public static UfFormatDescriber Ingredient()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "Ingredient";
            uFormat.Description = "An ingredient";
            uFormat.BaseElement = new UfElementDescriber("ingredient", true, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("value", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, false, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }


        // This currently not used because of issue with type/value
        public static UfFormatDescriber Nutrition()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "Nutrition";
            uFormat.Description = "An nutrition";
            uFormat.BaseElement = new UfElementDescriber("nutrition", true, false, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("value", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, false, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        #endregion


        #region Elemental formats

        ///<summary>Gets the Format Describer for the XHTML Friends Network microformat</summary>
        ///<returns>The UfFormatDescriber for Xfn</returns>
        public static UfFormatDescriber Xfn()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "xfn",
                Description = "XHTML Friends Network, describes realtionships",
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            { 
                Name = "xfn", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlTextAttribute 
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");

            uFElement.AddAttributeValues(
                new Dictionary<string, string>
                    {
                        {"met", ""},
                        {"co-worker", ""},
                        {"colleague", ""},
                        {"muse", "" },
                        {"crush", "" },
                        {"date", "" },
                        {"sweetheart", "" },

                        {"co-resident", "neighbor" },
                        {"neighbor", "co-resident" },

                        {"child", "parent sibling spouse kin"},
                        {"parent", "child sibling spouse kin"},
                        {"sibling", "child parent spouse kin"},
                        {"spouse", "child parent sibling kin" },
                        {"kin", "child parent sibling spouse"},

                        {"contact", "acquaintance friend"},
                        {"acquaintance", "contact friend"},
                        {"friend", "contact friend"},

                        {"me", "contact acquaintance friend met co-worker colleague co-resident neighbor " 
                                + "child parent sibling spouse kin muse crush date sweetheart"}
                    }
                );

            return uFormat;
        }

        /// <summary>
        /// Gets the format describer for the NoFollow microformat
        /// </summary>
        /// <returns>The UfFormatDescriber for NoFollow relation</returns>
        public static UfFormatDescriber NoFollow()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "nofollow",
                Description = "Stops search engines following links",
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "nofollow", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("nofollow");

            return uFormat;
        }

        /// <summary>
        /// Gets the format describer for the License microformat
        /// </summary>
        /// <returns>The UfFormatDescriber for the License relation</returns>
        public static UfFormatDescriber License()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "license", 
                Description = "License", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "license", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;

            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("license");

            return uFormat;
        }

        /// <summary>
        /// Gets the format describer for the ItemLicense microformat
        /// </summary>
        /// <returns>The UfFormatDescriber for the Item License relation</returns>
        public static UfFormatDescriber ItemLicense()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "item-license",
                Description = "item-license used in hNews hEntry item",
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "item-license",
                Attribute = "rel",
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("item-license");

            return uFormat;
        }

        /// <summary>
        /// Gets the format describer for the Principles microformat
        /// </summary>
        /// <returns>The UfFormatDescriber for the Principles relation</returns>
        public static UfFormatDescriber Principles()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "principles", 
                Description = "Principles", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "principles", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;

            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("principles");

            return uFormat;
        }

        /// <summary>
        /// Gets the format describer for the Directory microformat
        /// </summary>
        /// <returns>The UfFormatDescriber for the Directory relation</returns>
        public static UfFormatDescriber Directory()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "directory", 
                Description = "directory", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "directory", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("directory");

            return uFormat;
        }

        /// <summary>
        /// Gets the format describer for the Home microformat
        /// </summary>
        /// <returns>The UfFormatDescriber for the Home relation</returns>
        public static UfFormatDescriber Home()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "home", 
                Description = "home", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "home", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("home");

            return uFormat;
        }

        public static UfFormatDescriber Payment()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "payment", 
                Description = "payment", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "payment",
                Attribute = "rel",
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("payment");

            return uFormat;
        }

        public static UfFormatDescriber Enclosure()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "enclosure", 
                Description = "enclosure", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "enclosure", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlText
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("enclosure");

            return uFormat;
        }

        public static UfFormatDescriber Tag()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "tag", 
                Description = "Tag", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "tag",
                Multiples = true,
                Attribute = "rel",
                Type = UfElementDescriber.PropertyTypes.UrlTextTag
            };

            uFormat.BaseElement = uFElement;
            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValue("tag");

            return uFormat;
        }

        public static UfFormatDescriber VoteLinks()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "votelinks", 
                Description = "Vote Links", 
                Type = UfFormatDescriber.FormatTypes.Elemental
            };

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlTextAttribute
            };

            uFormat.BaseElement = uFElement;

            uFElement.AllowedTags.Add("a", "link");
            uFElement.AddAttributeValues(
                new Dictionary<string, string>
                {
                    {"vote-for", "vote-abstain vote-against"},
                    {"vote-against", "vote-abstain vote-for"},
                    {"vote-abstain", "vote-for vote-against"}
                }
            );
            
            return uFormat;
        }

        public static UfFormatDescriber XFolk()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "xFolk",
                Description = "xFolk",
                Type = UfFormatDescriber.FormatTypes.Compound,
                BaseElement = new UfElementDescriber("xfolkentry", false, true, UfElementDescriber.PropertyTypes.None)
            };

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("taggedlink", true, false, UfElementDescriber.PropertyTypes.UrlText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("description", false, true, UfElementDescriber.PropertyTypes.Text));

            UfElementDescriber uFElement = new UfElementDescriber
            {
                Name = "tag", 
                Attribute = "rel", 
                Type = UfElementDescriber.PropertyTypes.UrlTextTag
            };

            uFElement.AllowedTags.Add("a", "link");
            uFElement.AttributeValues.Add(new UfAttributeValueDescriber("tag"));
            uFormat.BaseElement.Elements.Add(uFElement);

            return uFormat;
        }

        public static UfFormatDescriber Geo()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "geo",
                Description = "Location constructed of latitude and longitude",
                Type = UfFormatDescriber.FormatTypes.Compound,
                BaseElement = new UfElementDescriber("geo", false, true, UfElementDescriber.PropertyTypes.Text)
            };

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("latitude", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("longitude", false, false, UfElementDescriber.PropertyTypes.Text));

            return uFormat;
        }

        public static UfFormatDescriber Adr()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber
            {
                Name = "adr",
                Description = "Address",
                Type = UfFormatDescriber.FormatTypes.Compound,
                BaseElement = new UfElementDescriber("adr", false, true, UfElementDescriber.PropertyTypes.None)
            };

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, true, "work,home,pref,postal,dom,intl"));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("post-office-box", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("extended-address", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("street-address", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("locality", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("region", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("postal-code", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("country-name", false, false, UfElementDescriber.PropertyTypes.Text));

            return uFormat;
        }

        #endregion


        #region "Reusabe patterns"
        //-----------------------------------------------------------------------

        public static UfFormatDescriber Competency()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "competency";
            uFormat.Description = "Part of draft hResume spec";
            uFormat.BaseElement = new UfElementDescriber("competency", false, false, UfElementDescriber.PropertyTypes.None);
            
            UfFormatDescriber ski = Tag();
            ski.BaseElement.CompoundName = "skill";
            ski.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(ski.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("proficiency", false, false, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        public static UfFormatDescriber Org()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "org";
            uFormat.Description = "Organization";
            uFormat.BaseElement = new UfElementDescriber("org", true, false, UfElementDescriber.PropertyTypes.Text);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("organization-name", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("organization-unit", false, false, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        public static UfFormatDescriber Email()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "email";
            uFormat.Description = "Email address";
            uFormat.BaseElement = new UfElementDescriber("email", false, true, UfElementDescriber.PropertyTypes.Email);
            uFormat.BaseElement.NodeType = UfElementDescriber.StructureTypes.TypeValuePair;
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, true, "internet,x400,pref"));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("value", false, false, true, UfElementDescriber.PropertyTypes.Email));
            return uFormat;
        }

        public static UfFormatDescriber Tel()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "tel";
            uFormat.Description = "Telephone";
            uFormat.BaseElement = new UfElementDescriber("tel", false, true, UfElementDescriber.PropertyTypes.Text);
            uFormat.BaseElement.NodeType = UfElementDescriber.StructureTypes.TypeValuePair;
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, true, "voice,home,msg,work,pref,fax,cell,video,pager,bbs,modem,car,isdn,pcs"));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("value", false, false, true, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        public static UfFormatDescriber Name()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "n";
            uFormat.Description = "Name";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("n", false, false, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, true, "intl,postal,parcel,work,dom,home,pref"));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("honorific-prefix", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("given-name", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("additional-name", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("family-name", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("honorific-suffix", false, true, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        public static UfFormatDescriber Bookmark()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "bookmark";
            uFormat.Description = "Bookmark";
            uFormat.Type = UfFormatDescriber.FormatTypes.Elemental;

            UfElementDescriber uFElement = new UfElementDescriber();
            uFElement.Name = "bookmark";
            uFElement.AllowedTags.Add("a");
            uFElement.AllowedTags.Add("link");
            uFElement.Attribute = "rel";
            uFElement.Type = UfElementDescriber.PropertyTypes.UrlText;
            uFormat.BaseElement = uFElement;
            uFElement.AttributeValues.Add(new UfAttributeValueDescriber("bookmark"));
            return uFormat;
        }

        public static UfFormatDescriber RRule()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "rrule";
            uFormat.Description = "Recurring durations and dates";
            uFormat.BaseElement = new UfElementDescriber("rrule", false, false, UfElementDescriber.PropertyTypes.Text);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("freq", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("count", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("interval", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("until", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("bysecond", false, false, UfElementDescriber.PropertyTypes.Number));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("byminute", false, false, UfElementDescriber.PropertyTypes.Number));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("bymonthday", false, false, UfElementDescriber.PropertyTypes.Number));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("byyearday", false, false, UfElementDescriber.PropertyTypes.Number));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("byweekno", false, false, UfElementDescriber.PropertyTypes.Number));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("bymonth", false, false, UfElementDescriber.PropertyTypes.Number));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("byday", false, false, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        #endregion


        #region "POSH pattern"
        //-----------------------------------------------------------------------

        public static UfFormatDescriber NextPrevious()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "nextprevious";
            uFormat.Description = "The rel next previous design pattern";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;

            UfElementDescriber uFElement = new UfElementDescriber();
            uFElement.Name = "nextprevious";
            uFElement.AllowedTags.Add("a");
            uFElement.AllowedTags.Add("link");
            uFElement.Attribute = "rel";
            uFElement.Type = UfElementDescriber.PropertyTypes.UrlTextAttribute;
            uFormat.BaseElement = uFElement;

            uFElement.AttributeValues.Add(new UfAttributeValueDescriber("next", ""));
            uFElement.AttributeValues.Add(new UfAttributeValueDescriber("prev", ""));

            return uFormat;
        }

        public static UfFormatDescriber Me()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "me";
            uFormat.Description = "The finds rel=me without rest of xfn";
            uFormat.Type = UfFormatDescriber.FormatTypes.Elemental;

            UfElementDescriber uFElement = new UfElementDescriber();
            uFElement.Name = "me";
            uFElement.AllowedTags.Add("a");
            uFElement.AllowedTags.Add("link");
            uFElement.Attribute = "rel";
            uFElement.Type = UfElementDescriber.PropertyTypes.UrlText;
            uFormat.BaseElement = uFElement;

            uFElement.AttributeValues.Add(new UfAttributeValueDescriber("me", ""));


            return uFormat;
        }

        public static UfFormatDescriber HCardXFN()
        {

            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "hCard";
            uFormat.Description = "hCard";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;


            uFormat.BaseElement = new UfElementDescriber("vcard", false, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("fn", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(Name().BaseElement);
            uFormat.BaseElement.Elements.Add(Adr().BaseElement);
            uFormat.BaseElement.Elements.Add(Geo().BaseElement);
            uFormat.BaseElement.Elements.Add(Org().BaseElement);
            uFormat.BaseElement.Elements.Add(Email().BaseElement);
            uFormat.BaseElement.Elements.Add(Tel().BaseElement);

            UfFormatDescriber cat = Tag();
            cat.BaseElement.CompoundName = "category";
            cat.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(cat.BaseElement);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("agent", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("bday", false, false, UfElementDescriber.PropertyTypes.Date));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("class", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("key", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("label", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("logo", false, true, UfElementDescriber.PropertyTypes.Image));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("mailer", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("nickname", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("note", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("photo", false, true, UfElementDescriber.PropertyTypes.Image));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("rev", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("role", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("sort-string", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("sound", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("title", false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("tz", false, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("uid", false, false, UfElementDescriber.PropertyTypes.Uid));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("url", false, true, UfElementDescriber.PropertyTypes.Url));

            uFormat.BaseElement.Elements.Add(Xfn().BaseElement);

            return uFormat;

        }

        public static UfFormatDescriber TestSuite()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "test-suite";
            uFormat.Description = "The structure of a test suite containing a number of test fixtures";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("test-suite", true, true, UfElementDescriber.PropertyTypes.None);

            uFormat.BaseElement.Elements.Add(Test().BaseElement);

            return uFormat;
        }

        public static UfFormatDescriber TestFixture()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "test-fixture";
            uFormat.Description = "A test fixture";
            uFormat.Type = UfFormatDescriber.FormatTypes.Compound;
            uFormat.BaseElement = new UfElementDescriber("test-fixture", true, true, UfElementDescriber.PropertyTypes.None);

            uFormat.BaseElement.Elements.Add(new UfElementDescriber("summary", true, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("description", false, true, UfElementDescriber.PropertyTypes.FormattedText));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("format", true, false, UfElementDescriber.PropertyTypes.Text));

            uFormat.BaseElement.Elements.Add(Output().BaseElement);
            uFormat.BaseElement.Elements.Add(Assert().BaseElement);

            UfFormatDescriber history = HCalendar();
            history.BaseElement.CompoundName = "history";
            history.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(history.BaseElement);

            UfFormatDescriber author = HCard();
            author.BaseElement.CompoundName = "author";
            author.BaseElement.CompoundAttribute = "class";
            uFormat.BaseElement.Elements.Add(author.BaseElement);

            return uFormat;
        }

        public static UfFormatDescriber Assert()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "assert";
            uFormat.Description = "An Assert, part of a test fixture";
            uFormat.BaseElement = new UfElementDescriber("assert", false, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("test", false, false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("result", false, false, true, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("comment", false, false, true, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        public static UfFormatDescriber Output()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "output";
            uFormat.Description = "The output, part of a test fixture";
            uFormat.BaseElement = new UfElementDescriber("output", false, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("url", false, false, true, UfElementDescriber.PropertyTypes.Url));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("type", false, false, true, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }

        public static UfFormatDescriber Test()
        {
            UfFormatDescriber uFormat = new UfFormatDescriber();
            uFormat.Name = "test";
            uFormat.Description = "The location of a test fixture";
            uFormat.BaseElement = new UfElementDescriber("test", false, true, UfElementDescriber.PropertyTypes.None);
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("format", true, false, UfElementDescriber.PropertyTypes.Text));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("url", false, false, true, UfElementDescriber.PropertyTypes.Url));
            uFormat.BaseElement.Elements.Add(new UfElementDescriber("description", false, false, true, UfElementDescriber.PropertyTypes.Text));
            return uFormat;
        }


        #endregion

    }

    public static class ElementUtils 
    {
        public static void AddAttributeValue(this UfElementDescriber describer, string name, string excludeValues)
        {
            describer.AttributeValues.Add(new UfAttributeValueDescriber(name, excludeValues));
        }

        public static void AddAttributeValue(this UfElementDescriber describer, string name) {
            describer.AttributeValues.Add(new UfAttributeValueDescriber(name));
        }

        public static void AddAttributeValues(this UfElementDescriber describer, IDictionary<string, string> dictionary) 
        {
            foreach (var name in dictionary.Keys) {
                describer.AttributeValues.Add(new UfAttributeValueDescriber(name, dictionary[name]));
            }
        }

        public static void Add(this ArrayList list, params string[] values) 
        {
            foreach (var value in values) {
                list.Add(value);
            }
        }
    }
}
