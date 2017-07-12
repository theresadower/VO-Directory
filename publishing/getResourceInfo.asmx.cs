using System.ComponentModel;
using System.Web.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Publishing
{
    [WebService(Namespace = "http://www.us-vo.org")]
    public class GetResourceInfo : System.Web.Services.WebService
    {
        private static UserManagement userManager = new UserManagement();
        private static string sConnect = (string)System.Configuration.ConfigurationManager.AppSettings["SqlConnection"];
        private static string appDir = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

        private static string sActive = "active";
        private static string sInactive = "inactive";
        private static string sDeleted = "deleted";

        private static string sNonStandard = "Non-standard";
        private static string sSSA = "Spectra";
        private static string sSIA = "Image";
        private static string sConeSearch = "ConeSearch";
        private static string sWebPage = "Web Page";
        private static string sParamHTTP = "HTTP Request";

//dower todo: JSON failures in server and client.
//dower todo: externalize json success/failure building

        public GetResourceInfo()
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
        public void GetMyAuthoritiesList()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            if (isLoggedIn())
            {
                string getListing = "select distinct rs.res_title, rs.ivoid from " +
                                        " resource rs, userAuthorities ua, users us where rstat = 1 and ua.authorityID = rs.ivoid and " +
                                        " ua.ukey = us.pkey and us.username = '" + Whoami.EZID + "'";
                GetIdentifierList(getListing, "AuthorityInfo", true);
            }

            Context.Response.Flush();
        }

        [WebMethod]
        public void GetAuthorityList()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            string getListing = "select distinct rs.res_title, rs.ivoid from authority auth, " +
                                 "resource rs where rs.ivoid like ('ivo://' + authority) and rs.[rstat] = 1 and (harvestedFromID = '' or harvestedFromID like '%STScI%') and rs.res_type like '%Authority%'";
            GetIdentifierList(getListing, "AuthorityInfo", true);

            Context.Response.Flush();
        }

        [WebMethod]
        public void GetPublisherList()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            string getListing = "select rs.res_title, rs.ivoid, rs.res_type from " +
                     "resource rs where [rstat]=1 and (res_type like '%Resource' or res_type like '%Organisation')";
            GetIdentifierList(getListing, "PublisherInfo");

            Context.Response.Flush();
        }

        //dower todo: allow to work with deleted resources.
        [WebMethod]
        public void GetMyResourcesList()
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            if (isLoggedIn())
            {
                string getList = "select rs.res_title, rs.short_name, rs.ivoid, rs.[rstat], rs.[updated], cap.cap_type, iface.intf_type from users us, resource rs left outer join capability cap on rs.pkey = cap.rkey " +
                        "left outer join interface iface on cap.pkey = iface.ckey " +
                        "where ([rstat] > 0 and [rstat] != 3) and rs.ukey = us.pkey and us.username = '" + Whoami.EZID + "'";

                System.Collections.Hashtable collectedRows = new System.Collections.Hashtable();
                #region query and translate to JSON
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(sConnect);
                    conn.Open();

                    SqlDataAdapter sqlDA = new SqlDataAdapter(getList, conn);
                    DataSet ds = new DataSet();
                    sqlDA.Fill(ds);

                    int ncount = ds.Tables[0].Rows.Count;
                    if (ncount > 0)
                    {
                        for (int i = 0; i < ncount; ++i)
                        {
                            DataRow dr = ds.Tables[0].Rows[i];
                            string title = ((String)dr[0]).Replace("\"", "").Replace("\'", "");
                            string shortName = ((String)dr[1]).Replace("\"", "").Replace("\'", "");
                            string status = sActive;
                            if ((int)dr[3] == 3)
                                status = sDeleted;
                            else if ((int)dr[3] == 2)
                                status = sInactive;
                            DateTime dt = ((DateTime)dr[4]);
                            string updated = String.Format("{0:u}", dt);

                            string type = string.Empty;
                            if (dr[5] != null && dr[5] != DBNull.Value)
                            {
                                string typetext = ((String)dr[5]);
                                if (typetext.Length == 0 || typetext.IndexOf("Capability") > -1)
                                {
                                    if (dr[6] != null && dr[6] != DBNull.Value)
                                        typetext = ((String)dr[6]);
                                    if (typetext.IndexOf("Param") > -1)
                                        type = sParamHTTP;
                                    else if (typetext.IndexOf("Web") > -1)
                                        type = sWebPage;
                                    else
                                        type = sNonStandard;
                                }
                                else
                                {
                                    type = typetext.Substring(typetext.IndexOf(':') + 1);
                                    if (type.IndexOf("Spectral") > -1)
                                        type = sSSA;
                                    else if (type.IndexOf("Image") > -1)
                                        type = sSIA;
                                    else if (type.IndexOf("Cone") > -1)
                                        type = sConeSearch;
                                }
                            }
                            string id = (string)dr[2];
                            if (collectedRows.ContainsKey(id))
                            {
                                String existing = (String)collectedRows[id];
                                if (existing.IndexOf(sNonStandard) > -1)
                                    existing = existing.Insert(existing.IndexOf(sNonStandard), type + ", ");
                                else if (existing.IndexOf(sParamHTTP) > -1)
                                    existing = existing.Insert(existing.IndexOf(sParamHTTP), type + ", ");
                                else if (existing.IndexOf(sWebPage) > -1)
                                    existing = existing.Insert(existing.IndexOf(sWebPage), type + ", ");
                                else
                                    existing = existing.Insert(existing.IndexOf("\"}"), ", " + type);
                                collectedRows[id] = existing;
                            }
                            else if (!collectedRows.ContainsKey(id)) //then add new resource to the list
                            {
                                collectedRows.Add(id, ("{\"title\":\"" + title + "\",\"shortName\":\"" + shortName + "\",\"identifier\":\"" + id + "\",\"status\":\"" + status + "\",\"updated\":\"" + updated + "\",\"type\":\"" + type + "\"}"));
                            }
                        }
                    }

                    //now get 'pending' resources.
                    System.Collections.Hashtable pendingRows = GetMyPendingResources(ref collectedRows);
                    if (collectedRows.Count > 0 || pendingRows.Count > 0)
                    {
                        ncount = 0;
                        Context.Response.Write("{\"ResourceInfo\":[");
                        foreach (System.Collections.DictionaryEntry de in collectedRows)
                        {
                            Context.Response.Write(de.Value);
                            if (++ncount < (collectedRows.Count + pendingRows.Count)) Context.Response.Write(',');
                        }
                        foreach (System.Collections.DictionaryEntry de in pendingRows)
                        {
                            Context.Response.Write(de.Value);
                            if (++ncount < (collectedRows.Count + pendingRows.Count)) Context.Response.Write(',');
                        }
                        Context.Response.Write("]}");
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                finally
                {
                    conn.Close();
                }
                #endregion
            }
            Context.Response.Flush();
        }

        [WebMethod]
        public void GetMyResource(string identifier)
        {
            Context.Response.ClearHeaders();
            Context.Response.ClearContent();
            Context.Response.ContentType = "text/javascript";

            if (isLoggedIn())
            {
                try
                {
                    string response = null;
                    bool isPending = GetResourceInfo.isPendingResource(identifier, Whoami.EZID);
                    if (isPending == false)
                        response = SelectResource(Whoami.EZID, identifier);
                    else
                        response = SelectPendingResource(Whoami.EZID, identifier);

                    if (response != null)
                    {
                        Context.Response.ContentType = "text/xml";
                        Context.Response.Write(response);
                    }
                    else
                    {
                        if (isPending)
                            Context.Response.Write("Pending ");
                        Context.Response.Write("Resource " + identifier + " does not exist or does not belong to the current user. Login may have timed out.");
                    }
                }
                catch (Exception ex)
                {
                    Context.Response.Write(ex.Message);
                }

                Context.Response.Flush();
            }
        }

        //used within the other GetList functions
        private void GetIdentifierList(string sql, string jsonTitle, bool concat = false)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(sConnect);
                conn.Open();


                SqlDataAdapter sqlDA = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);

                int ncount = ds.Tables[0].Rows.Count;
                String[] titles = new String[ncount];
                String[] keys = new String[ncount];

                for (int i = 0; i < ncount; ++i)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    titles[i] = ((String)dr[0]).Replace("\"", "").Replace("\'", "");
                    keys[i] = (String)dr[1];
                    if (concat)
                        titles[i] += " (" + keys[i] + ")";
                }
                Array.Sort(titles, keys);

                Context.Response.Write("{\"" + jsonTitle + "\":[");
                for (int i = 0; i < ncount; ++i)
                {
                    Context.Response.Write("{\"title\":\"" + titles[i] + "\",\"identifier\":\"" + keys[i] + "\"}");
                    if (i < ncount - 1) Context.Response.Write(',');
                }
                Context.Response.Write("]}");
            }
            finally
            {
                conn.Close();
            }
        }

        private System.Collections.Hashtable GetMyPendingResources(ref System.Collections.Hashtable publishedResources)
        {
            System.Collections.Hashtable table = new System.Collections.Hashtable();

            if (!isLoggedIn())
                return table;

            string[] resources = SelectPendingResources(Whoami.EZID);
            foreach (string str in resources)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);

                string id = doc.GetElementsByTagName("identifier")[0].InnerText;
                string title = doc.GetElementsByTagName("title")[0].InnerText;
                string shortName = string.Empty;
                if (doc.GetElementsByTagName("shortName").Count > 0)
                    shortName = doc.GetElementsByTagName("shortName")[0].InnerText;
                string updated = string.Empty;

                if (!table.ContainsKey(id) && !publishedResources.ContainsKey(id))
                    table.Add(id, ("{\"title\":\"" + title + "\",\"shortName\":\"" + shortName + "\",\"identifier\":\"" + id + "\",\"status\":\"" + "PENDING" + "\",\"updated\":\"" + updated + "\",\"type\":\"" + ' ' + "\"}"));
            }

            return table;
        }

        private string[] SelectPendingResources(string username)
        {
            string[] results = null;
            SqlConnection conn = null;
            try
            {
                string getList = "select xml from PendingResources, users where rstat = 1 and " +
                                 " PendingResources.ukey = users.pkey and users.username = '" + username + "'";

                conn = new SqlConnection(sConnect);
                conn.Open();

                SqlDataAdapter sqlDA = new SqlDataAdapter(getList, conn);
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);

                int ncount = ds.Tables[0].Rows.Count;
                results = new string[ncount];
                if (ncount > 0)
                {
                    for (int i = 0; i < ncount; ++i)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        results[i] = (string)dr[0];
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public static string SelectPendingResource(string username, string identifier)
        {
            string response = null;
            string sql = "select top 1 xml from PendingResources, users where rstat = 1 and ivoid = '" + identifier + "' and " +
                "PendingResources.ukey = users.pkey and users.username = '" + username + "'";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(sConnect);
                conn.Open();

                SqlDataAdapter sqlDA = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);

                int ncount = ds.Tables[0].Rows.Count;
                if (ncount > 0)
                {
                    response = (string)(ds.Tables[0].Rows[0][0]);
                }
                conn.Close();
            }
            catch (SqlException)
            {
                response = null;
            }
            return response;
        }

        public static bool isPendingResource(string identifier, string username)
        {
            bool isPending = false;

            string sql = "select ivoid from PendingResources, users where rstat = 1 and ivoid = '" + identifier + "' and " +
                "PendingResources.ukey = users.pkey and users.username = '" + username + "'";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(sConnect);
                conn.Open();

                SqlDataAdapter sqlDA = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);

                int ncount = ds.Tables[0].Rows.Count;
                if (ncount > 0)
                {
                    isPending = true;
                }
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return isPending;
        }

        private static string SelectResource(string username, string identifier)
        {
            string response = null;
            if (username != null && username.Length > 0)
            {
                string sql = "select top 1 xml from resource, users where ivoid = '" + identifier + "' and [rstat] = 1 and " +
                             "resource.ukey = users.pkey and users.username = '" + username + "'";
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(sConnect);
                    conn.Open();

                    SqlDataAdapter sqlDA = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    sqlDA.Fill(ds);

                    int ncount = ds.Tables[0].Rows.Count;
                    if (ncount > 0)
                    {
                        response = (string)(ds.Tables[0].Rows[0][0]);
                    }
                    conn.Close();
                }
                catch(SqlException)
                {
                    response = null;
                }
            }
            return response;
        }
    }
}