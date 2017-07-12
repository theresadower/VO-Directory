using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Web;
using System.Web.Services;
using System.ComponentModel;
using System.Text;

using registry;
using OperationsManagement;

namespace Publishing
{
    [WebService(Namespace = "http://www.us-vo.org")]
    public partial class IngestResource : System.Web.Services.WebService
    {
        private static string sConnect = (string)System.Configuration.ConfigurationManager.AppSettings["SqlAdminConnection"];
        public static string getResourceStatement = @"select top 1 xml from Resource where ivoid=@Identifier and ([rstat] = 1 or [rstat] = 0 or [rstat] = 3 ) and ukey=@ukey order by [updated] desc";
        private static string appDir = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

        public static VOR_XML vorXML = new VOR_XML();
        private static UserManagement userManagement = new UserManagement();

        public IngestResource()
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
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
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

        private bool isLoggedIn()
        {
            if (Whoami != null && Whoami.EZID != null && Whoami.EZID != "anonymous")
                return true;
            return false;
        }

        [WebMethod]
        public void IngestFormResource()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            if (!isLoggedIn())
            {
                Context.Response.Write(JSONFailure("Not Logged In"));
            }
            else
            {
                string[] userAuths = new string[] { };
                long userKey = UserManagement.GetUserKey(Whoami.EZID);
                string[] errors = userManagement.GetUserAuths(userKey, ref userAuths);
                if (errors.Length > 0)
                    Context.Response.Write(JSONFailure(errors));
                else if (Context.Request.InputStream == null || Context.Request.InputStream.Length == 0)
                {
                    Context.Response.Write(JSONFailure("No form-based Resource posted."));
                }
                else
                {
                    validationStatus vstatus = new validationStatus();
                    string DOM = string.Empty;
                    try
                    {
                        #region read the DOM resource as posted
                        System.IO.StreamReader reader = new System.IO.StreamReader(Context.Request.InputStream);
                        DOM = Uri.UnescapeDataString(reader.ReadToEnd());
                        DOM = DOM.Substring(DOM.IndexOf('<')).Substring(0, DOM.LastIndexOf("esource>") + 4); //resource or Resource, with or without ri:
                        #endregion

                        #region clean up empty sections, etc, and publish as if locally harvested
                         if (userAuths.Length > 0) //one record, not first auth: test it now
                            vstatus = ResourceManagement.CleanupAndVerifyData(ref DOM, ref userAuths);
                    }
                    catch(Exception ex)
                    {
                        vstatus.MarkInvalid(ex.Message);
                    }

                    if (vstatus.IsValid)
                    {
                        #region check draft parameters.
                        string url = Context.Request.Url.ToString();
                        bool isDraft = url.Contains("saveAsDraft");
                        bool isPending = url.Contains("pending");
                        #endregion

                        if (isDraft)
                        {
                            try
                            {
                                WritePendingResource(userKey, DOM);
                                Context.Response.Write(JSONSuccess());
                            }
                            catch (Exception ex)
                            {
                                Context.Response.Write(JSONFailure(ex.Message ));
                            }
                        }
                        else
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            int status = -1;

                            if (vstatus.IsValid && userAuths.Length == 0) //user is posting first org/authority record. split this.
                            {
                                string authDOM = string.Empty;
                                string orgDOM = string.Empty;
                                vstatus = SplitNewUserAuthRecords(DOM, ref authDOM, ref orgDOM);
                                if (vstatus.IsValid)
                                {
                                    vstatus = ResourceManagement.CleanupAndVerifyData(ref authDOM, ref userAuths);
                                    if (vstatus.IsValid)
                                    {
                                        vstatus = ResourceManagement.CleanupAndVerifyData(ref orgDOM, ref userAuths);
                                        if (vstatus.IsValid)
                                        {
                                            status = vorXML.LoadVORXML(authDOM, userKey, String.Empty, String.Empty, sb);
                                            if (status == 0)
                                            {
                                                status = vorXML.LoadVORXML(orgDOM, userKey, String.Empty, String.Empty, sb);
                                                if (status == 0)
                                                {
                                                    UserManagement.AssociateDefaultUserAuthority((string)userAuths[0], userKey);
                                                }
                                                else
                                                    vstatus.MarkInvalid(sb.ToString());
                                            }
                                            else
                                                vstatus.MarkInvalid(sb.ToString());
                                        }
                                    }
                                }
                            }
                            else if (vstatus.IsValid)
                                status = vorXML.LoadVORXML(DOM, userKey, String.Empty, String.Empty, sb);

                            if (vstatus.IsValid)
                            {
                                if (status == 0)
                                {
                                    DeprecatePendingResource(userKey, DOM); //if there was one
                                    Context.Response.Write(JSONSuccess());
                                }
                                else
                                    Context.Response.Write(JSONFailure("Error loading resource: " + sb.ToString()));
                            }
                            else
                                Context.Response.Write(JSONFailure(vstatus.GetErrors()));
                        }
                    }
                    else
                        Context.Response.Write(JSONFailure(vstatus.GetErrors()));
                    #endregion
                }
            }

