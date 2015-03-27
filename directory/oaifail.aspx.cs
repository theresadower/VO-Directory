using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Net;
using oai;
using log4net;

namespace nvo.oai
{
	// Class for simulating errors, used for development debug

	public class oaifail : System.Web.UI.Page
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(oaifail));

        private enum Verbs : int
        {
            GetRecord = 0,
            Identify,
            ListIdentifiers,
            ListMetadataFormats,
            ListRecords,
            ListSets,

            NumVerbs
        };
        private static ArrayList verbs = new ArrayList();
        private static ArrayList requiredArgs = new ArrayList();
        private static ArrayList optArgs = new ArrayList();

		private void Page_Load(object sender, System.EventArgs e)
        {
            #region Load Parameter Checking Information (once)
            lock (verbs.SyncRoot)
            {
                if (verbs.Count == 0)
                {
                    // Generate tables of allowed and optional arguments for each verb.
                    // Since we are calling a web service based on these parameters, we
                    // want to check them here where we can return proper OAI errors and not
                    // waste the expensive WS call.
                    verbs.AddRange( new String[(int)Verbs.NumVerbs] );
                    verbs[(int)Verbs.GetRecord] = "GetRecord";
                    verbs[(int)Verbs.Identify] = "Identify";
                    verbs[(int)Verbs.ListIdentifiers] = "ListIdentifiers";
                    verbs[(int)Verbs.ListMetadataFormats] = "ListMetadataFormats";
                    verbs[(int)Verbs.ListRecords] = "ListRecords";
                    verbs[(int)Verbs.ListSets] = "ListSets";

                    requiredArgs.AddRange( new String[verbs.Count] );
                    requiredArgs[(int)Verbs.GetRecord] = new ArrayList();
                    ((ArrayList)requiredArgs[(int)Verbs.GetRecord]).Add("identifier");
                    ((ArrayList)requiredArgs[(int)Verbs.GetRecord]).Add("metadataPrefix");

                    requiredArgs[(int)Verbs.ListIdentifiers] = new ArrayList();
                    ((ArrayList)requiredArgs[(int)Verbs.ListIdentifiers]).Add("metadataPrefix");

                    requiredArgs[(int)Verbs.ListRecords] = new ArrayList();
                    ((ArrayList)requiredArgs[(int)Verbs.ListRecords]).Add("metadataPrefix");


                    optArgs.AddRange(new String[verbs.Count]);

                    //Theoretically we should handle resumptionTokens on ListSets if we have many sets.
                    //However, with only one set in the DB, there is no reason that we should
                    //hand out a resumptionToken for ListSets. Therefore, any resumptionToken
                    //passed into ListSets must be invalid.

                    optArgs[(int)Verbs.ListIdentifiers] = new ArrayList();
                    ((ArrayList)optArgs[(int)Verbs.ListIdentifiers]).Add("from");
                    ((ArrayList)optArgs[(int)Verbs.ListIdentifiers]).Add("until");
                    ((ArrayList)optArgs[(int)Verbs.ListIdentifiers]).Add("set");
                    ((ArrayList)optArgs[(int)Verbs.ListIdentifiers]).Add("resumptionToken");

                    optArgs[(int)Verbs.ListMetadataFormats] = new ArrayList();
                    ((ArrayList)optArgs[(int)Verbs.ListMetadataFormats]).Add("identifier");

                    optArgs[(int)Verbs.ListRecords] = new ArrayList();
                    ((ArrayList)optArgs[(int)Verbs.ListRecords]).Add("from");
                    ((ArrayList)optArgs[(int)Verbs.ListRecords]).Add("until");
                    ((ArrayList)optArgs[(int)Verbs.ListRecords]).Add("set");
                    ((ArrayList)optArgs[(int)Verbs.ListRecords]).Add("resumptionToken");
                }
            };
            #endregion

            #region Check Parameters

            System.Collections.Specialized.NameValueCollection input = Request.QueryString;
            if (Request.RequestType == "POST")
            {
                input = Request.Form;
            }
            string verb = input["verb"];

			if(verb.Equals("Identify")){
				filterXML("oai.aspx?verb=Identify");
				return;

			}else{
				Response.Output.Write("Blah");
			}
            #endregion
        }

		// This allows schemaLocation output to be handled for OAI
		void filterXML(string partURL)
		{
			string fullURL = Request.Url.GetLeftPart(System.UriPartial.Authority)+Request.ApplicationPath + "/"+ partURL;

			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(fullURL); 
			// Sends the HttpWebRequest and waits for the response. 
			HttpWebResponse resp = null;

			try
			{
				resp = (HttpWebResponse)wr.GetResponse(); 
				// Gets the stream associated with the response.
				Stream receiveStream = resp.GetResponseStream();
				Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
				StreamReader stream = new StreamReader( receiveStream, encode);
				string line = null;
				while ((line = stream.ReadLine())!=null) 
				{
					line = line.Replace(" schemaLocation", " xsi:schemaLocation");
                    //line = line.Replace("http://www.ivoa.net/xml/SkyNode/v0.2", "http://www.ivoa.net/xml/OpenSkyNode/OpenSkyNode-v0.2.xsd");
					Response.Output.WriteLine(line);
				}
			}
			catch (Exception ex)
			{
				Response.Output.WriteLine(" Failed "+fullURL+" " +ex.Message+" : "+ ex.StackTrace);
			}

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

		// Checking for Bad input parameters or argument for OAI
		/*private bool checkParam(string str)
		{
			string lstr = str.ToLower();

			// Added by .NET so okay, ignore these
			if (lstr.StartsWith("asp")|| lstr.StartsWith("all") || lstr.StartsWith("app") 
				|| lstr.StartsWith("auth") || lstr.StartsWith("log") || lstr.StartsWith("rem")  
				|| lstr.StartsWith("cert") || lstr.StartsWith("cont")
				|| lstr.StartsWith("gate") || lstr.StartsWith("http")  
				|| lstr.StartsWith("ins")|| lstr.StartsWith("loc")  
				|| lstr.StartsWith("path") || lstr.StartsWith("quer")  
				|| lstr.StartsWith("req") || lstr.StartsWith("scri") 
                || lstr.StartsWith("__utm") //google analytics.
				|| lstr.StartsWith("serv") || lstr.StartsWith("url") ) return true;

			for (int i=0;i<validParams.Length;i++)
			{
				if (validParams[i]==lstr) return true;	
			}

			return false;
		}*/
		
	}
}

