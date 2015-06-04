using System;
using System.Collections.Generic;
using System.Linq;

namespace TexasHoldemChallenge
{
    class Program
    {
        static void Main() {
            Game texasHoldem = new Game();
            texasHoldem.Start();
            texasHoldem.Play();
            Console.ReadLine();
            
        }
    }

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

        public Card() { }
    }

    class Player
    {
        internal protected List<Card> cards;
        private string playerName;
        private double cash;
        public Hand PlayerHand;

        public double Bet { get; set; }
        public bool Playing { get; private set; }
        public string Name { get { return playerName; } }
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

        public Player(string name, Game game) {
            cards = new List<Card>();
            playerName = name;
            PlayerHand = new Hand(this, game);
            Playing = true;
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

    class Hand
    {
        public List<Card> allCards;
        public HandType Type {
            get {
                allCards.Clear();
                foreach (Card card in player.cards) {
                    allCards.Add(card);
                }
                foreach (Card card in game.CommunityCards) {
                    allCards.Add(card);
                }

                if (isStraight(allCards) && isFlush(allCards))
                    return HandType.StraightFlush;
                if (isFourOfAKind(allCards))
                    return HandType.FourOfAKind;
                if (isFullHouse(allCards))
                    return HandType.FullHouse;
                if (isFlush(allCards))
                    return HandType.Flush;
                if (isStraight(allCards))
                    return HandType.Straight;
                if (isThreeOfAKind(allCards))
                    return HandType.ThreeOfAKind;
                if (isTwoPair(allCards))
                    return HandType.TwoPair;
                if (isPair(allCards))
                    return HandType.OnePair;

                return HandType.HighCard;
                
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

        public Hand(Player player, Game game) {
            this.player = player;
            this.game = game;
            allCards = new List<Card>();
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
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            count = getCardDictionary(cards);
            foreach (var item in count) {
                if (item.Value == 4)
                    return true;
            }
            return false;
        }

        private bool isThreeOfAKind(List<Card> cards) {
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            count = getCardDictionary(cards);
            foreach (var item in count) {
                if (item.Value == 3)
                    return true;
            }
            return false;
        }

        private bool isTwoPair(List<Card> cards) {
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            bool firstPair = false;
            bool secondPair = false;
            count = getCardDictionary(cards);
            foreach (var item in count) {
                if ((item.Value == 2) && !firstPair) 
                    firstPair = true;
                else if (item.Value == 2)
                    secondPair = true;
                
            }

            if (firstPair && secondPair)
                return true;
            else
                return false;
        }

        private bool isPair(List<Card> cards) {
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            count = getCardDictionary(cards);
            foreach (var item in count) {
                if (item.Value == 2)
                    return true;
            }
            return false;
        }

        private Dictionary<Values, int> getCardDictionary(List<Card> cards) {
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            foreach (Card card in cards) {
                if (count.ContainsKey(card.Value)) {
                    count[card.Value]++;
                } else {
                    count.Add(card.Value, 1);
                }
            }
            return count;
        }
    }

    class Game
    {
        internal List<Card> CommunityCards;
        internal List<Player> Players;
        internal Deck GameDeck;
        private int numberOfPlayers;

        public bool Playing {get; set;}


        public Game() {
            CommunityCards = new List<Card>();
            Players = new List<Player>();
            GameDeck = new Deck();
        }

        public void Start() {
            getNumberOfPlayers();
            addPlayers();
            GameDeck.Shuffle();
        }

        public void Play() {
            deal(1);
            showCards(1);
            deal(2);
            showCards(2);
            deal(3);
            showCards(3);
            deal(4);
            showCards(4);
            // check for folding
            // display hands and handType
            Console.Write("\nResults\n");
            foreach (Player player in Players) {
                Console.Write("Player {0} has a hand of {1}\n", player.Name, player.PlayerHand.Type);
                Console.Write("Player cards: \n");
                player.PlayerHand.allCards.Sort(new CardComparerValue());
                foreach (Card c in player.PlayerHand.allCards) {
                    Console.Write("\t" + c.ToString() + " ");
                }
                Console.Write("\n\n");
            }
            getWinner();
            // check for winner
        }
        
        public void Reset() {

        }

        private void getWinner() {
            List<Player> winner = new List<Player>();
            int winningHand;
            int currentHand;
            winningHand = (int)Players[0].PlayerHand.Type;
            winner.Add(Players[0]);
            for (int i = 1; i < Players.Count(); i++) {
                currentHand = (int)Players[i].PlayerHand.Type;
                if (currentHand < winningHand) {
                    winner.Clear();
                    winner.Add(Players[i]);
                    winningHand = (int)Players[i].PlayerHand.Type;
                } else if (currentHand == winningHand) {
                    winner.Add(Players[i]);
                }
            }

            if (winner.Count() == 1) {
                Console.Write("Winner: {0} with a hand of {1}\n", winner[0].Name, winner[0].PlayerHand.Type);
            }
            if (winner.Count() > 1) {
                Console.Write("Ther is a tie!\n");
            }

        }

        private void showCards(int mode) {
            switch (mode) {
                case 1: // show player cards
                    Console.Write("\n********* Player Cards *********\n");
                    foreach (Player player in Players) {
                        Console.Write(player.ShowCards() + "\n");
                    }
                    break;
                case 2:
                    string flop = "\n********* The Flop *********\n";
                    flop += CommunityCards[0].ToString() + ", ";
                    flop += CommunityCards[1].ToString() + ", ";
                    flop += CommunityCards[2].ToString() + ", ";
                    Console.Write(flop);
                    break;
                case 3:
                    string river = "\n********* The River *********\n";
                    river += CommunityCards[3].ToString();
                    Console.Write(river);
                    break;
                case 4:
                    string turn = "\n********* The Turn *********\n";
                    turn += CommunityCards[4].ToString();
                    Console.Write(turn);
                    break;
            }
        }

        private void deal(int mode) {
            switch (mode) {
                case 1: // deal player cards
                    for (int i = 0; i < 2; i++) {
                        foreach (Player player in Players) {
                            player.GetCard(GameDeck.Deal(1));
                        }
                    }
                    break;
                case 2: // deal flop
                    for (int i = 0; i < 3; i++)
			        {
                        CommunityCards.Add(GameDeck.Deal(1));
			        }
                    break;
                case 3:
                    CommunityCards.Add(GameDeck.Deal(1));
                    break;
                case 4:
                    CommunityCards.Add(GameDeck.Deal(1));
                    break;
            }
        }

        private void addPlayers() {
            Players.Add(new Player("Your ", this));
            for (int i = 1; i < numberOfPlayers; i++) {
                Players.Add(new Player("CPU " + (i + 1), this));
            }
        }

        private void getNumberOfPlayers() {
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
            numberOfPlayers = input;
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
