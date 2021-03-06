﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemChallenge
{
    class Game
    {
        internal List<Card> CommunityCards;
        internal List<Player> Players;
        internal Deck GameDeck;
        private int numberOfPlayers;

        public bool Playing { get; set; }


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
                player.PlayerHand.AllCards.Sort(new CardComparerValue());
                foreach (Card c in player.PlayerHand.BestHand) {
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
            List<Player> finalWinner = new List<Player>();
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
                foreach (Player player in winner) {
                    player.PlayerHand.BestHand.Sort(new CardComparerValue());
                    Console.WriteLine("   {0} has a {1}", player.Name, player.PlayerHand.Type);
                    
                }
                finalWinner = tieBreaker(winner);
                if (finalWinner.Count > 1) {
                    Console.WriteLine("\n********** A Tie! Pot is split between **********\n");
                    foreach (Player p in finalWinner) {
                        Console.WriteLine(" {0} has a {1}", p.Name, p.PlayerHand.Type);
                    }
                } else {
                    Console.WriteLine("A winner?\n");
                    foreach (Player p in finalWinner) {
                        Console.WriteLine(" {0} has a {1}", p.Name, p.PlayerHand.Type);
                    }
                }
                
            }

        }

        private List<Player> tieBreaker(List<Player> winner) {
            int pToCompare;
            int compareResult = 0;
            List<Player> tempList = new List<Player>();
            tempList.Add(winner[0]);
            for (int i = 0; i < winner.Count; i++) {
                pToCompare = ((i + 1) < winner.Count) ? (i + 1) : winner.Count - 1;
                compareResult = compareHand(tempList[0].PlayerHand, winner[pToCompare].PlayerHand);
                if (compareResult < 1) {
                    tempList.Clear();
                    tempList.Add(winner[pToCompare]);
                } else if (compareResult == 0)
                    tempList.Add(winner[pToCompare]);

            }
            return tempList;
        }

        private int compareHand(Hand p1, Hand p2) {
            p1.BestHand.Sort(new CardComparerValue());
            p2.BestHand.Sort(new CardComparerValue());
            for (int i = 0; i < 5; i++) {
                if (p1.BestHand[i].Value > p2.BestHand[i].Value)
                    return 1;
                else if (p1.BestHand[i].Value < p2.BestHand[i].Value)
                    return -1;
            }
            return 0;
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
                    for (int i = 0; i < 3; i++) {
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
}
