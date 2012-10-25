using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using cardeira.p2s;

namespace cardeira
{
    public partial class Conversion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Handler1.PRequest(Context);
        }
    }
}