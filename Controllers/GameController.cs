using BlackJack.Models;
using BlackJack.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


//todo: figure out how to set the balance.  Where should it happen? 

namespace BlackJack.Controllers
{
    public class GameController : Controller
    {

        // GET: Game
        public ActionResult Bet()
        {
            if((double)Session["Balance"] < 1)
            {
                Session.Abandon();
                return View("GameOver");
            }
            else
            {
                Session["Bet"] = 0;
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult Bet(String txtBet)
        {
            //validation for the bet input
            int bet;
            if (!int.TryParse(txtBet, out bet) ||
                bet < 1 ||
                bet > (double)Session["Balance"] ||
                bet > 100)
            {
                return View("Bet");
            }
            else
            {
                Session["Bet"] = bet;
                return Redirect("InitializeGame");
            }
                
        }


        public void ResetGame()
        {
            //shuffle Deck
            Game.Shuffle(Session["Deck"] as Card[]);
            //Clear hands
            (Session["PlayerCards"] as List<Card>).Clear();
            (Session["DealerCards"] as List<Card>).Clear();
            //Set top card to 0
            Session["LastIndex"] = 0;
            //reset Outcome
            Session["Outcome"] = null;
            Session["OutcomeMessage"] = null;
        }

        //Initial Load
        public ActionResult InitializeGame()
        {
            //If Bet is not set or is 0, redirect to bet page
            if(Session["Bet"] == null || (int)Session["Bet"] == 0)
            {
                return Redirect("Bet");
            }

            //if Balance is 0 then its game over
            if((double)Session["Balance"] == 0)
            {
                Session.Abandon();
                return Redirect("GameOver");
            }

            ResetGame();
            
            //get deck
            Card[] deck = Session["Deck"] as Card[];

            //Deal cards
            (Session["PlayerCards"] as List<Card>).Add(deck[0]);
            (Session["PlayerCards"] as List<Card>).Add(deck[2]);

            (Session["DealerCards"] as List<Card>).Add(deck[1]);
            (Session["DealerCards"] as List<Card>).Add(deck[3]);
            
            //set the next available card in the deck to 4
            Session["LastIndex"] = 4;

            //Play state determines wether the second dealers card is revealed or not
            Session["DealerCardState"] = "flipped";

            //***************************************************************** CHECK FOR PLAYER BLACKJACK
            if (Game.HasBlackjack(Session["PlayerCards"] as List<Card>))
            {
                //check if dealer has blackjack
                if (Game.HasBlackjack(Session["DealerCards"] as List<Card>))
                {
                    //reveal turned over card
                    Session["DealerCardState"] = "visible";
                    Session["Outcome"] = Outcomes.Tie;
                    Session["OutcomeMessage"] = "You both got BlackJack!";
                    SetBalance(Outcomes.Tie);
                    
                }
                else
                {
                    Session["Outcome"] = Outcomes.BlackJack;
                    Session["OutcomeMessage"] = "You got Blackjack!";
                    SetBalance(Outcomes.BlackJack);
                }
            }

            return Redirect("Load");

        }

        //function that loads the page with Session Objects
        public ActionResult Load()
        {
            GameViewModel gameViewModel = new GameViewModel();

            gameViewModel.PlayerCards = Session["PlayerCards"] as List<Card>;
            gameViewModel.DealerCards = Session["DealerCards"] as List<Card>;

            ViewBag.PlayerScore = Game.CalculateLowScore(gameViewModel.PlayerCards);

            return View("Play", gameViewModel);
        }
       
        //Player Hits
        public ActionResult Hit()
        {
            Card[] deck = Session["Deck"] as Card[];
            List<Card> cards = Session["PlayerCards"] as List<Card>;
            int lastIndex = (int)Session["LastIndex"];

            cards.Add(deck[lastIndex]);

            lastIndex++;

            Session["LastIndex"] = lastIndex;
            Session["PlayerCards"] = cards;

            //****************************************************************** CHECK SCORE AFTER A HIT

            if (Game.CalculateHighScore(Session["PlayerCards"] as List<Card>) > 21)
            {
                //Player Busts
                //only outcome where dealer's card isn't revealed
                Session["Outcome"] = Outcomes.Lose;
                Session["OutcomeMessage"] = "You Bust!";
                SetBalance(Outcomes.Lose);
            }

            if (Game.CalculateHighScore(Session["PlayerCards"] as List<Card>) == 21)
            {
                //Reveal dealer's card
                Session["DealerCardState"] = "visible";
                //Player gets 21
                //Check if dealer's hand is 21
                if (Game.CalculateHighScore(Session["DealerCards"] as List<Card>) == 21)
                {
                    Session["Outcome"] = Outcomes.Lose;
                    Session["OutcomeMessage"] = "Dealer has Blackjack!";
                    SetBalance(Outcomes.Lose);
                    
                }
                else
                {
                    Session["Outcome"] = Outcomes.Win;
                    Session["OutcomeMessage"] = "You got 21!";
                    SetBalance(Outcomes.Win);
                }

            }

            return Redirect("Load");

        }


        public ActionResult Stand()
        {
            //reveal dealer's card:
            Session["DealerCardState"] = "visible";

            Card[] deck = Session["Deck"] as Card[];
            List<Card> dealerCards = Session["DealerCards"] as List<Card>;
            int lastIndex = (int)Session["LastIndex"];

            //get dealer's current score
            int dealerScore = Game.CalculateHighScore(dealerCards);

            //**************************************************************** CHECK FOR DEALER BLACKJACK
            if (Game.HasBlackjack(dealerCards))
            {
                Session["Outcome"] = Outcomes.Lose;
                Session["OutcomeMessage"] = "Dealer Had BlackJack!";
                SetBalance(Outcomes.Lose);

                return Redirect("Load");
            }
            
            //dealer hits while their score is less than 17
            while(dealerScore < 17)
            {
                dealerCards.Add(deck[lastIndex]);
                lastIndex++;
                dealerScore = Game.CalculateHighScore(dealerCards);
            }

            Session["LastIndex"] = lastIndex;
            Session["DealerCards"] = dealerCards;

            //check dealer score once they have surpassed 17 points
            if(Game.CalculateHighScore(dealerCards) > 21)
            {
                //dealer busts
                Session["Outcome"] = Outcomes.Win;
                Session["OutcomeMessage"] = "Dealer Busts!";
                SetBalance(Outcomes.Win);
            }
            
            else
            {
                //compare hands
                Session["Outcome"] = Game.GetOutcome(Session["PlayerCards"] as List<Card>, Session["DealerCards"] as List<Card>).ToString();

                if(Session["Outcome"].ToString() == Outcomes.Lose.ToString())
                {
                    Session["OutcomeMessage"] = "Dealer's hand beats yours";
                    SetBalance(Outcomes.Lose);
                }
                else if(Session["Outcome"].ToString() == Outcomes.Win.ToString())
                {
                    Session["OutcomeMessage"] = "Your hand wins!";
                    SetBalance(Outcomes.Win);
                }
                else
                {
                    Session["OutcomeMessage"] = "You Tie";
                    SetBalance(Outcomes.Tie);
                }
            }

            
            return Redirect("Load");

        }

        public void SetBalance(Outcomes outcome)
        {
            Session["Balance"] = Game.SetBalance(outcome, (double)Session["Balance"], (int)Session["Bet"]);

        }

    }
}