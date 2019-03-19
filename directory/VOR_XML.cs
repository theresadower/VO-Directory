using System;
using System.Xml;
using System.IO;
using System.Collections;
using registry;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Net;


namespace registry
{


    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class VOR_XML
    {
        private static string sConnect = (string)System.Configuration.ConfigurationManager.AppSettings["SqlAdminConnection"];
        private static string appDir = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        private static string registryIdentity = (string)System.Configuration.ConfigurationManager.AppSettings["registryIdentity"];
        public static string strValidatedBy = registryIdentity;
        private static bool testedTAPTables = false;
        private static bool hasTAPTables = false;

        public VOR_XML()
		{
            if (sConnect == null)
                
                sConnect = registry.Properties.Settings.Default.SqlAdminConnection;
            if (appDir == null)
                appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
            if (registryIdentity == null)
            {
                registryIdentity = registry.Properties.Settings.Default.registryIdentity;
                strValidatedBy = registryIdentity;
            }
		}

        private static string appendUrl(string urlbase, string last)
        {
            if (urlbase.EndsWith("/"))
                return urlbase + last;
            else
                return urlbase + '/' + last;
        }

        public int DeleteVORXML(string id, long userKey, string harvestUrl, string registryID, StringBuilder sbOut)
        {
            SqlConnection conn = null;
            string theXML = string.Empty;

            // Lookup Record to Check if Exists
            try
            {
                conn = new SqlConnection(sConnect);
                conn.Open();

                try
                {
                    string cmd = SQLHelper.createFullXMLActiveResourceSelect("resource.ivoid = '" + id + "'");
                    SqlDataAdapter sqlDA = new SqlDataAdapter(cmd, conn);
                    DataSet ds = new DataSet();
                    sqlDA.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 0) //not found, but not unexpected.
                        return 0;
                    else
                        theXML = (string) ds.Tables[0].Rows[0][0];
                }
                catch (Exception e) { sbOut.Append("Lookup Exist Record Error:" + e.Message + ":" + e.StackTrace); };

                if (theXML.Length == 0)
                {
                    return 0; //record not in DB anyway.
                }
                else
                {
                    //change status to deleted.
                    theXML = SetStatus(theXML, "deleted");
                    return LoadVORXML(theXML, userKey, harvestUrl, registryID, sbOut);
                }
            }
            catch (Exception e) { sbOut.Append("Error loading resource to be deleted: " + e.Message); };
            return -1;
        }


