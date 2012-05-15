using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceHelper.VirtualPOS.Garanti.Sample.Controllers {

    public class HomeController : Controller {

        public ActionResult Index() {

            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index_post() {

            return View();
        }

    }
}