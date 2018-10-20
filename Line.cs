using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricsGame
{
    abstract class Line
    {
        public readonly string FinalLine;
        public string UnsolvedLine;
        private Dictionary<string, bool> wordsToSolve;

        public bool Solved
        {
            get
            {
                // Iterate through the dictionary to see if we have solved the line
                foreach (KeyValuePair<string, bool> w in wordsToSolve)
                {
                    if (!w.Value) return false;
                }

                return true;
            }
        }

        public Line(string _finalLine, int _toRemove)
        {
            FinalLine = _finalLine;
            UnsolvedLine = "";
            wordsToSolve = new Dictionary<string, bool>();
            Prepare(_finalLine, _toRemove); // Let the prepare method handle removing words
        }

        private void Prepare(string _toPrepare, int _toRemove)
        {
            string[] words = _toPrepare.Split(" ");

            // Mostly to check if we're not playing in hard mode
            if (_toRemove < words.Length)
            {
                int[] indexes = new int[_toRemove];
                Random rnd = new Random();
    
                for (int i = 0; i < _toRemove; i++)
                {
                    // Loop while we don't have an array of unique indexes
                    while (true)
                    {
                        int newIndex = rnd.Next(0, words.Length);
                        if (!indexes.Contains(newIndex))
                        {
                            indexes[i] = newIndex;
                            break;
                        }
                    }
                }

                // Once we have all of our unique indexes, we can start removing words
                for (int i = 0; i < words.Length; i++)
                {
                    // Go back to the start of the loop if we're not removing this word
                    if (!indexes.Contains(i))
                    {
                        UnsolvedLine += words[i] + " ";
                        continue;
                    }

                    if (!wordsToSolve.ContainsKey(words[i].ToLower()))
                        wordsToSolve.Add(words[i].ToLower(), false);
                    UnsolvedLine += String.Concat(Enumerable.Repeat("_", words[i].Length)) + " ";
                }
            }
            else // If we're in hard mode then we just remove every word from the line
            {
                foreach (string s in words)
                {
                    // Add the word to the dictionary if we dont already have it in there
                    if (!wordsToSolve.ContainsKey(s.ToLower())) wordsToSolve.Add(s.ToLower(), false);
                    UnsolvedLine += String.Concat(Enumerable.Repeat("_", s.Length)) + " ";
                }
            }

            // Remove the trailing space at the end of the line
            UnsolvedLine = UnsolvedLine.Substring(0, UnsolvedLine.Length - 1);
        }

        public bool GuessWord(string guess)
        {
            try
            {
                wordsToSolve[guess.ToLower()] = true;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}