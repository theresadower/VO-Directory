using System.Collections.Specialized;
using System.Web;

namespace Publishing
{
    public class WhoamiInfo
    {
        public string FirstName = "";
        public string LastName = "";
        public string EZID = "";
        public string IsInternal = "";
        public string Department = "";
        public string Email = "";
        public string IP = "";

        public WhoamiInfo(HttpRequest request, string defaultFirstName = "Anonymous", string defaultEZID = "anonymous") :
            this(request.Headers, defaultFirstName, defaultEZID)
        {
            IP = getIP(request);
        }

        public WhoamiInfo(NameValueCollection headers, string defaultFirstName = "Anonymous", string defaultEZID = "anonymous")
        {
            // Note: Internal and External users have different header values, so we need to check for both.  
            FirstName = getter(headers, "STScIFirstName", "givenName", defaultFirstName);
            LastName = getter(headers, "STScILastName", "sn", "");
            EZID = getter(headers, "STScIEZID", "remoteuser", defaultEZID);
            IsInternal = getter(headers, "STScIInternal", "", "defaultInt");
            Department = getter(headers, "STScIDepartment", "", "defaultDept");
            Email = getter(headers, "STScIEmail", "mail", "");
        }

        protected string getIP(HttpRequest request)
        {
            string ip = "none";
            if (request != null)
            {
                //
                // NOTE: 
                // If remote client is going through a web proxy, then the client IP is stored in HTTP_X_FORWARDED_FOR.
                // If client is not going through proxy, then the client IP is stored in REMOTE_ADDR.
                //
                if (request.ServerVariables.Get("HTTP_X_FORWARDED_FOR") != null)
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                else if (request.ServerVariables.Get("REMOTE_ADDR") != null)
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }
            }

            return ip;
        }

        protected string getter(NameValueCollection headers, string name1, string name2, string defval)
        {
            if (headers[name1] != null) return headers[name1];
            else if (headers[name2] != null) return headers[name2];
            else return defval;
        }
    }
}

