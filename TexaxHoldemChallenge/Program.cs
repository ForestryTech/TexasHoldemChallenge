using System;
using System.Collections.Generic;
using System.Linq;

namespace TexasHoldemChallenge
{
    class Program
    {
        static void Main() {
            Console.Write("How many players (2-8) ? ");
            string temp = null;
            int input = 0;
            bool badInput = true;
            while (badInput) {
                try {
                    temp = Console.ReadLine();
                    input = Convert.ToInt32(temp);
                    if (input >= 2 && input <= 8)
                        badInput = false;
                    else {
                        Console.Clear();
                        Console.Write("Must be a number between 2 - 8. Try again. ");
                    }
                }
                catch (FormatException) {
                    Console.Clear();
                    Console.Write("Invalid input : {0}\n", temp);
                    Console.Write("Must be a number between 2 - 8. Try again. ");
                }
            }

            
            int numberOfPlayers = input;
            Deck texasHoldemDeck = new Deck();
            List<Player> texasHoldemPlayers = new List<Player>();
            Card[] flop = new Card[3];
            Card river;
            Card turn;

            texasHoldemPlayers.Add(new Player("Your "));
            texasHoldemDeck.Shuffle();

            for (int i = 0; i < numberOfPlayers - 1; i++) {
                texasHoldemPlayers.Add(new Player("CPU " + (i+1)));
            }

            for (int i = 0; i < 2; i++) {
                foreach (Player player in texasHoldemPlayers) {
                    player.GetCard(texasHoldemDeck.Deal(1));
                }
            }

            for (int i = 0; i < 3; i++) {
                flop[i] = texasHoldemDeck.Deal(1);
            }
            river = texasHoldemDeck.Deal(1);
            turn = texasHoldemDeck.Deal(1);

            Console.Write("\n***** Current game status *****\n");
            foreach (Player player in texasHoldemPlayers) {
                Console.Write(player.ShowCards() + "\n");
            }
            Console.Write("Flop: ");
            for (int i = 0; i < 3; i++) {
                if (i == (flop.Count() - 1))
                    Console.Write(flop[i].Name);
                else
                    Console.Write(flop[i].Name + ", ");
            }
            Console.Write("\nRiver: {0}\nTurn: {1}\n", river.Name, turn.Name);
            Console.ReadLine();
            
        }
    }

    class Card
    {
        public Suits Suit { get; set; }
        public Values Value { get; set; }

        public string Name {
            get {
                return Value + " of " + Suit;
            }
        }

        public override string ToString() {
            return Name;
        }

        public Card(int suit, int value) {
            Suit = (Suits)suit;
            Value = (Values)value;
        }

        public Card() { }
    }

    class Player
    {
        internal protected List<Card> cards;
        private string playerName;
        private double cash;
        public Hand PlayerHand { get; set; }

        public double Bet { get; set; }
        public bool Playing { get; private set; }

        public void GetCard(Card card) {
            cards.Add(card);
        }

        public string ShowCards() {
            string currentHand = playerName + " hand: ";
            for (int i = 0; i < cards.Count(); i++) {
                if (i == (cards.Count() - 1))
                    currentHand += cards[i].Name;
                else
                    currentHand += cards[i].Name + ", ";
            }
            return currentHand;
        }

        public Player(string name) {
            cards = new List<Card>();
            playerName = name;
        }

        public Player(List<Card> cards) {
            this.cards = cards;
        }
    }