            Context.Response.Flush();
        }

        [WebMethod]
        public void IngestXMLResource()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";
            //html/text and  around the json works around the form upload, but then the results come up as a file to download?

            if (!isLoggedIn())
            {
                Context.Response.Write(JSONFailure("Not Logged In"));
            }
            else
            {
                string[] userAuths = new string[] { };
                long userKey = UserManagement.GetUserKey(Whoami.EZID);
                string[] errors = userManagement.GetUserAuths(userKey, ref userAuths);
                if (errors.Length > 0)
                    Context.Response.Write(JSONFailure(errors));
                else
                {
                    try
                    {
                        HttpFileCollection uploadedFiles = Context.Request.Files;
                        if (uploadedFiles.Count == 0)
                        {
                            Context.Response.Write(JSONFailure("No XML Resource uploaded."));
                        }
                        else
                        {
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {
                                HttpPostedFile userPostedFile = uploadedFiles[i];
                                try
                                {
                                    if (userPostedFile.ContentLength > 0)
                                    {
                                        System.IO.StreamReader streamReader = new System.IO.StreamReader(userPostedFile.InputStream);
                                        string strResource = streamReader.ReadToEnd();
                                        streamReader.Close();
                                        strResource = Uri.UnescapeDataString(strResource);

                                        string ivoid = FindIdentifier(strResource);
                                        if (ivoid == string.Empty)
                                        {
                                            Context.Response.Write(JSONFailure("File does not contain a valid identifier record."));
                                        }
                                        else
                                        {
                                            try
                                            {
                                                XmlDocument doc = new XmlDocument();
                                                doc.LoadXml(strResource);
                                                validationStatus status = ResourceManagement.IngestXmlResource(doc, true, ivoid, userKey);
                                                if (status.IsValid)
                                                    Context.Response.Write(JSONSuccess());
                                                else
                                                    Context.Response.Write(JSONFailure(status.GetErrors()));
                                            }
                                            catch (Exception ex)
                                            {
                                                Context.Response.Write(JSONFailure(ex.Message));
                                            }
                                        }
                                    }
                                    else
                                        Context.Response.Write(JSONFailure("File is empty or did not upload."));
                                }
                                catch (System.ArgumentOutOfRangeException)
                                {
                                    Context.Response.Write(JSONFailure("File is not a valid XML Resource beginning with an ri:resource tag."));
                                }
                                catch (Exception Ex)
                                {
                                    Context.Response.Write(JSONFailure("General failure reading file: " + Ex.Message));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Context.Response.Write(JSONFailure("Error uploading XML resource: " + Server.UrlEncode(ex.Message)));
                    }
                }
            }
            Context.Response.Flush();
        }

        [WebMethod]
        public void SetMyResourceStatus(string identifier, string status)
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            if (!isLoggedIn())
            {
                Context.Response.Write(JSONFailure("Not Logged In"));
            }
            else
            {
                //we could send this as an argument, but don't trust the user/client:
                bool isPending = GetResourceInfo.isPendingResource(identifier, Whoami.EZID);

                validationStatus vs = new validationStatus();
                if (status.ToLower() == "inactive" || status.ToLower() == "active" || status.ToLower() == "deleted")
                { 
                    vs = SetResourceStatus(identifier, status, isPending);
                    if (vs.IsValid)
                        Context.Response.Write(JSONSuccess());
                    else
                        Context.Response.Write(JSONFailure(vs.GetErrors()));
                }
                else
                {
                    Context.Response.Write(JSONFailure("Invalid resource status " + status));
                }
            }
            Context.Response.Flush();
        }

        private validationStatus SetResourceStatus(string identifier, string resourceStatus, bool isPending = false)
        {
            if (!isLoggedIn())
                return new validationStatus("Not Logged In");

            validationStatus status = new validationStatus();
            string[] userAuths = new string[] { };
            long ukey = UserManagement.GetUserKey(Whoami.EZID);

            try
            {
                string xml = string.Empty;
                if (isPending)
                {
                    xml = GetResourceInfo.SelectPendingResource(Whoami.EZID, identifier);
                }
                else
                {
                    SqlConnection conn = new SqlConnection(sConnect);
                    conn.Open();

                    //don't let user delete anyone's default auth
                    if (resourceStatus == "deleted" || resourceStatus == "inactive")
                    {
                        string checkDefaults = "select count(*) from users where defaultAuthorityID = '" + identifier + "'";
                        SqlDataAdapter sqlDA = new SqlDataAdapter(checkDefaults, conn);
                        DataSet ds = new DataSet();
                        sqlDA.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0 && (int)ds.Tables[0].Rows[0][0] > 0)
                        {
                            status.MarkInvalid("Resource " + identifier + " cannot be deleted because it is the default authority ID of at least one active user." +
                                " Please contact helpdesk@usvao.org if you believe related user accounts should be purged or migrated to another authority.");
                        }
                    }

                    if (status.IsValid)
                    {
                        SqlCommand cmd = conn.CreateCommand();
                        cmd.CommandText = getResourceStatement;
                        cmd.Parameters.Add("@Identifier", SqlDbType.VarChar, 500);
                        cmd.Parameters.Add("@ukey", SqlDbType.BigInt);
                        cmd.Prepare();  // Calling Prepare after having setup commandtext and params.
                        cmd.Parameters["@Identifier"].Value = identifier;
                        cmd.Parameters["@ukey"].Value = ukey;

                        SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        sqlDA.Fill(ds);
                        int ncount = ds.Tables[0].Rows.Count;
                        if (ncount < 1)
                        {
                            status.MarkInvalid("Could not find resource " + identifier + " belonging to current user. Login may have timed out.");
                        }
                        else
                        {
                            xml = (string)ds.Tables[0].Rows[0][0];
                        }
                    }
                }

                if (status.IsValid)
                {
                    if (isPending && resourceStatus == "deleted")
                        status = MarkDeletedPendingResource(identifier);
                    else
                        status = ResourceManagement.SetXmlResourceStatus(identifier, resourceStatus, ukey);
                }

            }
            catch (Exception e)
            {
                status.MarkInvalid("Error changing status of resource " + identifier + " to " + resourceStatus + ": " + e.Message);
            }

            return status;
        }

        private validationStatus MarkDeletedPendingResource(string identifier)
        {
            validationStatus status = new validationStatus();
            SqlConnection conn = new SqlConnection(sConnect);
            long ukey = UserManagement.GetUserKey(Whoami.EZID);
            if (ukey == 0)
                status.MarkInvalid("Not logged in.");

            if (status.IsValid)
            {
                try
                {
                    conn.Open();

                    SqlCommand ins = conn.CreateCommand();
                    string updateCommand = "UPDATE PendingResources set rstat=3 where rstat = 1 and ivoid = '" + identifier + "' and ukey = " + ukey + ";";
                    ins.CommandText = updateCommand;
                    ins.Prepare();
                    ins.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    status.MarkInvalid(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            return status;
        }

        private validationStatus SplitNewUserAuthRecords(string DOM, ref string authDOM, ref string orgDOM)
            {
                validationStatus status = new validationStatus();
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(DOM);
                    string ivoid = doc.GetElementsByTagName("identifier")[0].InnerText.Trim();
                    string idAuth = ivoid.Substring(0, ivoid.LastIndexOf('/'));

                    XmlDocument auth = (XmlDocument)doc.CloneNode(true);
                    XmlDocument org = (XmlDocument)doc.CloneNode(true);

                    //change for auth record:
                    auth.GetElementsByTagName("identifier")[0].InnerText = idAuth;
                    XmlAttribute xsitype = auth.FirstChild.Attributes["xsi:type"];
                    xsitype.Value = "vg:Authority";


                    //change for org record:
                    XmlNode managingOrg = org.GetElementsByTagName("managingOrg")[0];
                    managingOrg.ParentNode.RemoveChild(managingOrg);
                    xsitype = org.FirstChild.Attributes["xsi:type"];
                    xsitype.Value = "vr:Organisation";

                    System.IO.StringWriter sw = new System.IO.StringWriter();
                    XmlTextWriter xw = new XmlTextWriter(sw);
                    auth.WriteTo(xw);
                    authDOM = sw.ToString();

                    sw = new System.IO.StringWriter();
                    xw = new XmlTextWriter(sw);
                    org.WriteTo(xw);
                    orgDOM = sw.ToString();
                }
                catch (Exception ex)
                {
                    status.MarkInvalid(ex.Message);
                }
                return status;
            }

        internal static string FindIdentifier(string strResource)
        {
            string id = string.Empty;
            System.IO.StringReader rd = new System.IO.StringReader(strResource);
            XmlReader xr = new XmlTextReader(rd);
            while (xr.Read())
            {
                string name = xr.LocalName.ToUpper();
                if ((name == "IDENTIFIER") && (xr.NodeType != XmlNodeType.EndElement))
                {
                    xr.Read();
                    id = xr.Value.Trim();
                    break;
                }
            }
            return id;
        }

        private void WritePendingResource(long userKey, string DOM)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(DOM);
                    string id = doc.GetElementsByTagName("identifier")[0].InnerText;

                    const int maxntext = 1073741823; //2^30-1 is max.
                    SqlConnection conn = new SqlConnection(sConnect);
                    conn.Open();
                    try
                    {
                        SqlCommand ins = conn.CreateCommand();

                        ins.CommandText = "update [PendingResources] set rstat=2 where rstat = 1 and ivoid = '" + id + "' and ukey = " + userKey + ";";
                        ins.ExecuteNonQuery();

                        ins = conn.CreateCommand();
                        ins.CommandText = "insert into [PendingResources](ukey, rstat, ivoid, xml) values(" + userKey + ",1,'" + id + "',@xml);";
                        ins.Parameters.Add("@xml", SqlDbType.NText, maxntext);
                        ins.Parameters[0].Value = DOM;
                        ins.Prepare();
                        ins.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

        private void DeprecatePendingResource(long userKey, string DOM)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(DOM);
            string id = doc.GetElementsByTagName("identifier")[0].InnerText;

            SqlConnection conn = new SqlConnection(sConnect);
            conn.Open();
            try
            {
                SqlCommand ins = conn.CreateCommand();
                string updateCommand = "UPDATE PendingResources set rstat=0 where rstat = 1 and ivoid = '" + id + "' and ukey = " + userKey + ";";
                ins.CommandText = updateCommand;
                ins.Prepare();
                ins.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        //Note there is an issue with file upload responses on some browsers wrapping this response in <pre> tags 
        //if something else isn't provided. This will break the JSON decoding of file upload response.
        //This is part of a workaround from Sencha.

        private string JSONSuccess()
        {
            return "{ 'success': true}";
        }

        private string JSONSuccess(string details)
        {
            return "{ 'success': true, 'details': '" + details + "'}";
        }

        private string JSONFailure(string error)
        {
            StringBuilder sb = new StringBuilder();
            if (error != null && error.Length > 0)
            {
                sb.Append("{ 'success': false, 'errors': { 'reason': '");
                sb.Append(error.Replace("\'","`").Replace("\r\n", " "));
                sb.Append("' }}");
            }
            else
                sb.Append("{ 'success': false, 'errors': { 'reason': 'Login failed. User unknown to this system.' }}");
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
                    sb.Append(error.Replace("\'", "`").Replace("\r\n", " ") + ' ');
                }
                sb.Append("' }}");
            }
            else
                sb.Append("{ 'success': false, 'errors': { 'reason': 'Login failed. User unknown to this system.' }}");
            return sb.ToString();
        }
    }
}