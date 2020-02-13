using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlackJack.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }

        [HttpPost]
        public ActionResult Index(String txtName)
        {
            //validation fot the bet input
            if(!String.IsNullOrEmpty(txtName)){
                Session["Name"] = txtName;
            }

            return Redirect("Game/Bet");
        }
    }
}