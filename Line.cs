using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricsGame
{
    abstract class Line
    {
        private string finalLine;
        private string unsolvedLine;
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

        // Use this property to get the line partially solved based off user guesses
        public string ProgressLine
        {
            get
            {
                // Of course, if the user hasnt solved the line at all yet just return the unsolved one in full
                if (!wordsToSolve.ContainsValue(true)) return unsolvedLine;
                // And if it's all solved just return the final line in full
                if (!wordsToSolve.ContainsValue(false)) return finalLine;

                // Get a list of all the words that have been solved
                List<string> solvedWords = new List<string>();
                foreach (KeyValuePair<string, bool> s in wordsToSolve)
                {
                    if (s.Value) solvedWords.Add(s.Key);
                }

                string[] unsolved = unsolvedLine.Split(" ");
                string[] solved = finalLine.Split(" "); // Just to compare values

                // Now we grab all the indexes where the solved words are in the line...
                List<int> indexes = new List<int>();
                for (int i = 0; i < solved.Length; i++)
                {
                    foreach (string s in solvedWords)
                    {
                        if (s == StripPuncuation(solved[i].ToLower())) indexes.Add(i);
                    }
                }

                // ...and we go ahead and replace them...
                foreach (int i in indexes)
                {
                    unsolved[i] = solved[i];
                }

                // ...and finally construct the string
                string output = "";
                foreach (string s in unsolved)
                {
                    output += s + " ";
                }
                return output;
            }
        }

        public Line(string _finalLine, int _toRemove)
        {
            finalLine = _finalLine;
            unsolvedLine = "";
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
                        unsolvedLine += words[i] + " ";
                        continue;
                    }

                    if (!wordsToSolve.ContainsKey(words[i].ToLower()))
                        wordsToSolve.Add(StripPuncuation(words[i].ToLower()), false);
                    unsolvedLine += String.Concat(Enumerable.Repeat("_", words[i].Length)) + " ";
                }
            }
            else // If we're in hard mode then we just remove every word from the line
            {
                foreach (string s in words)
                {
                    // Add the word to the dictionary if we dont already have it in there
                    if (!wordsToSolve.ContainsKey(s.ToLower())) wordsToSolve.Add(StripPuncuation(s.ToLower()), false);
                    unsolvedLine += String.Concat(Enumerable.Repeat("_", s.Length)) + " ";
                }
            }

            // Remove the trailing space at the end of the line
            unsolvedLine = unsolvedLine.Substring(0, unsolvedLine.Length - 1);
        }

        public void GuessWord(string guess)
        {
            string[] guesses = StripPuncuation(guess.ToLower()).Split(" ");

            // Check to see if any of the words in the guess match up to words in the line
            foreach (string s in guesses)
            {
                if (wordsToSolve.ContainsKey(s))
                {
                    wordsToSolve[s] = true;
                }
            }
        }

        private string StripPuncuation(string _line)
        {
            return new string(_line.Where(c => !char.IsPunctuation(c)).ToArray());
        }
    }
}