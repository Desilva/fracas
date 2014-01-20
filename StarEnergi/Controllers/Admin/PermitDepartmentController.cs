using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class PermitDepartmentController : Controller
    {
        //
        // GET: /PermitDepartment/

        public ActionResult Index()
        {
            return PartialView();
        }

    }
}
