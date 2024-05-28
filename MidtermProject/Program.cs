using System;
using System.Collections.Generic;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Random num = new Random();
            Console.WriteLine("Enter the number of players (2-4):");
            int playerCount = int.Parse(Console.ReadLine());
            playerCount = Math.Max(2, Math.Min(4, playerCount));

            while (true)  // Main loop to allow replaying the game.
            {
                List<Card> deck = InitializeDeck(); // Initialize and create a full deck of cards.
                ShuffleDeck(deck, num); // Shuffle the deck before starting.


                bool[] activePlayers = new bool[playerCount];

                int[] playerMoney = new int[playerCount];

                Array.Fill(activePlayers, true);

                Array.Fill(playerMoney, 500);

                int[] bets = new int[playerCount];

                int activeCount = playerCount;


                // Inner loop for each round of the game.
                while (activeCount > 1 && deck.Count > playerCount * 2)  
                {
                    // Collect bets from each player
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (activePlayers[i])
                        {
                            Console.WriteLine($"Player {i + 1} has ${playerMoney[i]}.");
                            Console.WriteLine($"Player {i + 1}, how much do you want to bet?");
                            int bet = int.Parse(Console.ReadLine());
                            bet = Math.Min(bet, playerMoney[i]); // Bet cannot exceed player's current money.
                            bets[i] = bet;
                        }
                    }
                    Console.WriteLine("-------------------------------------------");

                    Card computerCard1 = DrawCard(deck);
                    Console.WriteLine($"Computer's first card: {computerCard1.DisplayValue()}");
                    Console.WriteLine("-------------------------------------------");
                    int computerTotal = AdjustAceValue(computerCard1.Value);
                    bool anyTwoCards = false;

                    int[] playerTotals = new int[playerCount];
                    int[] choices = new int[playerCount];

                    // Player card draw phase
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (activePlayers[i])
                        {
                            Console.WriteLine($"Player {i + 1}, do you want to draw 1 or 2 cards? (Enter 1 or 2):");
                            int choice = int.Parse(Console.ReadLine());
                            choices[i] = choice;

                            Card playerCard1 = DrawCard(deck); // Draw first card for player.
                            playerTotals[i] = AdjustAceValue(playerCard1.Value);
                            Console.WriteLine($"Player {i + 1} drew: {playerCard1.DisplayValue()}");

                            if (choice == 2)
                            {
                                Card playerCard2 = DrawCard(deck); // Draw second card if chosen.
                                playerTotals[i] += AdjustAceValue(playerCard2.Value);
                                Console.WriteLine($"and {playerCard2.DisplayValue()}");
                                anyTwoCards = true;
                            }
                            Console.WriteLine("-------------------------------------------");

                        }
                    }

                    // Draw a second card for the computer if any player chose to draw two cards.
                    if (anyTwoCards)
                    {
                        Card computerCard2 = DrawCard(deck);
                        computerTotal += AdjustAceValue(computerCard2.Value);
                        Console.WriteLine($"Computer draws another card: {computerCard2.DisplayValue()}");
                    }


                    Console.WriteLine($"Computer's total card value: {computerTotal}");
                    Console.WriteLine("-------------------------------------------");


                    // Determine outcomes
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (activePlayers[i])
                        {
                            bool playerWins = (choices[i] == 1 && playerTotals[i] < computerCard1.Value) ||
                                              (choices[i] == 2 && playerTotals[i] > computerTotal);

                            if (playerWins)
                            {
                                playerMoney[i] += bets[i];
                                Console.WriteLine($"Player {i + 1} wins this round and now has ${playerMoney[i]}.");
                            }
                            else
                            {
                                playerMoney[i] -= bets[i];
                                Console.WriteLine($"Player {i + 1} loses this round and now has ${playerMoney[i]}.");
                                if (playerMoney[i] <= 0)
                                {
                                    Console.WriteLine($"Player {i + 1} is eliminated!");
                                    activePlayers[i] = false;
                                    activeCount--;
                                }
                            }
                        }
                    }

                    // Prepare for the next round.
                    if (activeCount > 1 && deck.Count > playerCount * 2)
                    {
                        Console.WriteLine("Next round begins...");
                        Console.WriteLine("-------------------------------------------");
                    }
                    else
                    {
                        break;  // No more cards or players to continue the current game.
                    }
                }

                // Ask players if they want to play again.
                Console.WriteLine("Do you want to play again? (yes/no):");
                if (Console.ReadLine().Trim().ToLower() != "yes")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Enter the number of players (2-4):");
                    playerCount = int.Parse(Console.ReadLine());
                    playerCount = Math.Max(2, Math.Min(4, playerCount));
                }
            }

            Console.WriteLine("Thanks for playing!");
        }

        //Initialize the deck with 52 cards.
        static List<Card> InitializeDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            List<Card> deck = new List<Card>();
            foreach (string suit in suits)
            {
                for (int value = 1; value <= 13; value++)
                {
                    deck.Add(new Card(value, suit));
                }
            }
            return deck;
        }

        //Shuffle the deck.
        static void ShuffleDeck(List<Card> deck, Random num)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = num.Next(i + 1);
                Card temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }

        //Draw a card from the deck.
        static Card DrawCard(List<Card> deck)
        {
            Card card = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            return card;
        }

        // Adjusts the card value for Aces and face cards.
        static int AdjustAceValue(int cardValue)
        {
            return cardValue == 1 ? 11 : cardValue > 10 ? 10 : cardValue;
        }
    }

    class Card
    {
        public int Value { get; }
        public string Suit { get; }

        public Card(int value, string suit)
        {
            Value = value;
            Suit = suit;
        }

        // Display the card's value and suit in a readable format.
        public string DisplayValue()
        {
            return Value switch
            {
                13 => "King",
                12 => "Queen",
                11 => "Jack",
                1 => "Ace",
                _ => Value.ToString()
            } + " of " + Suit;
        }
    }
}
