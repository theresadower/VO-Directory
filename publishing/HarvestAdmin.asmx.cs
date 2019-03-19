using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace Publishing
{
	[WebService(Namespace="http://www.us-vo.org")]//

    public class WhoAmI : System.Web.Services.WebService
	{
		public WhoAmI()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]
        public void whoami()
        {
			//////////////////////////////////////////////////////////////
			// Write the Whoami Response as Json back out to the client
			//////////////////////////////////////////////////////////////
			Context.Response.ClearHeaders();
			Context.Response.ClearContent();
			Context.Response.ContentType = "text/javascript";
			Context.Response.Write(Whoami.ToJson());
			Context.Response.Flush();
}
		
	}
}
