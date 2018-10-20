using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;

namespace LyricsGame
{
    class WebScraper
    {
        private Settings s;

        public WebScraper ()
        {
            string file = File.ReadAllText("./settings.json");
            s = JsonConvert.DeserializeObject<Settings>(file);
        }
        
        public string GetLyrics()
        {
            string siteURL = "";
            while (true)
            {
                // Find the web page we want
                IList<Result> items = UserSearch();
    
                // Let the user select a result from the list
                DisplayResults(items);
                siteURL = GetSiteURL(items);

                // If the siteURL is empty, then the user will want to find another song
                if (siteURL != "") break;
            }

            // Scrape the page for the lyrics
            return Scrape(siteURL);
        }

        private IList<Result> UserSearch()
        {
            Console.Write("Enter a song: ");
            string query = Console.ReadLine();

            // Actually search for the lyrics
            CustomsearchService searchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = s.APIKey });
            CseResource.ListRequest listRequest = searchService.Cse.List(query);
            listRequest.Cx = s.EngineID;

            return listRequest.Execute().Items;
        }

        private void DisplayResults(IList<Result> _items)
        {
            for (int i = 0; i < 5; i++)
                {
                    string title = _items[i].Title.Split("Lyrics")[0]; // Get only the title and artist of the song
                    Console.WriteLine("[ {0} ] {1}", i + 1, title);
                }
                Console.WriteLine("[ 6 ] Search for another song");
        }

        private string GetSiteURL(IList<Result> _items)
        {
            while (true)
            {
                try
                {
                    Console.Write("Select a song (1-6): ");
                    int index = Int32.Parse(Console.ReadLine());

                    if (index == 6)
                    {
                        return "";
                    }

                    return _items[index - 1].Link;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a whole number");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Please enter a number between 1 and 5");
                }
            }
        }

        private string Scrape(string _siteURL)
        {
            string html;
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(_siteURL);
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
    
            // Use XQuery to locate the lyrics of the song. Is this a bad way of doing it?
            //
            // ...Probably
            try
            {
                HtmlNodeCollection htmlBody = htmlDoc.DocumentNode.SelectNodes("//comment()/..");

		        return htmlBody[8].InnerText;
            }
            catch
            {
                return "Song lyrics not found!";
            }
        }
    }
}