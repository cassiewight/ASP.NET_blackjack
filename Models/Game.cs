using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackJack.Models
{
    public class Game
    {

        public static Card[] LoadDeck()
        {
            Card[] deck = new Card[52];

            //fill the cards array with new Cards
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    deck[i * 13 + j] = new Card((Suits)i, j + 1, VirtualPathUtility.ToAbsolute(String.Format("~/Content/images/{0}_{1}.svg", (Suits)i, j + 1)));
                }
            }
            return deck;
        }

        public static void Shuffle(Card[] deck)

        {
            Random r = new Random();
            for (int i = 0; i < deck.Length; i++)
            {

                Card tmp = deck[i];
                int x = r.Next(0, deck.Length - 1);
                deck[i] = deck[x];
                deck[x] = tmp;
            }
        }

        //helper method to calculate wether player busts (assumes aces are worth 1)
        public static int CalculateLowScore(List<Card> cards)
        {
            int score = 0;
            foreach (Card c in cards)
            {
                if (c.Rank > 9)
                {
                    score += 10;
                }
                else
                {
                    score += c.Rank;
                }
            }
            return score;
        }

        //helper method to calculate best possible blackjack score 
        public static int CalculateHighScore(List<Card> cards)
        {
            int score = 0;
            int aces = 0;
            foreach (Card c in cards)
            {
                if (c.Rank == 1)
                {
                    aces++;
                }
                else if (c.Rank > 9)
                {
                    score += 10;
                }
                else
                {
                    score += c.Rank;
                }
            }

            //every ace we just add one.  The last ace we check to see if we can add 11
            for (int i = 0; i < aces; i++)
            {
                if (i + 1 == aces)
                {
                    if (score + 11 <= 21)
                    {
                        score += 11;
                    }
                    else
                    {
                        score++;
                    }
                }
                else
                {
                    score++; ;
                }
            }


            return score;
        }

        public static Outcomes GetOutcome(List<Card> player, List<Card> dealer)
        {
            Outcomes outcome;

            if(CalculateHighScore(player) > CalculateHighScore(dealer))
            {
                outcome = Outcomes.Win;
            }
            else if(CalculateHighScore(player) < CalculateHighScore(dealer))
            {
                outcome = Outcomes.Lose;
            }
            else
            {
                outcome = Outcomes.Tie;
            }

            return outcome;

        }

        public static bool HasBlackjack(List<Card> cards)
        {
            List<Card> newList = new List<Card>();
            newList.Add(cards[0]);
            newList.Add(cards[1]);

            if (CalculateHighScore(newList) == 21)
            {
                return true;
            }
            else
                return false;
        }

        public static double SetBalance(Outcomes outcome, double balance, double bet)
        {
  
            if (outcome == Outcomes.BlackJack)
            {
                balance += bet * 1.5;
            }
            else if (outcome == Outcomes.Lose)
            {
                balance -= bet;
            }
            else if (outcome == Outcomes.Win)
            {
                balance += bet;
            }

            return balance;
        }

    }

    //possible player outcomes
    public enum Outcomes
    {
        Win, Lose, Tie, BlackJack
    }
}