    class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        public Deck() {
            cards = new List<Card>();
            for (int suit = 0; suit <= 3; suit++) {
                for (int value = 1; value <= 13; value++) {
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

    class Hand
    {
        public HandType Type {
            get {
                List<Card> allCards = new List<Card>();
                foreach (Card card in player.cards) {
                    allCards.Add(card);
                }
                foreach (Card card in game.CommunityCards) {
                    allCards.Add(card);
                }

                
                return HandType.Flush;
            }
        }
        
        public Card PrimaryHigh { get {
            Card tempCard;
            player.cards.Sort(new CardComparerValue());
            tempCard = player.cards[0];
            return tempCard;
        } }
        
        public Card SecondaryHigh { get {
            Card tempCard;
            player.cards.Sort(new CardComparerValue());
            tempCard = player.cards[1];
            return tempCard;
        } }

        private Player player;
        private Game game;

        public Hand(Player player) {
            this.player = player;
        }

        private bool isFlush(List<Card> cards) {
            int[] count = new int[4];
            foreach (Card card in cards) {
                count[(int)card.Suit]++;
                if (count[(int)card.Suit] >= 5)
                    return true;
            }
            return false;
        }

        private bool isStraight(List<Card> cards) {
            cards.Sort(new CardComparerValue());
            int straightCount = 0;
            int straightValue = (int)cards[0].Value;
            int straigtValueToCheck;

            // Check for straight with Aces high
            for (int i = 1; i < cards.Count(); i++) {
                straigtValueToCheck = (int)cards[i].Value;
                if ((straightValue + 1) == straigtValueToCheck) {
                    straightValue++;
                    straightCount++;
                    if (straightCount >= 4)
                        return true;
                } else {
                    straightValue = straigtValueToCheck;
                    straightCount = 0;
                }
            }
            // Check for straight with Aces low
            cards.Sort(new CardCompareAcesLow());
            straightCount = 0;
            straightValue = ((int)cards[0].Value == 14) ? 1 : (int)cards[0].Value;
            for (int i = 1; i < cards.Count(); i++) {
                straigtValueToCheck = ((int)cards[i].Value == 14) ? 1 : (int)cards[i].Value;
                if ((straightValue + 1) == straigtValueToCheck) {
                    straightValue++;
                    straightCount++;
                    if (straightCount >= 4)
                        return true;
                } else {
                    straightValue = straigtValueToCheck;
                    straightCount = 0;
                }
            }
            return false;

        
        }

        private bool isFullHouse(List<Card> cards) {
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            bool threeOfAKind = false;
            bool pair = false;

            foreach (Card card in cards) {
                if (count.ContainsKey(card.Value)) {
                    count[card.Value]++;
                } else {
                    count.Add(card.Value, 1);
                }
            }
            foreach (var item in count) {
                if (item.Value == 3)
                    threeOfAKind = true;
                if (item.Value == 2)
                    pair = true;
            }
            if (threeOfAKind && pair)
                return true;
            else
                return false;
        }

        private bool isFourOfAKind(List<Card> cards) {
            var count = cards.GroupBy(c => c.Value).Count();
            if (count == 4)
                return true;
            else
                return false;
        }

        private bool isThreeOfAKind(List<Card> cards) {

        }

        private bool isTwoPair(List<Card> cards) {

        }

        private bool isPair(List<Card> cards) {

        }
    }

    class Game
    {
        internal List<Card> CommunityCards;

        internal List<Player> Players;
        internal Deck GameDeck;

        public Game() {
            CommunityCards = new List<Card>();
            Players = new List<Player>();
            GameDeck = new Deck();
        }
    }
    enum Values
    {
        Ace = 14,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
    }

    enum Suits
    {
        Spades,
        Hearts,
        Clubs,
        Diamonds,
    }

    enum HandType
    {
        StraightFlush = 1,
        FourOfAKind = 2,
        FullHouse = 3,
        Flush = 4,
        Straight = 5,
        ThreeOfAKind = 6,
        TwoPair = 7,
        OnePair = 8,
        HighCard = 9,
    }

    class CardComparerValue : IComparer<Card>
    {
        public int Compare(Card x, Card y) {
            if (x.Value > y.Value)
                return 1;
            if (x.Value < y.Value)
                return -1;
            else
                return 0;
        }
    }

    class CardCompareAcesLow : IComparer<Card>
        {
            public int Compare(Card x, Card y) {
                if (x.Value == Values.Ace)
                    return -1;
                if (x.Value > y.Value)
                    return 1;
                if (x.Value < y.Value)
                    return -1;
                else
                    return 0;
            }
        }
    
    class CardComparerSuit : IComparer<Card>
    {
        public int Compare(Card x, Card y) {
            if (x.Suit > y.Suit)
                return 1;
            if (x.Suit < y.Suit)
                return -1;
            else
                return 0;
        }
    }
    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand x, Hand y) {
            if (x.Type > y.Type)
                return 1;
            if (x.Type < y.Type)
                return -1;
            else {
                if (x.PrimaryHigh.Value > y.PrimaryHigh.Value)
                    return 1;
                if (x.PrimaryHigh.Value < y.PrimaryHigh.Value)
                    return -1;
                else {
                    if (x.SecondaryHigh.Value > y.SecondaryHigh.Value)
                        return 1;
                    if (x.SecondaryHigh.Value < y.SecondaryHigh.Value)
                        return -1;
                    else 
                        return 0;
                }
            }
        }
    }
}
