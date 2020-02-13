using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackJack.Models
{
    public class Card
    {
        public Suits Suit { get; set; }
        public int Rank { get; set; }
        public String Path { get; set; }

        public Card(Suits s, int r, String path)
        {
            this.Suit = s;
            this.Rank = r;
            this.Path = path;
        }

        public override string ToString()
        {
            return String.Format("{0} of {1}", this.Rank, this.Suit.ToString());
        }
    }

    public enum Suits
    {
        Hearts, Diamonds, Spades, Clubs
    }
}