        // theXml: as a string. will be inherently schema-validated somewhat in the xsl translation and db ingestion process.
        // '0' userkey: no set value. Comes from harvest or certain management functions in publishing. 
        // should be overwritten with user key from previous versio if there is one.
        // harvestUrl: empty value from publishing; OAI interface if from harvesting.
        // registryID: empty value from publishing; RofR ivoid of home registry if known (tracking for when registry urls changes over time)
        // sbOut: for logging.
        public int LoadVORXML(string theXML, long userKey, string harvestUrl, string registryID, StringBuilder sbOut)
        {
            int iReturn = 0;
            SqlConnection conn = null; 

            // First Read theXML and select the IVOID for loading in the full XML
            // from the IDENTIFIER element tag
            
            string ivoid = "";
            long resourceKey = 0; // note keys can not be 0,  so this is for unset key
            StringReader rd = new StringReader(theXML);
            StringBuilder sbSQL = new StringBuilder();
            SqlCommand cachecmd = new SqlCommand();
 
            XmlReader xr = new XmlTextReader(rd);
            while (xr.Read())
			{
                string name = xr.LocalName.ToUpper();
                if ( (name == "IDENTIFIER") && ( xr.NodeType != XmlNodeType.EndElement) ) 
                {
                    xr.Read(); 
                    ivoid = xr.Value.Trim();
                    break; // to avoid selecting identifier tags within subelements of resource
                }
            }

            // Lookup Record to Check if Exists
            try
            {
                conn = new SqlConnection(sConnect);
                conn.Open();

                try
                {
                    SqlCommand lookupKeys = SQLHelper.getKeysLookupCmd(conn);
                    lookupKeys.Parameters[0].Value = ivoid;

                    using (SqlDataReader rdr = lookupKeys.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (rdr.Read())
                        {
                            resourceKey = rdr.GetInt64(0);
                            if (!rdr.IsDBNull(1) && userKey == 0) //previous version in DB exists and is set, no user key given by publishing/ingest code upstream.
                                userKey = rdr.GetInt64(1);
                        }
                    }

                }
                catch (Exception e) {sbOut.Append("Lookup Cache Record Error:" + e.Message + ":" + e.StackTrace); };

                if (testedTAPTables == false)
                {
                    try
                    {
                        hasTAPTables = SQLHelper.TestTAPTables(conn);
                        testedTAPTables = true;                  
                    }
                    catch (Exception e) { sbOut.Append("Lookup table info error:" + e.Message + ":" + e.StackTrace); };
                }

                //Load in the VOR DB Schema Table Fields
                try
                {
                    // convert XML resource into db sql insert commands via XSL

                    //preserve will generally be false -- allowing harvested and publisher-interface edits to overwrite changes. 
                    //but for initial harvest of new DB we want to keep this
                    bool preserveLocalLevels = false;
                    if ((strValidatedBy == registryIdentity)
                        || (registryID.StartsWith(strValidatedBy) && userKey == 0))
                        preserveLocalLevels = true;

                    int localLevel = 2; //default, technically valid, interfaces untested, etc.
                    theXML = SaveValidationLevels(theXML, preserveLocalLevels, ref localLevel);
                    bool bHandCurated = preserveLocalLevels && (localLevel != 2);
                   
                    string sres = theXML;
                    StringReader reader = new StringReader(sres);
                    XPathDocument myXPathDoc = new XPathDocument(reader);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    XmlUrlResolver resolver = new XmlUrlResolver();
                    resolver.Credentials = CredentialCache.DefaultCredentials;

                    //load the Xsl 
                    StringWriter sw = new StringWriter(sbSQL);
                    string insertFile = appDir + "xsl\\insertvoresource.xsl";
                    myXslTrans.Load(insertFile, XsltSettings.TrustedXslt, resolver);

                    // Load the Xsl Parameters
                    XsltArgumentList argList = new XsltArgumentList();
                    String harvestDate = DateTime.Now.ToUniversalTime().ToString("s");

                    argList.AddParam("harvestedFromEP", "", harvestUrl);
                    argList.AddParam("harvestedFromDate", "", harvestDate);
                    argList.AddParam("harvestedFromID", "", registryID);
                    argList.AddParam("localRegistryID", "", strValidatedBy);
                    if (resourceKey != 0) argList.AddParam("existingrkey", "", resourceKey);
                    if (bHandCurated)
                        argList.AddParam("curated", "", "true");
                    else
                        argList.AddParam("curated", "", "false");
                            

                    XmlTextWriter writer = new XmlTextWriter(sw);
                    myXslTrans.Transform(myXPathDoc, argList, writer);

                    if (testedTAPTables == true && hasTAPTables == true)
                    {
                        StringBuilder sbTAPSql = BuildTAPInsert(theXML, registryID, myXslTrans, resolver);
                        sbSQL.Append(sbTAPSql);
                    }

                    string cacherow = GetVOTableCacheRow(theXML, sbOut);
                    string interfaces = GetVOTableCacheInterfaceRows(theXML, sbOut);
                    if (cacherow.Length > 0)
                    {
                        cachecmd = SQLHelper.GetInsertVOTableCmd("@rkey", cacherow, interfaces, conn);
                    }
                    else
                    {
                        sbOut.Append(" No error given, but failed to create cache SQL. " + "Resource: " + theXML);
                        iReturn = -1;
                        return iReturn;
                    }
                }
                catch (Exception e)
                {
                    conn.Close();
                    sbOut.Append("Xsl Load Exception using application directory " + appDir + " . " + e.Message + ":" + e.StackTrace);
                    iReturn = -1;
                    return iReturn;
                };


                SqlCommand command = conn.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = conn.BeginTransaction("VOResourceInsertTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = conn;
                command.Transaction = transaction;
                cachecmd.Transaction = transaction;

                int rows = 0;
                int cacherows = 0;
                try
                {
                    command.CommandText = sbSQL.ToString();
                    rows = command.ExecuteNonQuery();

                    cacherows = cachecmd.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();

                    if (rows <= 0 || cacherows <= 0)
                    {
                        sbOut.Append(" No error given, but failed to write resource to DB. " + "ivoid: " + ivoid);
                        iReturn = -1;
                    }
                }
                catch (Exception ex)
                {
                    sbOut.Append(" ivoid: " + ivoid + " Commit Exception: " + ex.GetType());
                    sbOut.Append("  Message: " + ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        sbOut.Append("Rollback Exception: " + ex2.GetType());
                        sbOut.Append("  Message:" + ex2.Message);
                    }
                    iReturn = -1;
                    conn.Close();
                }

                if (iReturn == 0)
                {

                    // (2) 
                    // Load in the full XML field in the Resource Table where the Identifier 
                    // matches the resource 
                    try
                    {
                        const int maxntext = 1073741823; //2^30-1 is max.

                        SqlCommand ins = conn.CreateCommand();
                        StringBuilder sb = new StringBuilder();

                        string updateCommand = "UPDATE Resource set xml = @xml";
                        sb.Append(updateCommand);
                        sb.Append(" where pkey in ( select top 1 pkey from Resource where ivoid= '" + ivoid + "' order by harvestedFromDate desc)");
                        ins.CommandText = sb.ToString();
                        ins.Parameters.Add("@xml", SqlDbType.NText, maxntext);
                        ins.Parameters[0].Value = theXML.ToString();
                        ins.Prepare();
                        ins.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        sbOut.Append("Failed to Load Full XML for identifier: " + ivoid + ". " + e.Message);
                        iReturn = -1;
                    }

                    if (iReturn == 0)
                    {
                        // (3)
                        //  Associate new resource with a user record if one was specified.
                        try
                        {
                            if (userKey > 0)
                            {
                                SqlCommand ins = conn.CreateCommand();

                                ins.CommandText = "UPDATE Resource set ukey = " + userKey +
                                                        " where ivoid = " + "'" + ivoid + "'";
                                ins.ExecuteNonQuery();
                            }
                        }
                        catch (Exception e)
                        {
                            sbOut.Append("Failed to associate User with Resource: " + ivoid + ". " + e.Message);
                            iReturn = -1;
                        }
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            return iReturn;
        }

        private StringBuilder BuildTAPInsert(string theXML, string registryID, XslCompiledTransform myXslTrans, XmlUrlResolver resolver)
        {
            //some transformations can't be done in XSLT 1.0. One of these, required by the IVOA Registry Relational standard, is 
            //normalizing some fields to lowercase.

            //The following columns MUST be lowercased during ingestion: 
            //ivoid, res_type, content_level, content_type, source_format, waveband. )
            XmlDocument tapdoc = new XmlDocument();
            tapdoc.LoadXml(theXML);

            XmlNamespaceManager ns = new XmlNamespaceManager(tapdoc.NameTable);
            ns.AddNamespace("ri", "http://www.ivoa.net/xml/RegistryInterface/v1.0");
            ns.AddNamespace("vs", "http://www.ivoa.net/xml/VODataService/v1.0");
            ns.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            XmlNodeList targetNodes = tapdoc.SelectNodes("/ri:Resource/coverage/waveband", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
             targetNodes = tapdoc.SelectNodes("/ri:Resource/content/contentLevel", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/content/type", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/identifier", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/@xsi:type", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/content/source/@format", ns);
            foreach (XmlNode node in targetNodes) node.Value = node.Value.ToLower();

            //ivoid, role_ivoid, base_role
            targetNodes = tapdoc.SelectNodes("/ri:Resource/curation/contact/name/@ivo-id", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/curation/publisher/@ivo-id", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/curation/creator/name/@ivo-id", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/curation/contributor/@ivo-id", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //capability:ivoid, cap_type, standard_id.
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/@xsi:type", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/@standard_id", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //res_schema:ivoid, schema_name, schema_utype. 
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/name", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/utype", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //res_table:ivoid, table_name, table_type, table_utype.
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/name", ns);
            foreach (XmlNode node in targetNodes)  node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/@type", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/utype", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //table_column: ivoid, name, ucd, utype, datatype, type_system.
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/column/name", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/column/utype", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/column/ucd", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/column/dataType", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/tableset/schema/table/column/dataType/@xsi:type", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();


            //intf:ivoid, intf_type, intf_role, std_version, query_type, result_type, url_use.
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/interface/@role", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/interface/@xsi:type", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/@standardID", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/interface/accessURL/@use", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/interface/queryType", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/capability/interface/resultType", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //relationship: ivoid, relationship_type, related_id. 
            targetNodes = tapdoc.SelectNodes("/ri:Resource/content/relationship/relatedResource", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/content/relationship/relatedResource/@ivo-id", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();
            targetNodes = tapdoc.SelectNodes("/ri:Resource/content/relationship/relationshipType", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //validation: ivoid, validated_by. 
            targetNodes = tapdoc.SelectNodes("/ri:Resource/validationLevel/@validatedBy", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            //res_date:ivoid, value_role. 
            targetNodes = tapdoc.SelectNodes("/ri:Resource/curation/date/@role", ns);
            foreach (XmlNode node in targetNodes) node.InnerXml = node.InnerXml.ToLower();

            string tapres = tapdoc.OuterXml;
            StringReader reader = new StringReader(tapres);
            XPathDocument myXPathDoc = new XPathDocument(reader);

            StringBuilder sbTAPSql = new StringBuilder();
            StringWriter scachew = new StringWriter(sbTAPSql);
            string tapFile = appDir + "xsl\\InsertVOResource_RegTap.xsl";
            myXslTrans.Load(tapFile, XsltSettings.TrustedXslt, resolver);

            // Load the Xsl Parameters
            XsltArgumentList tapArgList = new XsltArgumentList();
            tapArgList.AddParam("harvestedFromID", "", registryID);
            tapArgList.AddParam("localRegistryID", "", strValidatedBy);

            XmlTextWriter tapwriter = new XmlTextWriter(scachew);
            myXslTrans.Transform(myXPathDoc, tapArgList, tapwriter);

            return sbTAPSql;
        }

        private static string SaveValidationLevels(string theXML, bool preserveLocalLevels, ref int localLevel)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(theXML);
            XmlNode localValidation = null;
            XmlNode lastNode = null;

            XmlNodeList list = doc.FirstChild.ChildNodes;
            foreach (XmlNode node in list)
            {
                if ((node.Name.ToLower() == "validationlevel") && !(node.ParentNode.Name == "capability"))
                {
                    lastNode = node;

                    //tdower temporary cleanup on first ingest: these records were validated locally.
                    if (node.Attributes["validatedBy"] != null &&
                        ((node.Attributes["validatedBy"].Value == "ivo://mast.stsci.edu") || node.Attributes["validatedBy"].Value == "ivo://archive.stsci.edu"))
                    {
                        node.Attributes["validatedBy"].Value = registryIdentity;
                    }


                    if (node.Attributes["validatedBy"] != null && node.Attributes["validatedBy"].Value == strValidatedBy)
                    {
                        node.Attributes["validatedBy"].Value = registryIdentity;
                        if (!preserveLocalLevels)
                            node.InnerXml = "2";

                        localValidation = node;
                        localLevel = Convert.ToInt32(node.InnerXml);
                    }
                }
            }
            if (localValidation == null)
            {
                localLevel = 2;

                XmlElement newNode = doc.CreateElement("validationLevel");
                XmlAttribute newAttribute = doc.CreateAttribute("validatedBy");
                newAttribute.Value = strValidatedBy;
                newNode.Attributes.Append(newAttribute);
                newNode.InnerXml = "2";
                if (lastNode != null)
                    lastNode.ParentNode.InsertAfter(newNode, lastNode);
                else
                    doc.FirstChild.InsertBefore(newNode, doc.FirstChild.FirstChild);
            }

            list = doc.GetElementsByTagName("capability");
            foreach (XmlNode node in list)
            {
                localValidation = null;
                lastNode = null;
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name.ToLower() == "validationlevel")
                    {
                        lastNode = child;

                        //tdower temporary cleanup on first ingest: these records were validated locally.
                        if (child.Attributes["validatedBy"] != null &&
                            ((child.Attributes["validatedBy"].Value == "ivo://mast.stsci.edu") || child.Attributes["validatedBy"].Value == "ivo://archive.stsci.edu"))
                        {
                            child.Attributes["validatedBy"].Value = strValidatedBy;
                        }

                        if (child.Attributes["validatedBy"] != null && child.Attributes["validatedBy"].Value == strValidatedBy)
                        {
                            child.Attributes["validatedBy"].Value = strValidatedBy;
                            if (!preserveLocalLevels)
                                child.InnerXml = "2";

                            localValidation = child;
                        }
                    }
                }
                if (localValidation == null)
                {
                    XmlElement newNode = doc.CreateElement("validationLevel");
                    XmlAttribute newAttribute = doc.CreateAttribute("validatedBy");
                    newAttribute.Value = strValidatedBy;
                    newNode.Attributes.Append(newAttribute);
                    newNode.InnerXml = "2";
                    if (lastNode != null)
                        lastNode.ParentNode.InsertAfter(newNode, lastNode);
                    else
                        node.InsertBefore(newNode, node.FirstChild);
                }
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);
            return sw.ToString();
        }

        private static string SetStatus(string theXML, string newStatus)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(theXML);

            XmlNode top = doc.FirstChild;
            top.Attributes["status"].InnerText = newStatus;
           

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);
            return sw.ToString();
        }


        public int LoadEntireVOTableCache(string theXML, StringBuilder sbOut)
        {
            int iReturn = 0;
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(sConnect);

                if (sConnect == null)
                    throw new Exception("Loader: SqlConnection.String not found in Web.config");

                // First Read theXML and select the IVOID for loading in the full XML
                // from the IDENTIFIER element tag

                string ivoid = "";
                long resourceKey = 0; // note keys can not be 0,  so this is for unset key
                long userKey = 0; //similarly, for unset.
                StringReader rd = new StringReader(theXML);
                StringBuilder sbSQL = new StringBuilder();

                XmlReader xr = new XmlTextReader(rd);
                while (xr.Read())
                {
                    string name = xr.LocalName.ToUpper();
                    if ((name == "IDENTIFIER") && (xr.NodeType != XmlNodeType.EndElement))
                    {
                        xr.Read();
                        ivoid = xr.Value.Trim();
                        break; // to avoid selecting identifier tags within subelements of resource
                    }
                }

                // Lookup Record to Check if Exists
                try
                {
                    conn.Open();

                    SqlCommand lookupKeys = SQLHelper.getKeysLookupCmd(conn);
                    lookupKeys.Parameters[0].Value = ivoid;

                    using (SqlDataReader rdr = lookupKeys.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (rdr.Read())
                        {
                            resourceKey = rdr.GetInt64(0);
                            if (!rdr.IsDBNull(1))
                                userKey = rdr.GetInt64(1);
                        }
                    }

                }
                catch (Exception e)
                {
                    sbOut.Append("Lookup Exist Record Error:" + e.Message + ":" + e.StackTrace);
                    resourceKey = 0;
                };
                if (resourceKey == 0)
                {
                    sbOut.Append("Resource does not exist in database. Cannot save VOTable version.");
                    return -1;
                }

                string resource = GetVOTableCacheRow(theXML, sbOut);
                string interfaces = GetVOTableCacheInterfaceRows(theXML, sbOut);
                SqlCommand command = SQLHelper.GetInsertVOTableCmd(resourceKey.ToString(), resource, interfaces, conn);

                // Make the DB Connection/Transaction for Loading in XML
                //conn.Open();

                SqlTransaction transaction;

                // Start a local transaction.
                transaction = conn.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = conn;
                command.Transaction = transaction;

                int rows = 0;
                try
                {
                    rows = command.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();

                    if (rows <= 0)
                    {
                        //sbOut.Append(" No error given, but failed to write resource to DB.");
                        iReturn = -1;
                    }
                }
                catch (Exception ex)
                {
                    sbOut.Append( " ivoid: " + ivoid + " Commit Exception:" + ex.GetType());
                    sbOut.Append("  Message: " + ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        sbOut.Append(" Rollback Exception:" + ex2.GetType());
                        sbOut.Append("  Message: " + ex2.Message + ". ");
                    }
                    iReturn = -1;
                }
            }
            finally
            {
                conn.Close();
            } 


            return iReturn;
        }

        private string GetVOTableCacheInterfaceRows(string theXML, StringBuilder sbOut)
        {
            string ifaces = string.Empty;

            try
            {
                string sres = theXML;
                StringReader reader = new StringReader(sres);
                XPathDocument myXPathDoc = new XPathDocument(reader);
                XslCompiledTransform myXslTrans = new XslCompiledTransform();

                //load the Xsl 
                StringBuilder sbXSL = new StringBuilder();
                StringWriter sw = new StringWriter(sbXSL);
                myXslTrans.Load(appDir + "xsl\\InterfaceViewResults_one_resource.xsl");

                XmlTextWriter writer = new XmlTextWriter(sw);
                myXslTrans.Transform(myXPathDoc, writer);

                string rawIfaces = sbXSL.ToString();

                int iTR = rawIfaces.IndexOf("<TR>");
                while (iTR > -1)
                {
                    ifaces += rawIfaces.Substring(iTR, rawIfaces.IndexOf("</TR>", iTR) - iTR + 5);
                    iTR = rawIfaces.IndexOf("<TR>", iTR + 1);
                }

            }
            catch (Exception e)
            {
                sbOut.Append("Xsl Load Exception" + e.Message + ":" + e.StackTrace);
                return string.Empty;
            };

            return ifaces;
        }

        private string GetVOTableCacheRow(string theXML, StringBuilder sbOut)
        {
            string resource = string.Empty;
            try
            {
                string sres = theXML;
                StringReader reader = new StringReader(sres);
                XPathDocument myXPathDoc = new XPathDocument(reader);
                XslCompiledTransform myXslTrans = new XslCompiledTransform();

                //load the Xsl 
                StringBuilder sbSQL = new StringBuilder();
                StringWriter sw = new StringWriter(sbSQL);
                myXslTrans.Load(appDir +  "xsl\\RegistryResults_vot_one_resource.xsl");

                XmlTextWriter writer = new XmlTextWriter(sw);
                myXslTrans.Transform(myXPathDoc, writer);

                resource = sbSQL.ToString();
                resource = resource.Substring(resource.IndexOf("<TR>"));
                resource = resource.Substring(0, resource.IndexOf("</TR>") + 5);
            }
            catch (Exception e)
            {
                sbOut.Append("Xsl Load Exception" + e.Message + ":" + e.StackTrace);
                return string.Empty;
            };

            return resource;
        }
    }
}
