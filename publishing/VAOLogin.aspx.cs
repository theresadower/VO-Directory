using System;
using System.Net;
using System.IO;
using System.Text;

namespace Publishing
{
    public partial class VAOLogin : System.Web.UI.Page
    {
        private static UserManagement userManager = new UserManagement();
        //private static string redirect = "resourcemanagement.html?debug";

        protected void Page_Load(object sender, EventArgs e)
        {
            //attempt to fill with whoami data
        }
    }
}