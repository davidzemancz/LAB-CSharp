using System;
using System.Collections;
using System.Collections.Generic;

namespace Poker
{
  
    internal class Program
    {
        static void Main(string[] args)
        {
            // For debugging
            bool debug = true;

            try
            {
                // Check args
                if (!debug)
                {
                    if (args.Length != 1)
                    {
                        Console.WriteLine("Usage: Program.exe file...");
                        return;
                    }
                }

                // Read input filename
                string filename = debug ? @"C:\Users\david.zeman\OneDrive\MFFUK\2. ročník\ZS\C#\LAB\LAB\Poker\test_data\fail3.in.txt" : args[0];

                // Prepare IO
                IInputReader inputReader;
                IOutputWriter outputWriter;
                using (inputReader = new FileInputReader(filename).Open())
                using (outputWriter = new ConsoleOutputWriter().Open())
                {
                    Game game;
                    IGameIO gameIO;

                    // Read count of players and init game
                    int players = int.Parse(inputReader.ReadLine());
                    game = new Game(players);
                    gameIO = new GameIO();

                    // Read rounds and evaluate
                    while (!inputReader.AtEnd)
                    {
                        // Read cards for each player
                        Card[][] cardsForPlayers = new Card[game.Players.Length][];
                        for (int p = 0; p < game.Players.Length; p++)
                        {
                            string line = inputReader.ReadLine();
                            if (string.IsNullOrEmpty(line)) throw new FormatException($"Invalid input format. Missing cards for player {p}.");

                            cardsForPlayers[p] = gameIO.ReadPlayerCards(line);
                        }

                        // Read empty line
                        string emptyLine = inputReader.ReadLine();
                        if (!string.IsNullOrEmpty(emptyLine)) throw new FormatException($"Invalid input format. Remaining cards.");

                        // Play round
                        game.StartRound(cardsForPlayers);
                        game.EvaluateRound();

                        // Write players infos
                        for (int p = 0; p < game.Players.Length; p++)
                        {
                            string playerInfo = gameIO.WritePlayerInfo(game.Players[p]);
                            outputWriter.WriteLine(playerInfo);
                        }

                        // New line for next round
                        outputWriter.WriteLine("");
                    }

                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Unexpected error]" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
