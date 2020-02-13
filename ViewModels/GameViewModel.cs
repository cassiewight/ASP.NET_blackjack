using BlackJack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BlackJack.ViewModels
{
    public class GameViewModel
    {
        public List<Card> PlayerCards { get; set; }
        public List<Card> DealerCards { get; set; }

    }
}