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
    public class Program
    {
        static void Main(string[] args)
        {
            // Read in settings
            string file = File.ReadAllText("./settings.json");
            Settings s = JsonConvert.DeserializeObject<Settings>(file);

            string siteURL = "";
            while (true)
            {
                // Find the web page we want
                Console.Write("Enter a song: ");
                string query = Console.ReadLine();
                var searchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = s.APIKey });
                var listRequest = searchService.Cse.List(query);
                listRequest.Cx = s.EngineID;
    
                // Let the user select a result from the list
                IList<Result> items = listRequest.Execute().Items;
                
                for (int i = 0; i < 5; i++)
                {
                    string title = items[i].Title.Split("Lyrics")[0]; // Get only the title and artist of the song
                    Console.WriteLine("[ {0} ] {1}", i + 1, title);
                }
                Console.WriteLine("[ 6 ] Search for another song");
    
                // This entire look is kinda cheese, but it works?
                while (true)
                {
                    try
                    {
                        Console.Write("Select a song (1-6): ");
                        int index = Int32.Parse(Console.ReadLine());
    
                        if (index == 6)
                        {
                            break;
                        }
    
                        siteURL = items[index - 1].Link;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please enter a whole number");
                        continue;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Please enter a number between 1 and 5");
                        continue;
                    }
    
                    break;
                }

                // If the siteURL is empty, then the user will want to find another song
                if (siteURL != "") break;
            }


            // Retrieve the web page
            string html;
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(siteURL);
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
    
            // Use XQuery to locate the lyrics of the song
            try
            {
                HtmlNodeCollection htmlBody = htmlDoc.DocumentNode.SelectNodes("//comment()/..");

                // Print the lyrics
		        Console.WriteLine(htmlBody[8].InnerText);
            }
            catch
            {
                Console.WriteLine("Song lyrics not found!");
            }
        }
    }
}
