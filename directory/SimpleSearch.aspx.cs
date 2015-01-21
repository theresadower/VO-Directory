
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

using System.Xml;
using System.Xml.Serialization;
using ivoa.altVOTable;

namespace registry
{
	public class SimpleSearch : System.Web.UI.Page
	{
        private static string registryIdentity = (string)System.Configuration.ConfigurationManager.AppSettings["registryIdentity"];

		private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "text/xml";
            status stat = new status();

            try
            {
                Object responseBody = null;
                
                stat =  HandleRequest(Request, ref responseBody);
                if (stat.responseCode == 200 && responseBody != null)
                {

                    //if (response.GetType() == typeof(string)) //simplest case
                    //{
                    //    Response.Write((string)response);
                    //}
                    //else //or it has not already been stringified
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        //add namespaces here as needed
                        XmlSerializer serializer = new XmlSerializer(responseBody.GetType());
                        XmlTextWriter xw = new XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8);
                        serializer.Serialize(xw, responseBody, ns);
                        xw.Close();
                    }
                 }
            }
            catch (Exception ex)
            {
                stat.responseCode = 500;
                stat.responseMessage = "Server Error. " + ex.Message;
            }

             if (stat.responseCode != 200)
                Response.ContentType = "text/plain";
             Response.StatusCode = stat.responseCode;
             Response.StatusDescription = stat.responseMessage;

            //Response.End();
        }

        //todo: scrub for bad SQL?
        internal status HandleRequest(HttpRequest Request, ref Object responseBody)
        {
            status stat = new status();

            System.Collections.Specialized.NameValueCollection input = Request.QueryString;
            if (Request.RequestType == "POST")
                input = Request.Form;
            string restPath = Request.PathInfo;
            if (restPath.Length > 1)
                restPath = restPath.Substring(1);
            restPath = restPath.ToUpper().Trim();

            if (restPath == "IDENTITY")
            {
                stat = GetIdentity(ref responseBody);
            }
            else if (restPath.StartsWith("RESOURCE"))
            {
                if (Request.QueryString.Count == 1)
                {
                    stat = GetResource(Request.QueryString[0], ref responseBody);
                }   
                else
                {
                    stat.responseCode = 400;
                    stat.responseMessage = "Bad or Missing Argument: resource identifier";
                }
           }
            else if (restPath.StartsWith("SEARCH"))
            {
                stat = GetSearchResults(Request.QueryString, ref responseBody);
            }
            else
            {
                stat.responseCode = 400;
                stat.responseMessage = "Bad Argument: " + restPath;
            }

            return stat;
        }

        private status GetIdentity(ref Object responseBody) //adapted from OAI
        {
            status stat = new status();

            Registry reg = new Registry();
            XmlDocument[] reses = reg.QueryRIResourceXMLDoc("ivoid = '" + registryIdentity + "' and rstat = 1");
            if (reses.Length > 0)
            {
                responseBody = reses[0];
                //todo: translate to simple search format.
            }
            else
            {
                stat.responseCode = 500;
                stat.responseMessage = "Cannot Find Server Identity Record";
            }
            return stat;
        }

        private status GetResource(string identifier, ref Object responseBody) //adapted from OAI
        {
            status stat = new status();
            Registry reg = new Registry();
            System.Xml.XmlDocument[] reses = reg.QueryRIResourceXMLDocAllResources("resource.ivoid='" + identifier + "'", true, true);
            if (reses.Length > 0 )
            {
                responseBody = reses[0];
                //todo: translate to simple search format.
            }
            else
            {
                stat.responseCode = 204;
                stat.responseMessage = "No records matched";
            }

            return stat;
        }

        private status GetSearchResults(System.Collections.Specialized.NameValueCollection args, ref Object responseBody)
        {
            string dbQuery = string.Empty;
            SimpleSearchArgs sa = new SimpleSearchArgs();
            status stat = sa.BuildSearchArgs(args); //can set statuscode to badargs
            if (Response.StatusCode == 200) 
            {
                //temp query:

                Registry reg = new Registry();
                System.Xml.XmlDocument[] reses = reg.SQLQueryRI10ResourceXMLDoc(sa.BuildSearchString());
                if (reses.Length > 0)
                {
                    responseBody = reses;
                }
 
                //handle paginated queries / session state?

                if (responseBody == null)
                {
                    stat.responseCode = 204;
                    stat.responseMessage = "No records matched";
                }
            }
            return stat;
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion
    }
    class status
    {
        public int responseCode = 200;
        public string responseMessage = string.Empty;
    };

    class SimpleSearchArgs
    {
        public ArrayList keywords = new ArrayList();
        public Hashtable parameterKeywords = new Hashtable();
        public int max = 0;
        public int from = 0;
        public bool orValues = false;
        public bool identifiersOnly = false;

        internal status BuildSearchArgs(System.Collections.Specialized.NameValueCollection args)
        {
            status stat = new status();
            foreach (string key in args)
            {
                if (key == "keywords" || key == "keyword")
                {
                    string argval = args[key];

                    Regex regexBySpaces = new Regex(@"[^\s""]+|""[^""]*""");
                    Regex regexByColon = new Regex(@"[^:""]+|""[^""]*""");

                    MatchCollection matches = regexBySpaces.Matches(argval);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string match = matches[i].Groups[0].Value;
                        MatchCollection parameters = regexByColon.Matches(match);
                        if (parameters.Count < 2)
                            keywords.Add(match);
                        else
                        {
                            //each param value may have more colons in it, we just wanted the first one as a delimiter.
                            string param = (string)parameters[0].Groups[0].Value.ToUpper();
                            parameterKeywords.Add(param, match.Substring(param.Length + 1));

                            //todo: allowable keywords
                        }
                    }
                }
                else if (key == "orValues")
                {
                    orValues = true;
                }
                else if (key == "identifiersOnly")
                {
                    identifiersOnly = true;
                }
                else if (key == "max")
                {
                    max = Convert.ToInt32(args[key]);
                }
                else if (key == "from")
                {
                    from = Convert.ToInt32(args[key]);
                }
                else
                {
                    stat.responseCode = 400;
                    stat.responseMessage = "Bad Argument: " + key;
                }
            }

            return stat;
        }

        //todo: handle paginated queries
        internal string BuildSearchString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" SELECT ");
            if (max > 0)
            {
                sb.Append(" TOP ");
                sb.Append(max);
            }
            sb.Append(" XML FROM RESOURCE ");

            if (keywords.Count > 0)
            {
                BuildFreetextMatchString(ref sb);
            }
            if (parameterKeywords.Count > 0)
            {
                BuildParameterMatchString(ref sb);
            }
            

            return sb.ToString();
        }

        internal void BuildFreetextMatchString(ref StringBuilder sb) //adapted from sqlhelper
        {
            //todo - rank 'or' case. test.
            if (this.orValues)
            {
                sb.Append(" WHERE [rstat] = 1 and (");

                sb.Append("contains (xml,'");
                for (int k = 0; k < this.keywords.Count; k++)
                {
                    if (k > 0) sb.Append(getLogical());
                    sb.Append(" \"" + this.keywords[k] + "\" ");
                }
                sb.Append("')");
            }
            else
            {
                bool canFullTextRank = true;
                for (int k = 0; k < this.keywords.Count; k++)
                {
                    string key = (string)this.keywords[k];
                    //some special characters break words in fulltext search.
                    if (key.IndexOf('/') >= 0 ||
                        key.IndexOf('&') >= 0 ||
                        key.IndexOf(' ') >= 0)
                    {
                        canFullTextRank = false;
                    }
                }
                if (canFullTextRank)
                {
                    sb.Append(" INNER JOIN CONTAINSTABLE(resource, xml, '");
                    for (int k = 0; k <this.keywords.Count; k++)
                    {
                        if (k > 0) sb.Append(" AND ");
                        sb.Append(this.keywords[k]);
                    }
                    sb.Append("') AS search ON resource.pkey = search.[KEY] WHERE resource.[rstat] = 1 and resource.validationLevel > 1 ORDER BY RANK DESC");
                }
                else
                {
                    sb.Append(" WHERE [rstat] = 1 and validationLevel > 1 ");

                    for (int k = 0; k < this.keywords.Count; k++)
                    {
                        if (k > 0) sb.Append(getLogical());
                        string key = (string)this.keywords[k];

                        //This is a common case where we can trick the fulltext index int
                        //finding the item we need by removing quotes and allowing the word breaker to 
                        //separate things.
                        if (key.Contains("ivo://"))
                        {
                            sb.Append(" contains (xml,'*" + key + "* ')");
                        }
                        //some special characters break words in fulltext search.
                        //do this the slow way.
                        else if (key.IndexOf('/') >= 0 ||
                                 key.IndexOf('&') >= 0)
                        {
                            //best we can do for &, also throws off 'like'
                            string temp = " xml like '%" + key + "%'";
                            sb.Append(temp.Replace('&', '%'));
                        }
                        else
                        {
                            sb.Append(" contains (xml,'\"*" + key + "*\" ')");
                        }
                    }
                }
            }
        }

        private string getLogical()
        {
            if (this.orValues)
                return " OR ";
            else
                return " AND ";
        }

        internal void BuildParameterMatchString(ref StringBuilder sb)
        {
            int whereIndex = sb.ToString().IndexOf("WHERE");
            int insertIndex = sb.ToString().IndexOf("ORDER BY");
            if (insertIndex == -1)
                insertIndex = sb.Length;

            StringBuilder clauseBuilder = new StringBuilder("");

            foreach (DictionaryEntry de in parameterKeywords)
            {
                if ((string)de.Key == "IDENTIFIER")
                {
                    clauseBuilder.Append(" ivoid = '" + de.Value + "' ");
                }
                //todo: other valid params, append logical operator, etc.
              
            }

            if (whereIndex > -1)
            {
                sb = sb.Insert(insertIndex, getLogical() + clauseBuilder.ToString());              
            }
            else
            {
                sb = sb.Insert(insertIndex, " WHERE [rstat] = 1 AND validationLevel > 1 AND (" + clauseBuilder.ToString() + ')');
            }          
        }
    };
}
