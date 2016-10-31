using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using oai;


namespace registry
{
    class ResumptionTokenUtil
    {
        private static string sqlConnection = (string)System.Configuration.ConfigurationManager.AppSettings["SqlConnection"];
        private static string sqlAdminConnection = (string)System.Configuration.ConfigurationManager.AppSettings["SqlAdminConnection"];

        public static ResumptionToken getResumptionToken(string tokenValue)
        {
            ResumptionToken token = null;
            if (tokenValue == null)
            {
                return token;
            }
            using (SqlConnection dbConn = new SqlConnection(sqlConnection))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = dbConn;
                command.CommandText = "select tokenValue,expirationDate,from,until,metadataPrefix,set from resumptionToken where token = @resumptionToken and expirationDate > @expirationDate";
                SqlParameter expirationDateParam = new SqlParameter("@expirationDate", SqlDbType.DateTime);
                expirationDateParam.Value = DateTime.Now.ToUniversalTime();
                SqlParameter resumptionTokenParam = new SqlParameter("@resumptionToken", SqlDbType.Text);
                resumptionTokenParam.Value = tokenValue;
                command.Parameters.Add(expirationDateParam);
                command.Parameters.Add(resumptionTokenParam);
                command.Prepare();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        token = new ResumptionToken();
                        reader.Read();
                        token.tokenValue = reader.GetString(0);
                        token.expirationDate = reader.GetDateTime(1);

                        if (!reader.IsDBNull(2))
                        {
                            token.from = reader.GetDateTime(2);
                        }
                        else
                        {
                            token.from = null;
                        }
                        if (!reader.IsDBNull(3))
                        {
                            token.until = reader.GetDateTime(3);
                        }
                        else
                        {
                            token.until = null;
                        }
                        if (!reader.IsDBNull(4))
                        {
                            token.metadataPrefix = reader.GetString(4);
                        }
                        else
                        {
                            token.metadataPrefix = null;
                        }
                        if (!reader.IsDBNull(5))
                        {
                            token.set = reader.GetString(5);
                        }
                        else
                        {
                            token.set = null;
                        }
                    }
                    // Check if the token exists in the database and if so remove it
                    if (token != null)
                    {
                        command = new SqlCommand();
                        command.Connection = dbConn;
                        command.CommandText = "delete from resumptionToken where tokenValue = @resumptionToken";
                        command.Parameters.Add(resumptionTokenParam);
                        command.Prepare();
                        int rows = command.ExecuteNonQuery();
                        // If the token exists and we removed it, then it was valid, so return true
                        return token;

                    }
                }
            }
            return token;
        }

        public static void saveResumptionToken(ResumptionToken token)
        {
            using (SqlConnection dbConn = new SqlConnection(sqlConnection))
            {
                //Todo -- eventually we ought to have a cleanup thread for this and temp files.
                //For now, delete old ones when creating new one since it's a rarely occuring and relevant time
                //to afford the slowdown.
                SqlCommand command = new SqlCommand();
                command.Connection = dbConn;
                command.CommandText = "delete from resumptionToken where expirationDate > @expirationDate";
                SqlParameter expirationDateParam = new SqlParameter("@expirationDate", SqlDbType.DateTime);
                expirationDateParam.Value = DateTime.Now.ToUniversalTime();
                command.Parameters.Add(expirationDateParam);
                command.Prepare();
                int rows = command.ExecuteNonQuery();

                command = new SqlCommand();
                command.Connection = dbConn;
                command.CommandText = "insert into resumptionToken(tokenValue, expirationDate, from, until, metadataPrefix, set) " +
                   "values (@resumptionToken,@expirationDate,@from,@until,@metadataPrefix,@set)";
                expirationDateParam = new SqlParameter("@expirationDate", SqlDbType.DateTime);
                expirationDateParam.Value = token.expirationDate;
                command.Parameters.Add(expirationDateParam);

                SqlParameter resumptionTokenParam = new SqlParameter("@resumptionToken", SqlDbType.Text);
                resumptionTokenParam.Value = token.tokenValue;
                command.Parameters.Add(resumptionTokenParam);
                
                SqlParameter fromParam = new SqlParameter("@from", SqlDbType.DateTime);
                fromParam.Value = token.from;
                command.Parameters.Add(fromParam);

                SqlParameter untilParam = new SqlParameter("@until", SqlDbType.DateTime);
                untilParam.Value = token.until;
                command.Parameters.Add(untilParam);
                command.Prepare();

                SqlParameter metadataPrefixParam = new SqlParameter("@metadataPrefix", SqlDbType.Text);
                metadataPrefixParam.Value = token.metadataPrefix;
                command.Parameters.Add(metadataPrefixParam);

                SqlParameter setParam = new SqlParameter("@set", SqlDbType.DateTime);
                setParam.Value = token.set;
                command.Parameters.Add(setParam);

                rows = command.ExecuteNonQuery();

            }
        }
    }
}
