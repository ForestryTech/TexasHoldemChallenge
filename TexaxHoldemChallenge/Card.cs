using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemChallenge
{
    class Card
    {
        public Suits Suit { get; set; }
        public Values Value { get; set; }

        public string Name {
            get {
                string cardValue;
                string cardSuit = "";
                switch ((int)Value) {
                    case 11:
                        cardValue = "J";
                        break;
                    case 12:
                        cardValue = "Q";
                        break;
                    case 13:
                        cardValue = "K";
                        break;
                    case 14:
                        cardValue = "A";
                        break;
                    default:
                        cardValue = ((int)Value).ToString();
                        break;
                }

                switch ((int)Suit) {
                    case 0:
                        cardSuit = "\u2660";
                        break;
                    case 1:
                        cardSuit = "\u2665";
                        break;
                    case 2:
                        cardSuit = "\u2663";
                        break;
                    case 3:
                        cardSuit = "\u2666";
                        break;
                }

                return cardValue + cardSuit;
            }
        }

        public override string ToString() {
            return Name;
        }

        public Card(int suit, int value) {
            Suit = (Suits)suit;
            Value = (Values)value;
        }

        public Card(Suits suit, Values value) {
            Suit = suit;
            Value = value;
        }

        public Card() { }
    }
}
