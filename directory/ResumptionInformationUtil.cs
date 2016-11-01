using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using oai;


namespace registry
{
    class ResumptionInformationUtil
    {
        private static string sqlConnection = (string)System.Configuration.ConfigurationManager.AppSettings["SqlConnection"];
        private static string sqlAdminConnection = (string)System.Configuration.ConfigurationManager.AppSettings["SqlAdminConnection"];

        public static ResumptionInformation getResumptionInformation(string tokenValue, bool removeToken)
        {
            ResumptionInformation info = null;
            if (tokenValue == null)
            {
                return info;
            }
            using (SqlConnection dbConn = new SqlConnection(sqlAdminConnection))
            {
                dbConn.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = dbConn;
                command.CommandText = "select tokenValue,expirationDate,fromDate,untilDate,metadataPrefix,setName,startIdx,completeListSize from " +
                    "resumptionInformation where tokenValue = @resumptionToken and expirationDate > @expirationDate";
                SqlParameter expirationDateParam = new SqlParameter("@expirationDate", SqlDbType.DateTime);
                expirationDateParam.Value = DateTime.Now.ToUniversalTime();
                SqlParameter resumptionTokenParam = new SqlParameter("@resumptionToken", SqlDbType.VarChar,500);
                resumptionTokenParam.Value = tokenValue;
                command.Parameters.Add(expirationDateParam);
                command.Parameters.Add(resumptionTokenParam);
                command.Prepare();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        info = new ResumptionInformation();
                        reader.Read();
                        info.tokenValue = reader.GetString(0);
                        info.expirationDate = reader.GetDateTime(1);

                        if (!reader.IsDBNull(2))
                        {
                            info.from = reader.GetDateTime(2);
                        }
                        else
                        {
                            info.from = null;
                        }
                        if (!reader.IsDBNull(3))
                        {
                            info.until = reader.GetDateTime(3);
                        }
                        else
                        {
                            info.until = null;
                        }
                        if (!reader.IsDBNull(4))
                        {
                            info.metadataPrefix = reader.GetString(4);
                        }
                        else
                        {
                            info.metadataPrefix = null;
                        }
                        if (!reader.IsDBNull(5))
                        {
                            info.set = reader.GetString(5);
                        }
                        else
                        {
                            info.set = null;
                        }
                        
                        info.startIdx = reader.GetInt32(6);
                        info.completeListSize = reader.GetInt32(7);
                    }
                }
                    // Check if the token exists in the database and if so remove it
                if (info != null && removeToken)
                {
                    command = new SqlCommand();
                    command.Connection = dbConn;
                    command.CommandText = "delete from resumptionInformation where tokenValue = @resumptionToken";
                    resumptionTokenParam = new SqlParameter("@resumptionToken", SqlDbType.VarChar, 500);
                    resumptionTokenParam.Value = tokenValue;
                    command.Parameters.Add(resumptionTokenParam);
                    command.Prepare();
                    int rows = command.ExecuteNonQuery();
                    // If the token exists and we removed it, then it was valid, so return true
                }
            }
            return info;
        }

        public static void saveResumptionInformation(ResumptionInformation info)
        {
            using (SqlConnection dbConn = new SqlConnection(sqlAdminConnection))
            {
                //Todo -- eventually we ought to have a cleanup thread for this and temp files.
                //For now, delete old ones when creating new one since it's a rarely occuring and relevant time
                //to afford the slowdown.
                dbConn.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = dbConn;
                command.CommandText = "delete from ResumptionInformation where expirationDate > @expirationDate";
                SqlParameter expirationDateParam = new SqlParameter("@expirationDate", SqlDbType.DateTime);
                expirationDateParam.Value = DateTime.Now.ToUniversalTime();
                command.Parameters.Add(expirationDateParam);
                command.Prepare();
                int rows = command.ExecuteNonQuery();

                command = new SqlCommand();
                command.Connection = dbConn;
                command.CommandText = "insert into ResumptionInformation(tokenValue, expirationDate, fromDate, untilDate, metadataPrefix, setName, startIdx, completeListSize) " +
                   "values (@resumptionToken, @expirationDate, @from, @until, @metadataPrefix, @set, @start, @completeListSize)";
                expirationDateParam = new SqlParameter("@expirationDate", SqlDbType.DateTime);
                expirationDateParam.Value = info.expirationDate;
                command.Parameters.Add(expirationDateParam);

                SqlParameter resumptionTokenParam = new SqlParameter("@resumptionToken", SqlDbType.VarChar, 500);
                resumptionTokenParam.Value = info.tokenValue;
                command.Parameters.Add(resumptionTokenParam);
                
                SqlParameter fromParam = new SqlParameter("@from", SqlDbType.DateTime);
                fromParam.Value = info.from ?? Convert.DBNull;
                command.Parameters.Add(fromParam);

                SqlParameter untilParam = new SqlParameter("@until", SqlDbType.DateTime);
                untilParam.Value = info.until ?? Convert.DBNull;
                command.Parameters.Add(untilParam);

                SqlParameter metadataPrefixParam = new SqlParameter("@metadataPrefix", SqlDbType.VarChar, 100);
                metadataPrefixParam.Value = info.metadataPrefix ?? Convert.DBNull;
                command.Parameters.Add(metadataPrefixParam);

                SqlParameter setParam = new SqlParameter("@set", SqlDbType.VarChar, 100);
                setParam.Value = info.set ?? Convert.DBNull;
                command.Parameters.Add(setParam);

                SqlParameter startParam = new SqlParameter("@start", SqlDbType.Int);
                startParam.Value = info.startIdx;
                command.Parameters.Add(startParam);

                SqlParameter completeListSizeParam = new SqlParameter("@completeListSize", SqlDbType.Int);
                completeListSizeParam.Value = info.completeListSize;
                command.Parameters.Add(completeListSizeParam);

                command.Prepare();
                rows = command.ExecuteNonQuery();

            }
        }
    }
}
