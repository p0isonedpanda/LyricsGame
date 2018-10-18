using System;
using System.Text;

namespace LyricsGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebScraper scraper = new WebScraper();
            string lyrics = scraper.GetLyrics();

            GameController g = new GameController(lyrics);
            g.StartGame();
        }
    }
}
