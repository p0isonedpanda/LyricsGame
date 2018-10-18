using System;
using System.Linq;
using System.Collections.Generic;

namespace LyricsGame
{
    public class GameController
    {
        private string[] lyrics;

        public GameController(string _lyrics)
        {
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
        }

        public void StartGame()
        {
            foreach (string s in lyrics)
            {
                Console.WriteLine(s);
            }
        }
    }
}