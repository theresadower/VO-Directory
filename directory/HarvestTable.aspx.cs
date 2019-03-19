using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace OperationsManagement
{
    public enum HarvestStatus
    {
        Success, //0
        Failure,
        PartialFailure,
        NoRecords,
        Unknown
    };

    public class HarvestSourceInfo
    {
        public string url = string.Empty;
        public string id = string.Empty;
        public bool isHarvesting = false;

        public HarvestSourceInfo(string url, string id, bool isHarvesting)
        {
            this.url = url;
            this.id = id;
            this.isHarvesting = isHarvesting;
        }
    }

    public partial class HarvestTable : System.Web.UI.Page
    {
        protected static string connect = (string)System.Configuration.ConfigurationManager.AppSettings["SqlConnection"];
        protected void Page_Load(object sender, EventArgs e)
        {
            GetLocalInfo();
            GetTotals();
            GetHarvesterLogTable(HarvesterTable);
        }

        protected void GetLocalInfo()
        {
            string db = "unknown host";
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connect);
                db = builder.DataSource + " / " + builder.InitialCatalog;
            }
            catch (Exception) { }
            LocalInfo.Text = "Reporting Resources in Registry at " + db;
        }

        protected void GetTotals()
        {
            TotalResources.Text = "Total Active Resources in Registry: ";
            TotalCount.Text = "Total Active Local Resources: ";
            HarvestedCount.Text = "Total Active Harvested Resources: ";

            SqlConnection conn = new SqlConnection(connect);
            try
            {
                conn = new SqlConnection(connect);
                conn.Open();

                string sGetEntry = "select count(*) from resource where (harvestedFromID is null or " +
                                   "harvestedFromID = '' or harvestedFromID like '%STScI%') and [rstat] = 1";
                SqlDataAdapter sqlDA = new SqlDataAdapter(sGetEntry, conn);
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    TotalCount.Text += row[0].ToString();
                }
                sGetEntry = "select count(*) from resource where (harvestedFrom is not null and " +
                                   "harvestedFrom <> '' and (not harvestedFrom like 'STScI%') ) and [rstat] = 1";
                sqlDA = new SqlDataAdapter(sGetEntry, conn);
                ds = new DataSet();
                sqlDA.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    HarvestedCount.Text += row[0].ToString();
                }
                sGetEntry = "select count(*) from resource where [rstat] = 1";
                sqlDA = new SqlDataAdapter(sGetEntry, conn);
                ds = new DataSet();
                sqlDA.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    TotalResources.Text = "<b>" + TotalResources.Text + row[0].ToString() + "</b>";
                }
            }
            finally
            {
                conn.Close();
            }
        }

        protected void GetHarvesterLogTable(Table HarvestTable)
        {
            SqlConnection conn = new SqlConnection(connect);

            HarvestSourceInfo[] info = null;
            int[] countArray = null;

            try
            {
                conn = new SqlConnection(connect);
                conn.Open();

                GetHarvestSourceInfo(conn, ref info);

                DateTime date = DateTime.MinValue;
                string message = String.Empty;
                HarvestStatus status = HarvestStatus.Unknown;


                TableRow trow = new TableRow();
                trow.BackColor = System.Drawing.Color.LightBlue;
                TableCell tcell = new TableCell();
                tcell.Controls.Add(new LiteralControl("<b><center>Remote Registry OAI Interface</center></b>"));
                trow.Cells.Add(tcell);
                //tcell = new TableCell();
                //tcell.Controls.Add(new LiteralControl("<b><center>Active Resources</center></b>"));
                //trow.Cells.Add(tcell);
                tcell = new TableCell();
                tcell.Controls.Add(new LiteralControl("<b><center>Automatically Harvested</center></b>"));
                trow.Cells.Add(tcell);
                tcell = new TableCell();
                tcell.Controls.Add(new LiteralControl("<b><center>Last Harvested (UTC)</center></b>"));
                trow.Cells.Add(tcell);
                tcell = new TableCell();
                tcell.Controls.Add(new LiteralControl("<b><center>Result of Last Harvest</center></b>"));
                trow.Cells.Add(tcell);
                tcell = new TableCell();
                tcell.Controls.Add(new LiteralControl("<b><center>Last Harvest Details</center></b>"));
                trow.Cells.Add(tcell);

                HarvestTable.Rows.Add(trow);
                GetResourcesCount(conn, ref info, ref countArray);

                for (int i = 0; i < info.Length; ++i)
                {
                    string url = info[i].url;
                    GetLastLogEntry(url, conn, ref date, ref message, ref status);

                    trow = new TableRow();
                    tcell = new TableCell();
                    tcell.VerticalAlign = VerticalAlign.Top;
                    tcell.Controls.Add(new LiteralControl(url));
                    trow.Cells.Add(tcell);
                    //tcell = new TableCell();
                    //tcell.VerticalAlign = VerticalAlign.Top;
                    //tcell.HorizontalAlign = HorizontalAlign.Center;
                    //tcell.Controls.Add(new LiteralControl(countArray[i].ToString()));
                    //trow.Cells.Add(tcell);


                    tcell = new TableCell();
                    tcell.VerticalAlign = VerticalAlign.Top;
                    tcell.HorizontalAlign = HorizontalAlign.Center;
                    if (info[i].isHarvesting)
                    {
                        tcell.Controls.Add(new LiteralControl("Yes"));
                    }
                    else
                    {
                        tcell.Controls.Add(new LiteralControl("No"));
                        trow.BackColor = System.Drawing.Color.LightGray;
                    }
                    trow.Cells.Add(tcell);

                    tcell = new TableCell();
                    tcell.VerticalAlign = VerticalAlign.Top;
                    if (date > DateTime.MinValue)
                        tcell.Controls.Add(new LiteralControl(date.ToUniversalTime().ToString()));
                    else
                        tcell.Controls.Add(new LiteralControl("Never"));
                    trow.Cells.Add(tcell);

                    tcell = new TableCell();
                    tcell.VerticalAlign = VerticalAlign.Top;
                    if (status == HarvestStatus.Success) 
                    {
                        tcell.Controls.Add(new LiteralControl("Success"));
                        tcell.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (status == HarvestStatus.Failure)
                    {
                        tcell.Controls.Add(new LiteralControl("Failure"));
                        tcell.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (status == HarvestStatus.PartialFailure)
                    {
                        tcell.Controls.Add(new LiteralControl("Partial Failure"));
                        tcell.ForeColor = System.Drawing.Color.DarkGoldenrod;
                    }
                    else if (status == HarvestStatus.NoRecords) //no updates
                    {
                        tcell.Controls.Add(new LiteralControl("Success"));
                        tcell.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (status == HarvestStatus.Unknown)
                    {
                        if (date == DateTime.MinValue)
                        {
                            tcell.Controls.Add(new LiteralControl(""));
                        }
                        else if (date.AddHours(2) < DateTime.Now)
                        {
                            tcell.Controls.Add(new LiteralControl("Did Not Complete"));
                            tcell.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                            tcell.Controls.Add(new LiteralControl("Waiting..."));
                    }
                    trow.Cells.Add(tcell);

                    //todo - fix the logging so this isn't so shaky.
                    string truncMessage = String.Empty;
                    if (status != HarvestStatus.Unknown)
                    {
                        truncMessage = "From " + message.Replace("Wrote DB inserts.", "").Replace(url, "");
                        int inHarvest = truncMessage.IndexOf("Harvesting") + 11;
                        int inGot = truncMessage.IndexOf("Got");
                        if (inHarvest > 0 && inGot > 0)
                            truncMessage = truncMessage.Remove(inHarvest, inGot - inHarvest);
                        while (truncMessage.Contains("resumption"))
                        {
                            try
                            {
                                int startVerb = truncMessage.IndexOf("?verb");
                                int endToken = truncMessage.IndexOf("Got", truncMessage.IndexOf("resumption")) - 1;
                                if (endToken > 0)
                                    truncMessage = truncMessage.Remove(startVerb, endToken - startVerb);
                                else //in case we have logged an error.
                                    truncMessage = truncMessage.Remove(startVerb, truncMessage.IndexOf("Harvester", startVerb) - startVerb);
                            }
                            catch (Exception)
                            {
                                truncMessage = "Harvest success but error reading total records loaded.";
                            }
                        }
                        if (truncMessage.Contains("recs") && truncMessage.Contains("Loaded"))
                        {
                            int recs = truncMessage.IndexOf("recs") + 4;
                            int loaded = truncMessage.IndexOf("Loaded");
                            truncMessage = truncMessage.Remove(recs, loaded - recs).Insert(recs, ". ");
                        }
                    }
                    tcell = new TableCell();
                    tcell.VerticalAlign = VerticalAlign.Top;
                    tcell.Controls.Add(new LiteralControl(truncMessage));
                    trow.Cells.Add(tcell);

                    HarvestTable.Rows.Add(trow);
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public void GetHarvestSourceInfo(SqlConnection conn, ref HarvestSourceInfo[] info)
        {
            string sGetEntry = "select distinct serviceurl, registryid, doharvest from harvester";

            SqlDataAdapter sqlDA = new SqlDataAdapter(sGetEntry, conn);
            DataSet ds = new DataSet();
            sqlDA.Fill(ds);

            info = new HarvestSourceInfo[ds.Tables[0].Rows.Count];

            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {
                DataRow row = ds.Tables[0].Rows[i];
                info[i] = new HarvestSourceInfo((string)row[0], (string)row[1], (bool)row[2]);
            }
        }

        private void GetResourcesCount(SqlConnection conn, ref HarvestSourceInfo[] info, ref int[] count)
        {
            string sGetCount = "select harvestedFrom, count(*) from Resource " +
                               "where  [rstat] = 1 and (harvestedFrom is not null and harvestedFrom <> '' and harvestedFrom not like 'STScI%') " +
                               "group by harvestedFrom";

            SqlDataAdapter sqlDA = new SqlDataAdapter(sGetCount, conn);
            DataSet ds = new DataSet();
            sqlDA.Fill(ds);

            count = new int[info.Length];
            for (int thiscount = 0; thiscount < count.Length; ++thiscount)
                count[thiscount] = 0;

            for (int i = 0; i < ds.Tables[0].Rows.Count; ++i)
            {
                DataRow data = ds.Tables[0].Rows[i];

                for (int j = 0; j < info.Length; ++j)
                {
                    if (info[j].url.CompareTo((string)data[0]) == 0)
                    {
                        count[j] += (int)data[1];
                        break;
                    }
                }
            }
        }

        private void GetLastLogEntry(string harvestURL, SqlConnection conn, ref DateTime date, ref string message, ref HarvestStatus status)
        {
            string sGetEntry = "select top 1 date, message, status " +
                               "from harvesterlog where serviceurl = '" +
                               harvestURL + "' and status is not null " +
                               "order by date desc";

            SqlDataAdapter sqlDA = new SqlDataAdapter(sGetEntry, conn);
            DataSet ds = new DataSet();
            sqlDA.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow data = ds.Tables[0].Rows[0];
                date = (DateTime)data[0];
                if (!(data[1] is System.DBNull))
                    message = (String)data[1];
                if (data[2] is System.DBNull)
                    status = HarvestStatus.Unknown;
                else
                    status = (HarvestStatus)(int)data[2];
            }
            else
            {
                date = DateTime.MinValue;
                message = String.Empty;
                status = HarvestStatus.Unknown;
            }
        }
    }
}