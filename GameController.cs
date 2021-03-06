using System;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace LyricsGame
{
    public class GameController
    {
        // Timer stuffs
        Thread timerThread;
        Clock timerClock;
        bool gameRunning;

        private string[] lyrics;
        private List<Line> gameLyrics;
        private Difficulty gameDifficulty;

        public GameController(string _lyrics)
        {
            // Initialise all the timer stuff
            timerThread = new Thread(new ThreadStart(RunTimer));
            timerClock = new Clock();
            gameRunning = true;

            lyrics = _lyrics.Split(new char[] { '\n', '\r' });
            lyrics = lyrics.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remove blank lines
            lyrics = lyrics.Where(x => x[0] != '[').ToArray(); // Remove any tags that indicate different artists

            char[] puncList = new char[] { '.', ',', '?', '!', ':', ';', '"' };

            // Remove any puncuation from the strings
            for (int i = 0; i < lyrics.Length; i++)
            {
                IEnumerable<char?> line = lyrics[i].Select(x => puncList.Contains(x) ? (char?)null : x);
                lyrics[i] = string.Concat(line.ToArray());
            }

            gameLyrics = new List<Line>(); // Until we decide on a difficulty we will just have an empty list
            gameDifficulty = Difficulty.Default;
        }

        public void StartGame()
        {
            SelectDifficulty();
            PrepareGame();
            
            // Now we get to the fun stuff
            Start();
        }

        private void SelectDifficulty()
        {
            while (true)
            {
                Console.WriteLine("[ 1 ] Easy\n[ 2 ] Medium\n[ 3 ] Hard");
                Console.Write("Select your difficulty (1-3): ");
                try
                {
                    int selection = Int32.Parse(Console.ReadLine());
                    if (selection < 1 || selection > 3) throw new IndexOutOfRangeException();
                    gameDifficulty = (Difficulty)selection - 1;
                    break;
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Please select an option between 1 and 3");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a whole number");
                }
            }
        }

        private void PrepareGame()
        {
            switch (gameDifficulty)
            {
                case Difficulty.Easy:
                    foreach (string s in lyrics)
                    {
                        gameLyrics.Add(new Easy(s));
                    }
                    break;

                case Difficulty.Medium:
                    foreach (string s in lyrics)
                    {
                        gameLyrics.Add(new Medium(s));
                    }
                    break;
                
                case Difficulty.Hard:
                    foreach (string s in lyrics)
                    {
                        gameLyrics.Add(new Hard(s));
                    }
                    break;
            }
        }

        private void Start()
        {
            // Start our timer
            timerThread.Start();

            foreach (Line l in gameLyrics)
            {
                while (true)
                {
                    // Show the progress of the line currently, then check if we've finished the line
                    Console.WriteLine(l.ProgressLine);
                    if (l.Solved) break; // Break out of loop if we're done with this line

                    Console.Write("> ");
                    string guess = Console.ReadLine();
                    l.GuessWord(guess);
                }
            }

            gameRunning = false;
            Console.WriteLine("Congratulations, you did it! Your time was: {0}", timerClock.CurrentTime);
        }

        private void RunTimer()
        {
            while (gameRunning)
            {
                Thread.Sleep(1000);
                timerClock.Tick();
            }
        }
    }
}