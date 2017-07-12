using System.ComponentModel;
using System.Web.Services;
using System.Text;

namespace Publishing
{
	[WebService(Namespace="http://www.us-vo.org")]//
    public class WhoAmI : System.Web.Services.WebService
	{
        private static UserManagement userManager = new UserManagement();

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

        protected WhoamiInfo thewho = null;
        protected WhoamiInfo Whoami
        {
            get
            {
                if (thewho == null) thewho = new WhoamiInfo(Context.Request);
                return thewho;
            }
        }

        [WebMethod]
        //To match Portal whoami in mashup.asmx, without a heavier included JSON library.
        //{ "FirstName":"Anonymous","LastName":"","EZID":"anonymous","IsInternal":"defaultInt","Department":"defaultDept","Email":"","IP":"172.17.7.238"}
        public void whoami()
        {
			Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            StringBuilder sb = new StringBuilder("{ ");
            sb.Append("\"First Name\":\"");
            sb.Append(Whoami.FirstName);
            sb.Append("\",\"LastName\":\"");
            sb.Append(Whoami.LastName);

            sb.Append("\",\"EZID\":\"");
            sb.Append(Whoami.EZID);
            sb.Append("\",\"IsInternal\":\"");
            sb.Append(Whoami.IsInternal);
            sb.Append("\",\"Department\":\"");
            sb.Append(Whoami.Department);
            sb.Append("\",\"Email\":\"");
            sb.Append(Whoami.Email);
            sb.Append("\",\"IP\":\"");
            sb.Append(Whoami.IP);
            sb.Append("\" }");

            Context.Response.Write(sb.ToString());
            Context.Response.Flush();
        }

        [WebMethod]
        public void getauthinfo()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            string response = string.Empty;
            if (Whoami.EZID != null && Whoami.EZID != "anonymous")
            {

                string[] userAuthData = FetchUserAuthData(Whoami.EZID);
                if (userAuthData != null) 
                {
                    if (userAuthData.Length > 0)
                    {
                        string defaultAuth = userAuthData[0];
                        if (defaultAuth.Length > 0)
                        {
                            bool bHasOrg = userManager.CheckAuthForOrg(defaultAuth);
                            if (bHasOrg)
                                response = JSONSuccess(defaultAuth, null, null);

                            else
                                response = JSONSuccess(defaultAuth, defaultAuth, "Organisation record required.");
                        }
                        else
                            response = JSONFailure(new string[] { "Invalid default authority resource for user " + Whoami.EZID });
                    }
                    else
                        response = JSONSuccess();
                }
                else
                    response = JSONFailure(new string[] { "No publishing authorization data for user " + Whoami.EZID });
            }
            else
                response = JSONFailure();

            Context.Response.Write(response);
            Context.Response.Flush();
        }

        internal string[] FetchUserAuthData(string EZID)
        {
            string[] userAuths = new string[] { };
            long ukey = UserManagement.GetUserKey(EZID);
            if (ukey != 0)
            {
                string[] errors = userManager.GetUserAuths(ukey, ref userAuths);
                if (errors.Length == 0)
                {
                    return userAuths;
                }
            }
            return null;
        }

        private string JSONSuccess(string defaultauth = null, string details = null, string message = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ 'success': true");
            if (defaultauth != null)
                sb.Append(", 'defaultauth': '" + defaultauth + '\'');
            if (details != null)
                sb.Append(", 'details': '" + details + '\'');
            if (message != null)
                sb.Append(", 'message': '" + message + '\'');

            sb.Append(" }");
            return sb.ToString();
        }

        private string JSONFailure(string[] errors = null)
        {
            StringBuilder sb = new StringBuilder();
            if (errors != null && errors.Length > 0)
            {
                sb.Append("{ 'success': false, 'errors': { 'reason': '");
                foreach (string error in errors)     //remove JSON-incompatible characters here.
                {
                    sb.Append(error.Replace('\'', '`').Replace("\r\n", " ") + ' ');
                }
                sb.Append("' }}");
            }
            else
                sb.Append("{ 'success': false, 'errors': { 'reason': 'Login failed. User unknown to this system.' }}");
            return sb.ToString();
        }
    }
}
