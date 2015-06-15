using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemChallenge
{
    class Hand
    {
        public List<Card> AllCards;
        public List<Card> BestHand {
            get {
                AllCards.Clear();

                foreach (Card card in player.cards) {
                    AllCards.Add(card);
                }
                foreach (Card card in game.CommunityCards) {
                    AllCards.Add(card);
                }
                return SortHand();
            }
        }

        public HandType Type {
            get {
                AllCards.Clear();
                foreach (Card card in player.cards) {
                    AllCards.Add(card);
                }
                foreach (Card card in game.CommunityCards) {
                    AllCards.Add(card);
                }

                if (isStraight(AllCards) && isFlush(AllCards))
                    return HandType.StraightFlush;
                if (isFourOfAKind(AllCards))
                    return HandType.FourOfAKind;
                if (isFullHouse(AllCards))
                    return HandType.FullHouse;
                if (isFlush(AllCards))
                    return HandType.Flush;
                if (isStraight(AllCards))
                    return HandType.Straight;
                if (isThreeOfAKind(AllCards))
                    return HandType.ThreeOfAKind;
                if (isTwoPair(AllCards))
                    return HandType.TwoPair;
                if (isPair(AllCards))
                    return HandType.OnePair;

                return HandType.HighCard;

            }

        }


        private Player player;
        private Game game;

        public List<Card> SortHand() {
            if (isFlush(AllCards) && isStraight(AllCards)) {
                return sortStraight();
            } else if (isFourOfAKind(AllCards)) {
                return sortFourOfAKind();
            } else if (isFlush(AllCards)) {
                return sortFlush();
            } else if (isStraight(AllCards)) {
                return sortStraight();
            } else if (isFullHouse(AllCards)) {
                return sortFullHouse();
            } else if (isThreeOfAKind(AllCards)) {
                return sortThreeOfAKind();
            } else if (isTwoPair(AllCards)) {
                return sortTwoPair();
            } else if (isPair(AllCards)) {
                return sortPair();
            } else {
                return sortHighCard();
            }
        }

        private List<Card> sortStraight() {
            AllCards.Sort(new CardComparerValue());
            List<Card> tempHand = new List<Card>();
            int straightCount = 0;
            int startValue = 0;
            int straightValue = (int)AllCards[0].Value;
            int straigtValueToCheck;
            bool cont = true;

            // Check for straight with Aces high
            for (int i = 1; i < AllCards.Count(); i++) {
                straigtValueToCheck = (int)AllCards[i].Value;

                if ((straightValue - 1) == straigtValueToCheck) {
                    straightValue--;
                    straightCount++;
                    if (straightCount >= 4) {
                        for (int x = 0; x < AllCards.Count; x++) {
                            if (x == startValue || x == (startValue + 1) || x == (startValue + 2) || x == (startValue + 3) || x == (startValue + 4))
                                tempHand.Add(AllCards[x]);

                        }
                        cont = false;
                        break;
                    }
                } else {
                    straightValue = straigtValueToCheck;
                    startValue = i;
                    straightCount = 0;
                }
            }
            if (cont == false) {
                return tempHand;
            }
            // Check for straight with Aces low
            AllCards.Sort(new CardCompareAcesLow());
            straightCount = 0;
            startValue = 0;
            straightValue = ((int)AllCards[0].Value == 14) ? 1 : (int)AllCards[0].Value;
            for (int i = 1; i < AllCards.Count(); i++) {
                straigtValueToCheck = ((int)AllCards[i].Value == 14) ? 1 : (int)AllCards[i].Value;
                if ((straightValue - 1) == straigtValueToCheck) {
                    straightValue--;
                    straightCount++;
                    if (straightCount >= 4) {
                        for (int x = 0; x < AllCards.Count; x++) {
                            if (x == startValue || x == (startValue + 1) || x == (startValue + 2) || x == (startValue + 3) || x == (startValue + 4))
                                tempHand.Add(AllCards[x]);
                        }
                        break;
                    }
                } else {
                    straightValue = straigtValueToCheck;
                    startValue = i;
                    straightCount = 0;
                }
            }
            return tempHand;
        }

        private List<Card> sortFlush() {
            Suits suit = Suits.Clubs;
            List<Card> tempHand = new List<Card>();

            int[] cardsPerSuit = new int[4];
            foreach (Card card in AllCards) {
                cardsPerSuit[(int)card.Suit]++;
                if (cardsPerSuit[(int)card.Suit] > 4) {
                    suit = card.Suit;
                    break;
                }
            }
            AllCards.Sort(new CardComparerValue());
            AllCards.Reverse();
            int ctr = 0;
            foreach (Card card in AllCards) {
                if ((card.Suit == suit) && ctr < 5) {
                    tempHand.Add(card);
                    ctr++;
                }
            }


            return tempHand;

        }

        private List<Card> sortFourOfAKind() {
            Dictionary<Values, int> count = new Dictionary<Values, int>();
            List<Card> temp = AllCards.ToList();
            List<Card> tempHand = new List<Card>();
            Values fourOfAKindValue = Values.Ace;
            count = getCardDictionary(AllCards);
            foreach (var item in count) {
                if (item.Value == 4)
                    fourOfAKindValue = (Values)item.Key;
            }


            tempHand = AllCards.Where(c => c.Value == fourOfAKindValue).ToList();
            temp = AllCards.Where(c => c.Value != fourOfAKindValue).ToList();
            temp.Sort(new CardComparerValue());
            tempHand.Add(temp[0]);

            return tempHand;

        }

        private List<Card> sortThreeOfAKind() {
            List<Card> tempHand = new List<Card>();
            List<Card> temp = AllCards.ToList();
            Dictionary<Values, int> count = getCardDictionary(AllCards);
            Values threeOfAKind = Values.Ace;

            foreach (var item in count) {
                if (item.Value == 3)
                    threeOfAKind = item.Key;
            }

            tempHand = AllCards.Where(c => c.Value == threeOfAKind).ToList();
            temp = AllCards.Where(c => c.Value != threeOfAKind).ToList();
            temp.Sort(new CardComparerValue());
            tempHand.Add(temp[0]);
            tempHand.Add(temp[1]);


            return tempHand;

        }

        private List<Card> sortPair() {
            List<Card> tempHand = new List<Card>();
            List<Card> temp = AllCards.ToList();
            Dictionary<Values, int> count = getCardDictionary(AllCards);
            Values pair = Values.Ace;

            foreach (var item in count) {
                if (item.Value == 2) {
                    pair = item.Key;
                    break;
                }
            }

            tempHand = AllCards.Where(c => c.Value == pair).ToList();
            temp = AllCards.Where(c => c.Value != pair).ToList();
            tempHand.Add(temp[0]);
            tempHand.Add(temp[1]);
            tempHand.Add(temp[2]);


            return tempHand;

        }

        private List<Card> sortFullHouse() {
            List<Card> tempHand = new List<Card>();

            Dictionary<Values, int> count = getCardDictionary(AllCards);
            Values threeOfAKind = Values.Ace;
            Values pair = Values.Ace;

            foreach (var item in count) {
                if (item.Value == 3)
                    threeOfAKind = item.Key;
                if (item.Value == 2)
                    pair = item.Key;
            }

            tempHand = AllCards.Where(c => c.Value == threeOfAKind || c.Value == pair).ToList();

            return tempHand;

        }

        private List<Card> sortTwoPair() {
            List<Card> tempHand = new List<Card>();
            List<Card> temp = new List<Card>();
            Dictionary<Values, int> count = getCardDictionary(AllCards);
            Values firstPair = Values.Ace;
            Values secondPair = Values.Two;
            bool firstPairExists = false;
            

            foreach (var item in count) {
                if ((item.Value == 2) && !firstPairExists) {
                    firstPair = item.Key;
                    firstPairExists = true;
                } else if ((item.Value == 2)) {
                    secondPair = item.Key;
                }
            }

            tempHand = AllCards.Where(c => c.Value == firstPair || c.Value == secondPair).ToList();
            temp = AllCards.Where(c => (c.Value != firstPair) && (c.Value != secondPair)).ToList();
            temp.Sort(new CardComparerValue());
            tempHand.Add(temp[0]);

            return tempHand;

        }

        private List<Card> sortHighCard() {
            List<Card> tempHand = new List<Card>();
            AllCards.Sort(new CardComparerValue());
            for (int i = 0; i < 5; i++) {
                tempHand.Add(AllCards[i]);
            }

            return tempHand;
        }

        public Hand(Player player, Game game) {
            this.player = player;
            this.game = game;
            AllCards = new List<Card>();
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
                if ((straightValue - 1) == straigtValueToCheck) {
                    straightValue--;
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
                if ((straightValue - 1) == straigtValueToCheck) {
                    straightValue--;
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
}
