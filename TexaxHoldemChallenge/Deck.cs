using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemChallenge
{
    class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        public Deck() {
            cards = new List<Card>();
            for (int suit = 0; suit <= 3; suit++) {
                for (int value = 2; value <= 14; value++) {
                    cards.Add(new Card(suit, value));
                }
            }
        }

        public Deck(List<Card> initialCards) {
            cards = new List<Card>(initialCards);
        }

        public int Count { get { return cards.Count; } }

        public void Add(Card cardToAdd) {
            cards.Add(cardToAdd);
        }

        public Card Deal(int index) {
            Card cardToDeal = cards[index];
            cards.RemoveAt(index);
            return cardToDeal;
        }

        public Card Peek(int index) {
            return cards[index];
        }

        public void Shuffle() {
            List<Card> newDeck = new List<Card>();
            while (cards.Count > 0) {
                int cardToMove = random.Next(cards.Count);
                newDeck.Add(cards[cardToMove]);
                cards.RemoveAt(cardToMove);
            }
            cards = newDeck;
        }

    }
}
