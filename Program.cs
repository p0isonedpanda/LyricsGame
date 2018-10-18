using System;

namespace LyricsGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebScraper scraper = new WebScraper();
            string lyrics = scraper.GetLyrics();

            Console.WriteLine(lyrics);
        }
    }
}
