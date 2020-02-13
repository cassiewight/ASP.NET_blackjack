using BlackJack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BlackJack
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            Card[] deck = Game.LoadDeck();
            List<Card> dealerCards = new List<Card>();
            List<Card> playerCards = new List<Card>();

            Session["Balance"] = 500.00;

            Session["Deck"] = deck;
            Session["DealerCards"] = dealerCards;
            Session["PlayerCards"] = playerCards;

        }
    }
}
